using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//挂载到UIRoot-->SafeArea这个物体上。

/// <summary>
/// 安全区域适配
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaAdapter : MonoBehaviour
{
    void Start()
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;
        Vector2 min = safe.position;
        Vector2 max = safe.position + safe.size;
        min.x /= Screen.width;
        min.y /= Screen.height;
        max.x /= Screen.width;
        max.y /= Screen.height;
        
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = min;
        rt.anchorMax = max;
    }
}
