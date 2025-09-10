using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Xicheng.UIAdapter
{
    /// <summary>
    /// 适配模式
    /// </summary>
    public enum AdaptMode
    {
        [LabelText("宽适配")]Width,
        [LabelText("高适配")]Height,
        [LabelText("自动适配")]Auto,
    }

    public class UISafeAreaAdapter : MonoBehaviour
    {
        public RectTransform rectTransform;
        public CanvasScaler scaler;
        [Header("适配模式")]public AdaptMode adaptMode; //
        [ShowIf("IsAdaptMode")]
        [Range(1.6f, 2.0f)] public float autoMatchThreshold = 1.8f; //自动适配阈值

        [Header("是否竖屏")] public bool isPortrait; //是否竖屏
        [Header("横屏分辨率")] public Vector2 landSpaceResolution = new(1920, 1080);
        [Header("竖屏分辨率")] public Vector2 portraitResolution = new(1080, 1920);

        private bool IsAdaptMode=> adaptMode == AdaptMode.Auto;

        public void Start()
        {
            SafeAreaAdapter(AdaptMode.Width, true);
        }

        public void SafeAreaAdapter(AdaptMode mode, bool portraitScreen)
        {
            adaptMode = mode;
            isPortrait = portraitScreen;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = isPortrait ? portraitResolution : landSpaceResolution;

            string screen = portraitScreen ? "竖屏" : "横屏";   
            ULog.InfoYellow($"分辨率:{scaler.referenceResolution} {screen} 适配:{mode}");
            float aspect = (float)Screen.width / Screen.height;
            switch (adaptMode)
            {
                case AdaptMode.Width:
                    scaler.matchWidthOrHeight = 0;
                    break;
                case AdaptMode.Height:
                    scaler.matchWidthOrHeight = 1;
                    break;
                case AdaptMode.Auto:
                    scaler.matchWidthOrHeight = aspect >= autoMatchThreshold ? 1 : 0;
                    break;
            }
            
            ApplySafeArea();
        }

        //计算安全区域
        private void ApplySafeArea()
        {
            Rect safe = Screen.safeArea;
            Vector2 min = safe.position;
            Vector2 max = safe.position + safe.size;

            min.x /= Screen.width;
            min.y /= Screen.height;
            max.x /= Screen.width;
            max.y /= Screen.height;

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
        }
    }
}