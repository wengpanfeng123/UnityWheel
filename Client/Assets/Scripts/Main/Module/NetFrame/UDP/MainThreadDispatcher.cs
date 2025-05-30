using System;
using System.Collections.Generic;
using UnityEngine;

namespace xicheng.udp
{
    //主线程调度器（解决跨线程问题）
    public class MainThreadDispatcher
    {
        private static readonly Queue<Action> _executionQueue = new();

        public static void Enqueue(Action action) 
        {
            lock (_executionQueue) 
            {
                _executionQueue.Enqueue(action);
            }
        }
        
        public static void Tick()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }
    }
}