using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Xicheng.Utility;

namespace Xicheng.Download
{
    public class DownloaderManager : MonoSingleton<DownloaderManager>
    {
        private Queue<DownloadTask> _taskQueue = new();
        private bool _isDownloading;
        private DownloadTask _currentTask;
        private readonly object _lock = new();

        private void Awake()
        {
            _taskQueue = new();
        }

        public void AddTask(DownloadTask task)
        {
            lock (_lock)
            {
                _taskQueue.Enqueue(task);
                if (!_isDownloading)
                    StartCoroutine(DownloadNext());
            }
        }

        /// <summary>
        /// 暂停当前任务下载
        /// </summary>
        public void PauseCurrentTask()
        {
            if (_currentTask is { currentRequest: not null })
            {
                _currentTask.IsPaused = true;
                _currentTask.currentRequest.Abort();
                // 记录当前下载位置
                _currentTask.resumeOffset = new FileInfo(_currentTask.SavePath + ".tmp").Length;
            }
        }

        /// <summary>
        /// 恢复当前任务下载
        /// </summary>
        public void ResumeCurrentTask()
        {
            if (_currentTask is { IsPaused: true })
            {
                _currentTask.IsPaused = false;
                StartCoroutine(DownloadWithResumeInternal(_currentTask));
            }
        }

        private IEnumerator DownloadNext()
        {
            _isDownloading = true;
            while (_taskQueue.Count > 0)
            {
                _currentTask = _taskQueue.Dequeue();
                yield return StartCoroutine(DownloadWithResumeInternal(_currentTask));
            }

            _isDownloading = false;
        }

        private IEnumerator DownloadWithResumeInternal(DownloadTask task)
        {
            try
            {
                string tempPath = task.SavePath + ".tmp";
                long existing = File.Exists(tempPath) ? new FileInfo(tempPath).Length : 0;

                UnityWebRequest req = UnityWebRequest.Get(task.Url);
                req.SetRequestHeader("Range", "bytes=" + existing + "-");
                req.downloadHandler = new DownloadHandlerFile(tempPath, true);

                task.currentRequest = req;
                task.IsPaused = false;

                var op = req.SendWebRequest();

                while (!op.isDone)
                {
                    if (task.IsPaused)
                        yield break; // 被手动中止
                    task.DownloadedBytes = existing + (long)req.downloadedBytes;

                    task.OnProgress?.Invoke(req.downloadProgress);
                    task.OnSizeUpdate?.Invoke(task.DownloadedBytes, task.TotalBytes);
                    yield return null;
                }

                if (req.result == UnityWebRequest.Result.Success || req.responseCode == 206)
                {
                    MergeTempToTarget(tempPath, task.SavePath);
                    task.OnComplete?.Invoke();
                }
                else if (req.result == UnityWebRequest.Result.ConnectionError)
                {
                    task.OnError?.Invoke("网络连接失败");
                }
                else if (req.responseCode == 404)
                {
                    task.OnError?.Invoke("资源不存在");
                }
                else
                {
                    task.OnError?.Invoke($"错误代码:{req.responseCode}");
                }
            }
            finally
            {
                task.currentRequest?.Dispose();
            }
        }

        void MergeTempToTarget(string tempPath, string finalPath)
        {
            try
            {
                // 确保目录存在
                string dir = Path.GetDirectoryName(finalPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // 覆盖已存在的文件
                if (File.Exists(finalPath))
                    File.Delete(finalPath);

                File.Move(tempPath, finalPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"文件移动失败: {e.Message}");
                // 保留临时文件供恢复
                throw;
            }
        }
    }
}