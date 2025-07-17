using System;
using xicheng.module.ui;

namespace Xicheng.UI
{
    public class UITest:UIBase
    {
        public override UIKey UIKey { get; }

 
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