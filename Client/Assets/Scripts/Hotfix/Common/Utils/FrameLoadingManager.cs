using System;
using System.Collections.Generic;
using Xicheng.Utility;

namespace HsJam
{
/*添加一个简单的加载任务
    FrameLoadingManager.Instance.AddTask(() => {
        // 这里执行具体的加载逻辑
        // 返回true表示任务完成，false表示需要下一帧继续
        return true;
    }, (result) => {
        // 任务完成后的回调
        Debug.Log("任务完成");
    });
    */ 
    /// <summary>
    /// 分帧加载管理器
    /// 用于将大量加载任务分散到多帧执行，避免单帧卡顿
    /// </summary>
    public class FrameLoadingManager : MonoSingleton<FrameLoadingManager>
    {
        // 加载任务队列
        private readonly Queue<FrameLoadingTask> _loadingTasks = new();

        // 当前正在执行的任务
        private FrameLoadingTask _currentTask;

        // 每帧最大处理的任务数量
        public int maxTasksPerFrame = 5;

        // 每帧最大处理时间(毫秒)，防止单帧处理时间过长
        public int maxFrameTime = 5;

        private void Update()
        {
            ProcessLoadingTasks();
        }

        /// <summary>
        /// 处理加载任务
        /// </summary>
        private void ProcessLoadingTasks()
        {
            if (_loadingTasks.Count == 0 && _currentTask == null)
                return;

            int processedCount = 0;
            long startTime = DateTime.Now.Ticks / 10000; // 毫秒数

            // 处理任务直到达到每帧上限或超时
            while (processedCount < maxTasksPerFrame)
            {
                // 检查是否超时
                long currentTime = DateTime.Now.Ticks / 10000;
                if (currentTime - startTime > maxFrameTime)
                    break;

                // 获取下一个任务
                if (_currentTask == null && _loadingTasks.Count > 0)
                {
                    _currentTask = _loadingTasks.Dequeue();
                }

                if (_currentTask == null)
                    break;

                // 执行任务
                bool isComplete = _currentTask.Execute();

                if (isComplete)
                {
                    _currentTask.OnComplete?.Invoke(_currentTask.Result);
                    _currentTask = null;
                    processedCount++;
                }
                else
                {
                    // 如果任务未完成但需要暂停，留到下一帧继续
                    break;
                }
            }
        }

        /// <summary>
        /// 添加加载任务
        /// </summary>
        /// <param name="task">加载任务</param>
        private void AddTask(FrameLoadingTask task)
        {
            _loadingTasks.Enqueue(task);
        }

        /// <summary>
        /// 添加加载任务
        /// </summary>
        /// <param name="executeFunc">执行函数</param>
        /// <param name="onComplete">完成回调</param>
        /// <param name="priority">优先级(0-10，10最高)</param>
        public void AddTask(Func<bool> executeFunc, Action<object> onComplete = null, int priority = 5)
        {
            var task = new FrameLoadingTask
            {
                Execute = executeFunc,
                OnComplete = onComplete,
                Priority = priority
            };
            AddTask(task);
        }

        /// <summary>
        /// 清空所有任务
        /// </summary>
        public void ClearAllTasks()
        {
            _loadingTasks.Clear();
            _currentTask = null;
        }

        /// <summary>
        /// 获取当前任务数量
        /// </summary>
        public int GetTaskCount()
        {
            return _loadingTasks.Count + (_currentTask != null ? 1 : 0);
        }
    }

    /// <summary>
    /// 「分帧加载」任务类
    /// </summary>
    public class FrameLoadingTask
    {
        /// <summary>
        /// 执行任务的函数
        /// 返回值：true表示任务完成，false表示需要下一帧继续执行
        /// </summary>
        public Func<bool> Execute { get; set; }

        /// <summary>
        /// 任务完成回调
        /// </summary>
        public Action<object> OnComplete { get; set; }

        /// <summary>
        /// 任务优先级(0-10)
        /// </summary>
        public int Priority { get; set; } = 5;

        /// <summary>
        /// 任务执行结果
        /// </summary>
        public object Result { get; set; }
    }
}
