using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Xicheng.Vedio
{
    public class VideoPlayerExample : MonoBehaviour
    {
        [SerializeField] private VideoPlayerController videoController;
        [SerializeField] private string networkVideoURL = "视频地址";

        public RawImage videoImage;
        private void Start()
        {
            videoController.OnPlayStarted += () => Debug.Log("播放开始");
            videoController.OnPlayCompleted += () => Debug.Log("播放完成");
            videoController.OnError += (error) => Debug.LogError($"播放错误:  {error}");
            videoController.SetVideoURL(networkVideoURL);
            
           // RenderTexture rt = new RenderTexture(1360, 768, 24)
            
            
        }

        [Button("Video-Play")]
        public void OnPlayButtonClick() => videoController.Play();
        [Button("Video-Pause")]
        public void OnPauseButtonClick() => videoController.Pause();
        [Button("Video-Stop")]
        public void OnStopButtonClick() => videoController.Stop();

        public void OnProgressSliderChanged(float value)
        {
            videoController.SetProgress(value);
        }
    }
}