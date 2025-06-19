using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr : MonoBehaviour
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
            timer.callback?.Invoke();
            
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
        // 注意：PriorityQueue没有直接Remove方法，需要自定义实现
        // 这里简化处理，实际项目中需要扩展PriorityQueue
        return timers.Remove(timer);
    }
}
