using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class FormatUtil
{
    // 用于数字缩写的后缀列表
    private static readonly string[] Suffixes = { "", "k", "M", "B", "T" };
    private static readonly StringBuilder TimeBuilder = new();
    
    /// <summary>
    /// 数字格式化（千、百万...）
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string FormatNumber(float number)
    {
        // 处理小于1000的数字，无需缩写
        if (number < 1000)
            return number.ToString("0");
        
        // 确定数字的数量级
        int magnitude = Mathf.FloorToInt(Mathf.Log10(number) / 3);
        
        // 计算缩写后的数值
        float scaled = number / Mathf.Pow(10, magnitude * 3);
        
        // 生成格式化字符串
        string format = scaled >= 100 ? "0" : (scaled >= 10 ? "0.0" : "0.00");
        
        // 返回带后缀的格式化数字
        return scaled.ToString(format) + Suffixes[magnitude];
    }
    

    /// <summary>
    /// 格式化倒计时
    /// </summary>
    /// <param name="totalSeconds"></param>
    public static string FormatCountdown(long totalSeconds)
    {
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        TimeBuilder.Clear();
        if (hours > 0)
        {
            TimeBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
        else
        {
            TimeBuilder.AppendFormat("{0:D2}:{1:D2}", minutes, seconds);
        }
        return TimeBuilder.ToString();
    }
    
    /// <summary>
    /// 根据时间跨度返回对应的显示格式
    /// 超过1天：显示 天数d 小时数h
    /// 不足1天：显示 时:分:秒（带前导零）
    /// </summary>
    /// <returns>格式化后的字符串</returns>
    public static string FormatCountdown2(long totalSeconds)
    {
        // 判断是否超过1天（总天数>=1）
        if (totalSeconds >= 86400)
        {
            int days = Mathf.FloorToInt(totalSeconds / 86400f);
            // 计算剩余秒数（总秒数减去天数对应的秒数）
            long remainingSeconds = totalSeconds % 86400;
            int hours = Mathf.FloorToInt(remainingSeconds / 3600f);
            return $"{days}d {hours}h";
        }
        // 不足1天，格式化为 时:分:秒（带前导零）
        return FormatCountdown((int)totalSeconds);
        //return timeSpan.ToString("hh:mm:ss");
    }
    
    /// <summary>
    /// 超过1天：天数d 小时数h
    /// 小于1天大于1小时：小时数h 分数m
    /// 小于1小时：分数m 描述s
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static string FormatCountdown3(TimeSpan timeSpan,bool needSpace =true)
    {
        int days = (int)timeSpan.TotalDays;
        // 判断是否超过1天（总天数>=1）
        if (days >= 1)
        {
            return needSpace ? $"{days}d {timeSpan.Hours}h" : $"{days}d{timeSpan.Hours}h";
        }

        string format = null;
        if (needSpace)
        {
            format = timeSpan.Hours > 0 ? 
                $"{timeSpan.Hours}h {timeSpan.Minutes}m" : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            format = timeSpan.Hours > 0 ? 
                $"{timeSpan.Hours}h{timeSpan.Minutes}m" : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        return format;
    }

}
