namespace Xicheng.Audio
{
    public enum AudioType
    {
        /// <summary>背景音乐</summary>
        BGM,

        /// <summary>UI音效：按钮、界面</summary>
        UI,

        /// <summary>
        /// 环境音:
        /// 天气音效:下雨、打雷、下雪、起雾
        /// 地形环境音:森林的鸟鸣 / 风声、沙漠的风沙声、海洋的波浪声、雪地的踏雪声
        /// 地图交互音:特殊地形（如沼泽、岩浆）的音效、触发地图事件（如陷阱、补给点）的声音。
        /// </summary>
        Ambient,


        /// <summary>语音播报</summary>
        Voice,

        /// <summary>其它</summary>
        SFX,
    }
}