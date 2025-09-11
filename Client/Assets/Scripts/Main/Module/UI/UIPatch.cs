/*************************************************************************
* Copyright  xicheng. All rights reserved.
*------------------------------------------------------------------------
* File     : UIPatch.cs.meta
* Author   : xicheng
* Date     : 2025-09-10 15:54
* Tips     : xicheng知识库
* Description : 
*************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HsJam
{
    public class UIPatch : MonoBehaviour
    {
        [SerializeField]public Slider slider;
        [SerializeField]public TextMeshProUGUI txtTotalCount; //总数量
        [SerializeField]public TextMeshProUGUI txtUpdatedCount; //已更新的数量
        [SerializeField]public TextMeshProUGUI txtTotalSize; //总大小
        [SerializeField]public TextMeshProUGUI txtUpdatedSize; //已更新的大小
        [SerializeField]public TextMeshProUGUI txtStageDesc; //阶段描述
        [SerializeField]public TextMeshProUGUI txtTitle; //阶段描述

        public void SetTotal(int totalCount,float totalSize)
        {
            txtTotalCount.text = totalCount.ToString();
            txtTotalSize.text = totalSize.ToString(CultureInfo.InvariantCulture);
        }

        public void UpdateCount(int updateCount)
        {
            txtUpdatedCount.text = updateCount.ToString();
        }
        
        public void UpdateSize(int updateSize)
        {
            txtUpdatedCount.text = txtUpdatedSize.ToString();
        }

        public void SetStage(string stage)
        {
            txtStageDesc.text = stage;
        }
       
        
        public void SetProgress(float progressValue)
        {
            slider.value = progressValue;
            Debug.Log($"下载进度: {progressValue * 100}%");
        }

        public void OnDestroy()
        {
            
        }
    }
}