using System;
using System.Collections.Generic;
using xicheng.log.Log;
using UnityEngine;using UnityEngine.UIElements;

public interface IEventParam
{
}

public interface IEvent : IReference
{
    public void Acquire();
}

public abstract class EventBase:IReference
{
    public virtual void Clear() {}

    public void Acquire<T>() where T : class, IReference, new()
    {
        RefPool.Acquire<T>();
    }
}

/*文档1
使用 LinkedList 存储监听器，插入/删除效率高，但遍历可能略慢。
文档2
委托合并（+=/-=）底层为链表，性能与文档1接近，但代码更简洁。
无哈希计算开销（直接使用 int 事件ID）
 *静态成员默认共享，需手动处理线程同步（如 lock）
 * 静态类成员（静态字段、方法等）：

存储在 全局数据区（高频堆，Loader Heap） 中。
这部分内存由 .NET 运行时在程序启动时直接分配，独立于垃圾回收（GC）机制。
生命周期与应用程序域（AppDomain）一致，程序结束时自动释放。
静态构造函数：在类首次被访问时执行一次，初始化静态成员的数据。
 关键特性：
  不可被垃圾回收：静态成员一旦初始化，会一直占用内存直到程序结束。
 共享性：所有线程和实例共享同一份静态数据。

 适用静态类的场景：
    工具方法（如 Math、File）。
    全局配置（如 AppSettings）。
    扩展方法容器(如：扩展Transform的方法)
 */
namespace xicheng.events
{
    public static class GameEvent
    {
        public delegate void OnEventAction(IEvent param);

        private static bool _isInit;
        private static Dictionary<int, OnEventAction> _eventActionDic;
        // 添加锁对象
        private static readonly object _lock = new object();
 
        //静态构造函数：在类首次被访问时执行一次，初始化静态成员的数据。
        static GameEvent()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _isInit = true;
            _eventActionDic = new();
        }

        public static void AddListener<T>(OnEventAction onEvent) where T : IEvent
        {
            int eventKey = typeof(T).GetHashCode();
            Add(eventKey, onEvent);
        }

        public static void AddListener(Type type, OnEventAction onEvent)
        {
            int eventKey = type.GetHashCode();
            Add(eventKey, onEvent);
        }

        private static void Add(int eventKey,OnEventAction onEvent)
        {
            if(!_isInit)
                return;
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

        public static void RemoveListener<T>(OnEventAction onEvent) where T : IEvent
        {
            int eventKey = typeof(T).GetHashCode();
            if (_eventActionDic.ContainsKey(eventKey))
            {
                _eventActionDic[eventKey] -= onEvent;
            }
        }

        public static void RemoveListener(Type type, OnEventAction onEvent)
        {
            int eventKey = type.GetHashCode();
            if (_eventActionDic.ContainsKey(eventKey))
            {
                _eventActionDic[eventKey] -= onEvent;
            }
        }

        public static void Send<T>(T param) where T : IEvent
        {
            int eventKey = typeof(T).GetHashCode();
            if (_eventActionDic.TryGetValue(eventKey, out var action) && action!=null)
            {
                action.Invoke(param);
                return;
            }
            ULog.Info($"[Send] 事件<{typeof(T).Name}>不存在");
        }

        public static void OnRelease()
        {
            _isInit = false;
            lock (_lock)
            {
                if (_eventActionDic != null)
                {
                    _eventActionDic.Clear();
                    _eventActionDic = null;
                }
            }
        }
    }
}