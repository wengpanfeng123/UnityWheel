using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// AOT启动脚本
/// </summary>
public class AotStart : MonoBehaviour
{
    public Slider slider;
    private byte[] _dllBytes;

    private readonly string _gameStartScenePath = "Assets/AssetsPackage/Scenes/GameStart.unity";
    private readonly string _hotUpdateDllPath = "Assets/AssetsPackage/HotUpdateDlls/HotUpdateDll/HotUpdate.dll.bytes";
    
    void Start()
    {
        StartCoroutine(GameLaunch());
    }

    private IEnumerator GameLaunch()
    {
        yield return DoUpdateAddressable(); // 检测更新ab
        yield return LoadDll(); //加载热更ab
       EnterGame();//启动游戏
    }
    
    IEnumerator DoUpdateAddressable()
    {
        AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 检测更新
        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        if (checkHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"CheckForCatalogUpdates Error{checkHandle.OperationException.ToString()}");
            yield break;
        }

        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
            yield return updateHandle;

            if (updateHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"UpdateCatalogs Error{updateHandle.OperationException.ToString()}");
                yield break;
            }

            // 更新列表迭代器
            List<IResourceLocator> locators = updateHandle.Result;
            foreach (var locator in locators)
            {
                List<object> keys = new List<object>();
                foreach (var key in locator.Keys)
                {
                    if (key is string)
                    {
                        keys.Add(key);
                    }
                }

                var sizeHandle = Addressables.GetDownloadSizeAsync(keys);
                yield return sizeHandle;
                if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"GetDownloadSizeAsync Error{sizeHandle.OperationException.ToString()}");
                    yield break;
                }

                long totalDownloadSize = sizeHandle.Result;
                Debug.Log("download size : " + totalDownloadSize);
                if (totalDownloadSize > 0)
                {
                    // 下载
                    var downloadHandle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
                    while (!downloadHandle.IsDone)
                    {
                        if (downloadHandle.Status == AsyncOperationStatus.Failed)
                        {
                            Debug.LogError(
                                $"DownloadDependenciesAsync Error{downloadHandle.OperationException.ToString()}");
                            yield break;
                        }

                        // 下载进度
                        float percentage = downloadHandle.PercentComplete;
                        Debug.Log($"已下载: {percentage}");
                        slider.value = percentage;
                        yield return null;
                    }

                    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("下载完毕!");
                        slider.value = 1f;
                    }
                }
                else
                {
                    Debug.Log("无下载");
                    slider.value = 1f;
                }
            }
        }
        else
        {
            Debug.Log("没有检测到更新!");
            slider.value = 1f;
        }

        // 进游戏
    }

    private IEnumerator LoadDll()
    {
        yield return LoadMetadataForAOTAssemblies();
        yield return LoadGameHotUpdateDll();
        yield return ReloadAddressableCatalog();
        ULog.InfoCyan("[LoadDll] LoadAssemblies finish!");
        yield return null;
    }

    private IEnumerator LoadGameHotUpdateDll()
    {
        ReadDllBytes(_hotUpdateDllPath);
        if (_dllBytes != null)
        {
            var assembly = Assembly.Load(_dllBytes);
            Debug.Log($"Load Assembly success,assembly Name:{assembly.FullName}");
        }
        yield return null;
    }

    //补充元数据
    private IEnumerator LoadMetadataForAOTAssemblies()
    {
        List<string> aotDllList = new List<string>
        {
            "mscorlib.dll",
            //"System.dll",
            //"System.Core.dll", // 如果使用了Linq，需要这个
        };

        foreach (var aotDllName in aotDllList)
        {
            var path = $"Assets/AssetsPackage/HotUpdateDlls/MetaDataDll/{aotDllName}.bytes";
            ReadDllBytes(path);
            if (_dllBytes != null)
            {
                var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(_dllBytes, HomologousImageMode.SuperSet);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
            }
        }

        yield return null;

        Debug.Log("LoadMetadataForAOTAssemblies finish!");
    }

    private void ReadDllBytes(string path)
    {
        var dllText = LoadAsset<TextAsset>(path);

        if (dllText == null)
        {
            Debug.LogError($"[ReadDllBytes] cant load dllText,path:{path}");
            _dllBytes = null;
        }
        else
        {
            _dllBytes = dllText.bytes;
        }

        UnloadAsset(dllText);
    }

    #region 为了方便 所以写到这里了

    public T LoadAsset<T>(string path)
    {
        var op = Addressables.LoadAssetAsync<T>(path);
        if (!op.IsValid())
            return default;
        op.WaitForCompletion();
        return op.Result;
    }
 

    private void UnloadAsset(Object asset)
    {
        if (asset != null)
            Addressables.Release(asset);
    }

    //https://blog.csdn.net/weixin_43329960/article/details/142097230?spm=1001.2101.3001.6650.3&utm_medium=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ECtr-3-142097230-blog-139867886.235%5Ev43%5Epc_blog_bottom_relevance_base4&depth_1-utm_source=distribute.pc_relevant.none-task-blog-2%7Edefault%7EBlogCommendFromBaidu%7ECtr-3-142097230-blog-139867886.235%5Ev43%5Epc_blog_bottom_relevance_base4&utm_relevant_index=5
    // 加载完dll 要  重新加载catalog--官网说的=
    private IEnumerator ReloadAddressableCatalog()
    {
        var op = Addressables.LoadContentCatalogAsync($"{Addressables.RuntimePath}/catalog.json");
        yield return op;
        if (op.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(
                $"load content catalog failed, exception:{op.OperationException.Message} \r\n {op.OperationException.StackTrace}");
        }
    }
    
    private void EnterGame()
    {
        var op = Addressables.LoadSceneAsync(_gameStartScenePath);
        op.Completed += handle =>
        {
            ULog.InfoWhite("[EnterGame] Load 'GameStart' SceneEnd");
        };
    }

    #endregion
}