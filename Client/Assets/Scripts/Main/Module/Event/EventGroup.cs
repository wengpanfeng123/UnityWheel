using System;
using System.Collections.Generic;


namespace Xicheng.module.events
{
    //事件分组：好处就是可以统一管理，统一销毁，防止事件泄露

    public delegate void OnEventDelegate(IEvent param);

    public class EventGroup : IReference
    {
        private readonly object _lock = new();
        private Dictionary<int, OnEventDelegate> _eventActionDic;

        public void Subscribe<T>(OnEventDelegate onEvent) where T : IEvent
        {
            int eventKey = typeof(T).GetHashCode();
            lock (_lock)
            {
                if (_eventActionDic.ContainsKey(eventKey))
                {
                    _eventActionDic[eventKey] += onEvent;
                }
                else
                {
                    _eventActionDic[eventKey] = onEvent;
                }
            }
        }


        /// <summary>
        /// 事件通知
        /// </summary>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        public void Notify<T>(T param) where T : IEvent
        {
            int eventKey = typeof(T).GetHashCode();
            lock (_lock)
            {
                if (_eventActionDic.TryGetValue(eventKey, out var action) && action != null)
                {
                    action.Invoke(param);
                    return;
                }
            }

            ULog.Error($"[Send] 事件<{typeof(T).Name}>不存在");
        }


        public void Subscribe(Type type, OnEventDelegate onEvent)
        {
            int eventKey = type.GetHashCode();
            lock (_lock)
            {
                if (_eventActionDic.ContainsKey(eventKey))
                {
                    _eventActionDic[eventKey] += onEvent;
                }
                else
                {
                    _eventActionDic[eventKey] = onEvent;
                }
            }
        }

        public void UnSubscribe(Type type, OnEventDelegate onEvent)
        {
            int eventKey = type.GetHashCode();
            lock (_lock)
            {
                if (_eventActionDic.ContainsKey(eventKey))
                {
                    _eventActionDic[eventKey] -= onEvent;
                }
            }
        }

        /// <summary>
        /// 移除所有缓存的监听
        /// </summary>
        public void ReleaseAll()
        {
            lock (_lock)
            {
                _eventActionDic?.Clear();
                _eventActionDic = null;
            }
        }

        /// <summary>
        /// 清理所有事件
        /// </summary>
        public void Clear()
        {
            ReleaseAll();
            RefPool.Release(this);
        }

        /// <summary>
        /// 获得一个事件组实例
        /// </summary>
        public static EventGroup Acquire()
        {
            return RefPool.Acquire<EventGroup>();
        }
    }
}