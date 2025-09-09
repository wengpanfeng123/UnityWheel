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
    }
}