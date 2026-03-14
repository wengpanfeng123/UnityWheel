using UnityEngine;

namespace Xicheng.Download
{
    public class TestDownload
    {
        void Start()
        {
            var task = RefPool.Acquire<DownloadTask>();

            task.Url = "https://www.youraddress.com/data.zip";
            task.SavePath = Application.persistentDataPath + "/data.zip";
            task.OnProgress = (p) =>
            {
                Debug.Log($"进度:  {p * 100f:F2}%");
            };
            task.OnSizeUpdate = (cur, total) => Debug.Log($"下载  {cur}/{total}");
            task.OnComplete = () => Debug.Log("下载完成！");
     

            DownloaderManager.Inst.AddTask(task);
        }
    }
}