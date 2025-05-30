using System;
using System.Collections.Generic;
using xicheng.common;

public delegate void CallBack();

public delegate void CallBack<T>(T arg1);

public delegate void CallBack<T, U>(T arg1, U arg2);

public delegate void CallBack<T, U, V>(T arg1, U arg2, V arg3);

public delegate void CallBack<T, U, V, X>(T arg1, U arg2, V arg3, X arg4);

public class EventMgr : MonoSingleton<EventMgr>
{
    private Dictionary<int, Delegate> m_EventDic;

    public override void Init()
    {
        base.Init();
        m_EventDic = new Dictionary<int, Delegate>();
    }

    public void AddListener(string eType, CallBack handler)
    {
        int hashCode = OnListenerAdding(eType, handler);
        m_EventDic[hashCode] = (CallBack) m_EventDic[hashCode] + handler;
    }

    public void AddListener<T>(string eType, CallBack<T> handler)
    {
        int hashCode = OnListenerAdding(eType, handler);
        m_EventDic[hashCode] = (CallBack<T>) m_EventDic[hashCode] + handler;
    }

    public void AddListener<T, U>(string eType, CallBack<T, U> handler)
    {
        int hashCode = OnListenerAdding(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U>) m_EventDic[hashCode] + handler;
    }

    public void AddListener<T, U, V>(string eType, CallBack<T, U, V> handler)
    {
        int hashCode = OnListenerAdding(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U, V>) m_EventDic[hashCode] + handler;
    }

    public void AddListener<T, U, V, X>(string eType, CallBack<T, U, V, X> handler)
    {
        int hashCode = OnListenerAdding(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U, V, X>) m_EventDic[hashCode] + handler;
    }

    private int OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        int hashCode = eventType.GetHashCode();
        m_EventDic.TryAdd(hashCode, null);

        Delegate d = m_EventDic[hashCode];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            //“正在尝试为事件类型{0}添加签名不一致的侦听器。当前侦听器的类型为{1}，正在添加的侦听器的类型为{2}”，
            throw new Exception(string.Format(
                "Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}",
                eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }

        return hashCode;
    }
    public void RemoveListener(EEventType eType, CallBack handler)
    {
        int hashCode = eType.GetHashCode();
        RemoveParamCheck(eType, handler);
        m_EventDic[hashCode] = (CallBack) m_EventDic[hashCode] - handler;
        OnListenerRemoved(eType);
    }
    public void RemoveListener<T>(EEventType eType, CallBack<T> handler)
    {
        int hashCode = eType.GetHashCode();
        RemoveParamCheck(eType, handler);
        m_EventDic[hashCode] = (CallBack<T>) m_EventDic[hashCode] - handler;
        OnListenerRemoved(eType);
    }

    public void RemoveListener<T, U>(EEventType eType, CallBack<T, U> handler)
    {
        int hashCode = eType.GetHashCode();
        RemoveParamCheck(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U>) m_EventDic[hashCode] - handler;
        OnListenerRemoved(eType);
    }

    public void RemoveListener<T, U, V>(EEventType eType, CallBack<T, U, V> handler)
    {
        int hashCode = eType.GetHashCode();
        RemoveParamCheck(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U, V>) m_EventDic[hashCode] - handler;
        OnListenerRemoved(eType);
    }

    public void RemoveListener<T, U, V, X>(EEventType eType, CallBack<T, U, V, X> handler)
    {
        int hashCode = eType.GetHashCode();
        RemoveParamCheck(eType, handler);
        m_EventDic[hashCode] = (CallBack<T, U, V, X>) m_EventDic[hashCode] - handler;
        OnListenerRemoved(eType);
    }

    //事件移除时，参数检测
    private void RemoveParamCheck(EEventType eType, Delegate handler)
    {
        int hashCode = eType.GetHashCode();
        if (!m_EventDic.ContainsKey(hashCode))
        {
            throw new Exception($"RemoveListener Error,Could‘t Find GameEventType {eType}");
        }

        Delegate d = m_EventDic[hashCode];
        if (d != null && d.GetType() != handler.GetType())
        {
            //“正在尝试为事件类型{0}添加签名不一致的侦听器。当前侦听器的类型为{1}，正在添加的侦听器的类型为{2}”，
            throw new Exception(
                $"Attempting to add listener with inconsistent signature for event type {eType}. Current listeners have type {d.GetType().Name} and listener being added has type {handler.GetType().Name}");
        }
    }

    private void OnListenerRemoved(EEventType eType)
    {
        int hashCode = eType.GetHashCode();
        if (m_EventDic[hashCode] == null)
            m_EventDic.Remove(hashCode);
    }

    public void Dispatch(EEventType eType)
    {
        int hashCode = eType.GetHashCode();
        DispatchParamCheck(eType);
        if (m_EventDic.TryGetValue(hashCode, out Delegate handler))
        {
            CallBack callback = handler as CallBack;
            callback?.Invoke();
        }
    }

    public void Dispatch<T>(EEventType eType, T arg0)
    {
        int hashCode = eType.GetHashCode();
        DispatchParamCheck(eType);
        if (m_EventDic.TryGetValue(hashCode, out Delegate handler))
        {
            CallBack<T> callback = handler as CallBack<T>;
            callback?.Invoke(arg0);
        }
    }

    public void Dispatch<T, U>(EEventType eType, T arg0, U arg1)
    {
        int hashCode = eType.GetHashCode();
        DispatchParamCheck(eType);
        if (m_EventDic.TryGetValue(hashCode, out Delegate handler))
        {
            CallBack<T, U> callback = handler as CallBack<T, U>;
            callback?.Invoke(arg0, arg1);
        }
    }

    public void Dispatch<T, U, V>(EEventType eType, T arg0, U arg1, V arg2)
    {
        int hashCode = eType.GetHashCode();
        DispatchParamCheck(eType);
        if (m_EventDic.TryGetValue(hashCode, out Delegate handler))
        {
            CallBack<T, U, V> callback = handler as CallBack<T, U, V>;
            callback?.Invoke(arg0, arg1, arg2);
        }
    }

    public void Dispatch<T, U, V, X>(EEventType eType, T arg0, U arg1, V arg2, X arg3)
    {
        int hashCode = eType.GetHashCode();
        DispatchParamCheck(eType);
        if (m_EventDic.TryGetValue(hashCode, out Delegate handler))
        {
            CallBack<T, U, V, X> callback = handler as CallBack<T, U, V, X>;
            callback?.Invoke(arg0, arg1, arg2, arg3);
        }
    }

    private void DispatchParamCheck(EEventType eType)
    {        
        int hashCode = eType.GetHashCode();
        if (!m_EventDic.ContainsKey(hashCode))
        {
            throw new Exception($"RemoveListener Error,Could‘t Find GameEventType {eType}");
        }
    }
}