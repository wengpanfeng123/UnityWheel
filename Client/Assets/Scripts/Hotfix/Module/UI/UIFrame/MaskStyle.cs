namespace xicheng.ui
{
    public enum MaskStyle {
        BlackTransparent, // 半透黑
        Blur,            // 高斯模糊
        CustomTexture    // 自定义贴图
    }

    public enum UIMutexRule
    {
        Mutex =0, //互斥关闭
        Overlay = 1, //覆盖共存显示
    }
}