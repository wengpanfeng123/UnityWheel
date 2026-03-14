/*************************************************************************
* Copyright  xicheng. All rights reserved.
*------------------------------------------------------------------------
* File     : UIRoot.cs.meta
* Author   : xicheng
* Date     : 2025-09-10 17:18
* Tips     : xicheng知识库
* Description : UI根节点
*************************************************************************/

using UnityEngine;
using Xicheng.UIAdapter;

namespace Xicheng.UI
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] public Transform safeArea;
        [SerializeField] public Camera uiCam;
        [SerializeField] public UISafeAreaAdapter safeAreaAdapter;
        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}