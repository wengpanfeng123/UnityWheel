using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AdaptMode
{
    [Header("宽适配")]
    Width,
    [Header("高适配")]
    Height,
    [Header("自动适配")]
    Auto
}

[CreateAssetMenu(menuName = "UI/ResolutionAdaptSettings")]
public class ResolutionAdaptSettings : ScriptableObject
{
    [Header("屏幕分辨率设置")]
    public Vector2 referenceResolution = new(1080, 1920);
    [Header("适配模式")]
    public AdaptMode adaptMode = AdaptMode.Auto;
    [Header("宽高比阈值")]
    [Range(1.6f, 2.0f)] public float autoMatchThreshold = 1.8f;
}
