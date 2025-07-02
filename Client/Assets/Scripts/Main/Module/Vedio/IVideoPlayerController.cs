namespace xicheng.Vedio
{
    /// <summary>
    /// 视频播放器接口定义
    /// </summary>
    public interface IVideoPlayerController
    {
        // 播放控制
        void Play();
        void Pause();
        void Stop();
    
        // 进度控制
        void SetProgress(float progress);
        float GetProgress();
    
        // 音量控制
        void SetVolume(float volume);
        float GetVolume();
    
        // 状态查询
        bool IsPlaying { get; }
        bool IsPaused { get; }
        float Duration { get; }
    
        // 事件回调
        event System.Action OnPlayStarted;
        event System.Action OnPlayPaused;
        event System.Action OnPlayStopped;
        event System.Action OnPlayCompleted;
        event System.Action<string> OnError;
    } 
}