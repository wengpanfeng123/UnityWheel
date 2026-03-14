using System;
using UnityEngine;
 

// 定时器类
public class Timer : IComparable<Timer>
{
    public float nextTriggerTime;  // 下次触发时间
    public float interval;         // 触发间隔（循环定时器）
    public Action callback;        // 回调函数
    public bool isLooping;         // 是否循环

    public Timer(float delay, float interval, Action callback, bool isLooping = false)
    {
        if (delay < 0)
            throw new ArgumentException("Delay cannot be negative.", nameof(delay));
        if (interval < 0)
            throw new ArgumentException("Interval cannot be negative.", nameof(interval));
        if (callback == null)
            throw new ArgumentNullException(nameof(callback), "Callback cannot be null.");

        this.nextTriggerTime = Time.time + delay;
        this.interval = interval;
        this.callback = callback;
        this.isLooping = isLooping;
    }

    // 实现IComparable接口，用于优先队列排序
    public int CompareTo(Timer other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other), "Cannot compare to null.");

        return nextTriggerTime.CompareTo(other.nextTriggerTime);
    }
}

// 定时器管理器
public class TimerManager : MonoBehaviour
{
    // 优先队列（最小堆）
    private PriorityQueue<Timer> timers = new PriorityQueue<Timer>();

    void Update()
    {
        float currentTime = Time.time;

        // 只需检查堆顶元素（最早触发的定时器）
        while (timers.Count > 0 && timers.Peek().nextTriggerTime <= currentTime)
        {
            Timer timer = timers.Dequeue();
            try
            {
                timer.callback?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error invoking timer callback: {ex.Message}");
            }

            // 如果是循环定时器，重新计算下次触发时间并入队
            if (timer.isLooping)
            {
                timer.nextTriggerTime = currentTime + timer.interval;
                timers.Enqueue(timer);
            }
        }
    }

    // 添加定时器
    public Timer AddTimer(float delay, Action callback)
    {
        Timer timer = new Timer(delay, 0, callback, false);
        timers.Enqueue(timer);
        return timer;
    }

    // 添加循环定时器
    public Timer AddLoopTimer(float interval, Action callback)
    {
        Timer timer = new Timer(interval, interval, callback, true);
        timers.Enqueue(timer);
        return timer;
    }

    // 移除定时器
    public bool RemoveTimer(Timer timer)
    {
        if (timer == null)
            throw new ArgumentNullException(nameof(timer), "Timer cannot be null.");

        return timers.Remove(timer);
    }
}
