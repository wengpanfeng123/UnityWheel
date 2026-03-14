using System;
using Xicheng.module.ui;

namespace Xicheng.UI
{
    public class UITest:UIBase
    {
        public override UIKey _UIKey_ { get; }

 
        public override void OnShow(object args)
        {
            base.OnShow(args);
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }
}