using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HsJam;
using HybridCLR;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using Xicheng.Common;
using Object = UnityEngine.Object;

namespace Xicheng.AOT
{
    /// <summary>
    /// AOT启动脚本
    /// </summary>
    public class AOTStart : MonoBehaviour
    {
        private byte[] _dllBytes; //dll字节数组

        [SerializeField] public UIPatch uiPatch;

        void Start()
        {
            InitUIPatch();
            StartCoroutine(GameLaunch());
        }

        private void InitUIPatch()
        {
            uiPatch.SetStage("Start Game");
            GameObject clone = Instantiate(uiPatch.gameObject);
            uiPatch = clone.GetComponent<UIPatch>();
        }

        private IEnumerator GameLaunch()
        {
            yield return UpdateAddressable(); // 检测更新ab
            yield return LoadDll(); //加载热更ab
            EnterGame(); //启动游戏
        }

        private IEnumerator UpdateAddressable()
        {
            uiPatch.SetStage("Check Resource Update");
            AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync(); //初始化aa系统
            yield return initHandle;

            // 检测更新
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            yield return checkHandle;
            if (checkHandle.Status != AsyncOperationStatus.Succeeded)
            {
                 ULog.Error($"CheckForCatalogUpdates Error{checkHandle.OperationException}");
                uiPatch.SetStage("CheckForCatalogUpdates error ");
                yield break;
            }

            if (checkHandle.Result.Count > 0)
            {
                var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
                yield return updateHandle;

                if (updateHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    ULog.Error($"UpdateCatalogs Error{updateHandle.OperationException.ToString()}");
                    uiPatch.SetStage("Update catalogs  error !");
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
                         ULog.Error($"GetDownloadSizeAsync Error{sizeHandle.OperationException}");
                        yield break;
                    }

                    long totalDownloadSize = sizeHandle.Result;
                     ULog.InfoWhite("download size : " + totalDownloadSize);
                    if (totalDownloadSize > 0)
                    {
                        // 下载依赖
                        var downloadHandle = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union);
                        while (!downloadHandle.IsDone)
                        {
                            if (downloadHandle.Status == AsyncOperationStatus.Failed)
                            {
                                 ULog.Error($"DownloadDependenciesAsync Error{downloadHandle.OperationException}");
                                yield break;
                            }

                            // 下载进度
                            float percentage = downloadHandle.PercentComplete;
                            uiPatch.SetProgress(percentage);
                            yield return null;
                        }

                        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                             ULog.InfoWhite("下载完毕!");
                            uiPatch.SetStage("Download resources finish");
                            uiPatch.SetProgress(1);
                        }
                    }
                    else
                    {
                         ULog.InfoWhite("无下载");
                        uiPatch.SetStage("no download resources");
                        uiPatch.SetProgress(1);
                    }
                }
            }
            else
            {
                ULog.InfoWhite("没有检测到更新!");
                uiPatch.SetStage("check no update");
                uiPatch.SetProgress(1);
            }
        }

        private IEnumerator LoadDll()
        {
            uiPatch.SetStage("load meta and update dll,reload catalog");
            yield return LoadMetadataForAOTAssemblies();
            yield return LoadGameHotUpdateDll();
            yield return ReloadAddressableCatalog();
            ULog.InfoWhite("[LoadDll] LoadAssemblies finish!");
            yield return null;
        }

        private IEnumerator LoadGameHotUpdateDll()
        {
            ReadDllBytes(MainConst.HotUpdateDllPath);
            if (_dllBytes != null)
            {
                var assembly = Assembly.Load(_dllBytes);
                 ULog.InfoWhite($"Load Assembly success,assembly Name:{assembly.FullName}");
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
                     ULog.InfoWhite($"LoadMetadataForAOTAssembly:{aotDllName}. ret:{err}");
                }
            }

            yield return null;

             ULog.InfoWhite("LoadMetadataForAOTAssemblies finish!");
        }

        private void ReadDllBytes(string path)
        {
            var dllText = LoadAsset<TextAsset>(path);

            if (dllText == null)
            {
                 ULog.Error($"[ReadDllBytes] can not load dllText,path:{path}");
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
                 ULog.Error(
                    $"load content catalog failed, exception:{op.OperationException.Message} \r\n {op.OperationException.StackTrace}");
            }
        }

        private void EnterGame()
        {
            var op = Addressables.LoadSceneAsync(MainConst.GameStartPath);
            op.Completed += handle => { ULog.InfoWhite("[EnterGame] Load 'GameStart.unity' end"); };
        }

        #endregion


        private void OnDestroy()
        {
            if (uiPatch != null)
            {
                Destroy(uiPatch.gameObject);
            }
        }
    }
}