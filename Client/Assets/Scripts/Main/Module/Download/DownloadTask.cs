using System;
using UnityEngine.Networking;

namespace Xicheng.Download
{
    //支持暂停 / 恢复 / 状态查询
    public class DownloadTask : IReference
    {
        public string Url;
        /// <summary> 保存路径</summary>
        public string SavePath;
        /// <summary> 下载进度回调 </summary>
        public Action<float> OnProgress;
        /// <summary> 大小更新 </summary>
        public Action<long, long> OnSizeUpdate;
        /// <summary> 下载完成回调</summary>
        public Action OnComplete;
        /// <summary>报错回调</summary>
        public Action<string> OnError;

        /// <summary>总字节数 </summary>
        public long TotalBytes;
        /// <summary>已下载字节数 </summary>
        public long DownloadedBytes;
        /// <summary>下载进度 </summary>
        public float Progress;
        /// <summary>下载是否被暂停</summary>
        public bool IsPaused;
        public UnityWebRequest currentRequest;
        public long resumeOffset;

        public void Clear()
        {
            currentRequest.Dispose();
            currentRequest = null;
        }
    }
    
    // 任务状态枚举
    public enum DownloadStatus
    {
        Pending, 
        Downloading, 
        Paused,
        Completed,
        Failed
    }
}