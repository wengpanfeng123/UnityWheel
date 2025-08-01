using UnityEngine;

namespace Xicheng.mic
{
    public class TestMic:MonoBehaviour
    {
        void Start()
        {
            //初始化权限(推荐在游戏启动时检查一次)
            MicManager.Inst.CheckMicrophonePermission();
            //订阅事件
            MicManager.Inst.OnRecordingStateChanged += (isRecording) =>
            {
                Debug.Log("状态："+isRecording);
            };
            
            MicManager.Inst.OnVolumeChanged += (data) =>
            {
                Debug.Log("");
            };
            MicManager.Inst.OnFileSaved += (data) =>
            {
                Debug.Log("录制文件保存完成"+data);
            };
            MicManager.Inst.OnError += (data) =>
            {
                Debug.Log("----mic error : " + data);
            };
        }

        public void StartRecording()
        {
            MicManager.Inst.StartRecording();
        }

        public void StopRecord()
        {
            MicManager.Inst.StopRecording();
        }
    }
}