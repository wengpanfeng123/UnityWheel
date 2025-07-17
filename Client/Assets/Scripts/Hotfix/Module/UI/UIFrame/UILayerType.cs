namespace Xicheng.UI
{
    public enum UILayerType
    {
        Debug,          // 调试层（日志/性能面板）
        Background=1001,     // 背景层（过场图/场景UI）
        MainHUD,        // 主界面层（血条/小地图）
        Function,       // 功能层（商城/背包）
        Popup,          // 弹窗层（设置/确认框）
        Toast,          // 通知层（Toast/系统提示）
        Animation,      // 动效层（全屏转场特效）
        Cinema,         // 演出层（剧情对话/过场）
        SystemTop       // 系统顶层（Loading/紧急维护）
    }
}