using System;
using xicheng.module.ui;

namespace Xicheng.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIInitAttribute:Attribute
    {
        public UIKey Key { get; }
        public UIInitAttribute(UIKey key)
        {
            this.Key = key;
        }
    }
}