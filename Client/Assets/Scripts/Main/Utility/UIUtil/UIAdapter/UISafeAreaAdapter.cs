using System;
using kcp2k;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xicheng.Screens;

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

    public class UISafeAreaAdapter : UIBehaviour
    {
        public RectTransform rectTransform;
        public CanvasScaler scaler;
        [Header("适配模式")]public AdaptMode adaptMode; //
        [ShowIf("IsAdaptMode")]
        [Range(1.6f, 2.0f)] public float autoMatchThreshold = 1.8f; //自动适配阈值

        [Header("横屏分辨率")] public Vector2 landSpaceResolution = new(1920, 1080);
        [Header("竖屏分辨率")] public Vector2 portraitResolution = new(1080, 1920);

        private bool IsAdaptMode=> adaptMode == AdaptMode.Auto;
        private float _screenWidth, _screenHeight; //缓存上次
        private bool _isQuitApp; 

        /// <summary>
        /// 安全区域适配
        /// </summary>
        /// <param name="portraitScreen">竖屏</param>
        private void SafeAreaAdapter(bool portraitScreen)
        {
            adaptMode = portraitScreen ? AdaptMode.Width : AdaptMode.Height;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = portraitScreen ? portraitResolution : landSpaceResolution;

            string screen = portraitScreen ? "竖屏" : "横屏";   
            ULog.InfoYellow($"分辨率:{scaler.referenceResolution} {screen} 适配:{adaptMode}");
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
            Vector2 min = safe.position; //左下角像素坐标。
            Vector2 max = safe.position + safe.size; //右上角像素坐标。

            //坐标转比例[0,1](因为RectTransform的anchorMin/anchorMax是相对父容器的比例)
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
            ULog.InfoYellow($"[SafeArea]anchorMin:{min} anchorMax:{max}");
        }

        /// <summary>
        /// 当关联的 RectTransform（矩形变换组件）的尺寸发生变化时，会调用此回调。
        /// 它总是在 Awake、OnEnable 或 Start 之前被调用。该调用也会发送到所有子 RectTransform，
        /// 无论它们的尺寸是否发生变化（这取决于它们的锚定方式）。
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            if (_isQuitApp)
                return;
            if (!Mathf.Approximately(_screenWidth, Screen.width) || !Mathf.Approximately(_screenHeight, Screen.height))
            {
                _screenWidth = Screen.width;
                _screenHeight = Screen.height;
                ScreenManager.Inst.OnRectTransformDimensionsChange();
                bool portraitScreen = ScreenManager.Inst.IsPortrait;

                SafeAreaAdapter(portraitScreen);
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitApp =true;
        }
    }
}