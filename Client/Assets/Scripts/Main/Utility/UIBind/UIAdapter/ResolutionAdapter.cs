using System;
using UnityEngine;
using UnityEngine.UI;

//使用方式：游戏初始化时设置。


/// <summary>
/// 分辨率适配
/// </summary>
[RequireComponent(typeof(CanvasScaler))]
public class ResolutionAdapter : MonoBehaviour
{
    public ResolutionAdaptSettings settings;

    private void Awake()
    {
        ApplyAdaptation();
    }

    private void ApplyAdaptation()
    {
        if (settings == null) 
        {
            Debug.LogError("分辨率setting未指定");
            return;
        }
        
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; //设置UI缩放模式。
        scaler.referenceResolution = settings.referenceResolution; //设置参考分辨率。

        float aspect = (float)Screen.width / Screen.height; //屏幕宽高比
        //设置屏幕匹配模式
        switch (settings.adaptMode)
        {
            case AdaptMode.Width:
                scaler.matchWidthOrHeight = 0;
                break;
            case AdaptMode.Height:
                scaler.matchWidthOrHeight = 1;
                break;
            case AdaptMode.Auto:
                scaler.matchWidthOrHeight = aspect >= settings.autoMatchThreshold ? 1 : 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
