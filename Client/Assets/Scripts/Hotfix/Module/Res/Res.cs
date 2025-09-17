/*************************************************************************
* Copyright  xicheng. All rights reserved.
*------------------------------------------------------------------------
* File     : Res.cs 
* Author   : xicheng
* Date     : 2025-09-09 11:24
* Tips     : xicheng知识库
* Description : Addressable资源加载包装类
*************************************************************************/

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Xicheng.Resource
{
    /// <summary>
    /// 简单包装了Addressables资源加载
    /// </summary>
    public static class Res
    {
        public static T LoadAsset<T>(string path)
        {
            var op = Addressables.LoadAssetAsync<T>(path);
            if (!op.IsValid())
                return default;
            op.WaitForCompletion();
            return op.Result;
        }


        public static Sprite LoadSprite(string path)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(path);
            if (!handle.IsValid())
                return null;

            handle.WaitForCompletion();
            return handle.Result;
        }
        
        public static Texture2D LoadTexture(string path)
        {
            var handle = Addressables.LoadAssetAsync<Texture2D>(path);
            if (!handle.IsValid())
                return null;

            handle.WaitForCompletion();
            return handle.Result;
        }


        public static void UnloadAsset(Object asset)
        {
            Addressables.Release(asset);
        }
        
        #region 实例化、销毁对象
        public static GameObject Instantiate(string path)
        {
            var op = Addressables.InstantiateAsync(path);
            return op.Result;
        }

        public static void ReleaseInstance(GameObject instance)
        {
            Addressables.ReleaseInstance(instance);
        }
        
        #endregion
    }
}