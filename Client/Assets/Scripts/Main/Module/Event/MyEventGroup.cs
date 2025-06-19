using System;
using System.Collections.Generic;
using Main.Module.Log;


namespace xicheng.module.events
{
    //事件分组：好处就是可以统一管理，统一销毁，防止事件泄露
    //TODO的地方还没开发完成。
    public class MyEventGroup
    {
        private readonly Dictionary<int,List<Action<IEventParam>>> _cachedListener = new Dictionary<int, List<Action<IEventParam>>>();
        /// <summary>
        /// 添加一个监听
        /// </summary>
        public void AddListener(string eventKey,Action<IEventParam> listener)
        {
            int eventId = eventKey.GetHashCode();
            if (!_cachedListener.ContainsKey(eventId))
                _cachedListener.Add(eventId, new List<Action<IEventParam>>());
            
            if (!_cachedListener[eventId].Contains(listener))
            {
                _cachedListener[eventId].Add(listener);
                //ModuleEvent.AddListener(eventKey, listener); TODO:
            }
            else
            {
                Log.Warning($"Event listener is exist : {eventKey}");
            }
        }

        /// <summary>
        /// 移除所有缓存的监听
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (var pair in _cachedListener)
            {
                var eventId = pair.Key;
                foreach (var listener in pair.Value)
                {
                   // ModuleEvent.RemoveListener(eventId,listener); TODO:
                }
                pair.Value.Clear();
            }
            _cachedListener.Clear();
        }
    }
}