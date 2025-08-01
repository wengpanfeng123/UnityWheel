using Xicheng.module.ui;

namespace Xicheng.UI
{
    public interface IUIBase
    {
        //打开时初始化一次。关闭和隐藏不会触发
        void OnInit(UIKey uiKey);

        void OnShow(object args); //UI展示

        void OnHide(); //UI隐藏

        void OnClose(); //UI关闭。实际实例销毁，也会调用OnHide
    }
}