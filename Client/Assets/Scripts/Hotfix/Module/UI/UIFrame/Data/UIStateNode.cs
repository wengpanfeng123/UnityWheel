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
        public UIBase Instance;
        public List<UIKey> HiddenChildren; // 该UI打开时隐藏的子级
        public DateTime OpenTime;

        public void UpdateOpenTime()
        {
            OpenTime = DateTime.Now;
        }
    }
}