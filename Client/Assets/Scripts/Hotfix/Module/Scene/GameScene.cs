using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Xicheng.Common;
using Xicheng.Utility;

namespace Xicheng.Secne
{
    public class GameScene:MonoSingleton<GameScene>
    {
        public string TargetScenePath { get; private set; }
        public LoadSceneMode LoadSceneMode { get; private set; }

        public bool ActiveOnLoad { get; private set; }

        /// <summary>
        /// 带loading的场景切换。
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="mode"></param>
        /// <param name="activeOnLoad"></param>
        public void LoadingSceneAsync(string assetPath,LoadSceneMode mode = LoadSceneMode.Single,bool activeOnLoad = true)
        {
            TargetScenePath = assetPath;
            LoadSceneMode = mode;
            ActiveOnLoad = activeOnLoad;
            //同步把loading加载出来。
            Addressables.LoadSceneAsync(MainConst.LoadingScenePath).Completed += OnLoadingEnd;
        }

        
        //不带loading直接切场景。
        public void LoadSceneAsync(string assetPath,LoadSceneMode mode = LoadSceneMode.Single,bool activeOnLoad = true)
        {
            ClearGC();
            Addressables.LoadSceneAsync(assetPath, LoadSceneMode).Completed  +=(p) =>
            {
                Debug.Log($"场景{assetPath}加载成功");
            };
        }

        public void ClearGC()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        private void OnLoadingEnd(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("场景Loading加载成功");
                //切场景的时候GC一下。
                ClearGC();
            }
            else
            {
                Debug.LogError($"场景Loading加载失败: {handle.OperationException}");
            }
        
            // 释放句柄
            Addressables.Release(handle);
        }
    }
}