using System;
using System.Collections.Generic;

namespace Xicheng.UI
{
    // 链表节点定义
    /// <summary>
    /// UI状态定义
    /// </summary>
    public class UIStateNode
    {
        public UIBase Instance; //UI实例
        public List<UIKey> HiddenChildren; // 该UI打开时隐藏的子级（层级互斥时用到）
        public DateTime OpenTime; //打开UI的时间节点

        public void UpdateOpenTime()
        {
            OpenTime = DateTime.Now;
        }
    }
}