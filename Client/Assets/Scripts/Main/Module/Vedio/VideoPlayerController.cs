using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace xicheng.Vedio
{   
    /*
    • ✅ 支持本地和网络视频播放
    • ✅ 提供播放控制接口（播放、暂停、停止）
    • ✅ 实现进度控制和获取
    • ✅ 支持音量调节
    • ✅ 提供播放状态回调
    • ✅ 异常处理和错误恢复*/
    
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoPlayerController :MonoBehaviour, IVideoPlayerController
    {
        private VideoPlayer _videoPlayer;
        private bool _isInitialized = false;
        private float _targetProgress = -1f;
        private Coroutine _progressUpdateCoroutine;

        [Header("播放器配置")]
        [SerializeField] private bool autoPlay; //自动播放
        [SerializeField] private bool loop; //循环播放
        [Range(0,1)]
        [SerializeField] private float defaultVolume = 1.0f; //默认音量

        [Header("性能优化")] 
        [SerializeField] private bool skipOnDrop = true; //是否允许在视频播放落后时跳帧追赶当前时间
        [SerializeField] private VideoAspectRatio aspectRatio = VideoAspectRatio.FitInside; //适配

        #region 状态

        public bool IsPlaying => _videoPlayer != null && _videoPlayer.isPlaying;
        public bool IsPaused => _videoPlayer != null && _videoPlayer.isPaused;
        public float Duration => _videoPlayer != null ? (float)_videoPlayer.length : 0f;

        #endregion

        #region 事件

        public event Action OnPlayStarted;
        public event Action OnPlayPaused;
        public event Action OnPlayStopped;
        public event Action OnPlayCompleted;
        public event Action<string> OnError;

        ///  <summary>
        ///  订阅视频播放器事件
        ///  </summary>
        private void SubscribeEvents()
        {
            _videoPlayer.started += OnVideoStarted;
            _videoPlayer.loopPointReached += OnVideoCompleted;
            _videoPlayer.errorReceived += OnVideoError;
            _videoPlayer.prepareCompleted += OnVideoPrepared;
        }

        ///  <summary>
        ///  取消订阅事件
        ///  </summary>
        private void UnsubscribeEvents()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.started -= OnVideoStarted;
                _videoPlayer.loopPointReached -= OnVideoCompleted;
                _videoPlayer.errorReceived -= OnVideoError;
                _videoPlayer.prepareCompleted -= OnVideoPrepared;
            }
        }

        private void OnVideoStarted(VideoPlayer vp) => OnPlayStarted?.Invoke();
        private void OnVideoCompleted(VideoPlayer vp) => OnPlayCompleted?.Invoke();
        private void OnVideoError(VideoPlayer vp, string message) => OnError?.Invoke(message);
        private void OnVideoPrepared(VideoPlayer vp) => Debug.Log("视频准备完成");

        #endregion

        void Start()
        {
            InitializeVideoPlayer();
        }

        /// <summary>
        /// 初始化视频播放器
        /// </summary>
        private void InitializeVideoPlayer()
        {
            try
            {
                _videoPlayer = GetComponent<VideoPlayer>();
                ConfigureVideoPlayer();
                SubscribeEvents();
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"VideoPlayer初始化失败: {ex.Message}");
                OnError?.Invoke($"初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 配置视频播放器参数
        /// </summary>
        private void ConfigureVideoPlayer()
        {
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = loop;
            _videoPlayer.skipOnDrop = skipOnDrop;
            _videoPlayer.aspectRatio = aspectRatio;
            _videoPlayer.SetDirectAudioVolume(0, defaultVolume);

            // 设置渲染模式为RenderTexture以获得最佳兼容性
            if (_videoPlayer.renderMode == VideoRenderMode.CameraFarPlane ||
                _videoPlayer.renderMode == VideoRenderMode.CameraNearPlane)
            {
                _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            }
        }

        /// <summary>
        /// 播放视频
        /// </summary>
        public void Play()
        {
            if (!_isInitialized)
            {
                OnError?.Invoke("播放器未初始化");
                return;
            }

            try
            {
                _videoPlayer.Play();
                StartProgressCoroutine();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"播放失败:   {ex.Message}");
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause()
        {
            if (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                _videoPlayer.Pause();
                StopProgressCoroutine();
                OnPlayPaused?.Invoke();
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.Stop();
                StopProgressCoroutine();
                OnPlayStopped?.Invoke();
            }
        }

        /// <summary>
        /// 设置播放进度
        /// </summary>
        /// <param name="progress">进度值(0-1)</param>
        public void SetProgress(float progress)
        {
            if (_videoPlayer == null) return;

            progress = Mathf.Clamp01(progress);
            _targetProgress = progress;

            if (_videoPlayer.isPrepared)
            {
                _videoPlayer.time = _videoPlayer.length * progress;
            }
        }

        /// <summary>
        /// 获取当前播放进度
        /// </summary>
        /// <returns>进度值(0-1)</returns>
        public float GetProgress()
        {
            if (_videoPlayer == null || _videoPlayer.length <= 0) return 0f;
            return (float)(_videoPlayer.time / _videoPlayer.length);
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="volume">音量值(0-1)</param>
        public void SetVolume(float volume)
        {
            if (_videoPlayer != null)
            {
                volume = Mathf.Clamp01(volume);
                _videoPlayer.SetDirectAudioVolume(0, volume);
            }
        }

        /// <summary>
        /// 获取当前音量
        /// </summary>
        /// <returns>音量值(0-1)</returns>
        public float GetVolume()
        {
            return _videoPlayer != null ? _videoPlayer.GetDirectAudioVolume(0) : 0f;
        }


        /// <summary>
        /// 设置本地视频文件
        /// </summary>
        /// <param name="videoClip">视频剪辑</param>
        public void SetVideoClip(VideoClip videoClip)
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.source = VideoSource.VideoClip;
                _videoPlayer.clip = videoClip;
                PrepareVideo();
            }
        }

        /// <summary>
        /// 设置网络视频URL
        /// </summary>
        /// <param name="url">视频URL</param>
        public void SetVideoURL(string url)
        {
            if (_videoPlayer != null && !string.IsNullOrEmpty(url))
            {
                _videoPlayer.source = VideoSource.Url;
                _videoPlayer.url = url;
                PrepareVideo();
            }
        }

        /// <summary>
        /// 准备视频播放
        /// </summary>
        private void PrepareVideo()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.Prepare();
            }
        }


        /// <summary>
        /// 启动进度更新协程
        /// </summary>
        private void StartProgressCoroutine()
        {
            StopProgressCoroutine();
            _progressUpdateCoroutine = StartCoroutine(UpdateProgressCoroutine());
        }

        /// <summary>
        /// 停止进度更新协程
        /// </summary>
        private void StopProgressCoroutine()
        {
            if (_progressUpdateCoroutine != null)
            {
                StopCoroutine(_progressUpdateCoroutine);
                _progressUpdateCoroutine = null;
            }
        }

        /// <summary>
        /// 进度更新协程
        /// </summary>
        private IEnumerator UpdateProgressCoroutine()
        {
            while (_videoPlayer != null && _videoPlayer.isPlaying)
            {
                // 处理设定的目标进度
                if (_targetProgress >= 0f && _videoPlayer.isPrepared)
                {
                    _videoPlayer.time = _videoPlayer.length * _targetProgress;
                    _targetProgress = -1f;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }


        /// <summary>
        /// 网络视频预加载策略
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator PreloadNetworkVideo(string url)
        {
            _videoPlayer.source = VideoSource.Url;
            _videoPlayer.url = url;
            _videoPlayer.Prepare();

            while (!_videoPlayer.isPrepared)
            {
                yield return null;
            }

            Debug.Log("网络视频预加载完成");
        }

        //播放速率调节在设置videoPlayer.playbackSpeed之前需要判断VideoPlayer.canSetPlaybackSpeed是否为True才可以进行设置:
        private void SetBackSpeed()
        {
            if (_videoPlayer.canSetPlaybackSpeed)
            {
                _videoPlayer.playbackSpeed = 2f;
            }
        }

        public void SetRenderMode(VideoRenderMode renderMode)
        {
            _videoPlayer.renderMode = renderMode;
        }

        public void SetRenderTexture()
        {
            
        }
    }
}