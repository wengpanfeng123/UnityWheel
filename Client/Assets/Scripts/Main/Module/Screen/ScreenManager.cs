/*************************************************************************
 * Copyright  xicheng. All rights reserved.
 *------------------------------------------------------------------------
 * File     : ScreenManager.cs.meta
 * Author   : xicheng
 * Date     : 2025-09-11 09:47
 * Tips     : xicheng知识库
 * Description : 屏幕 管理
 横竖屏切换的时候，时机顺序：
    阶段1：方向变化检测(第N帧)
        a.Screen.orientation更新
        b.Screen.orientationChanged事件触发 ，此时Screen.width/height还是旧值。
    阶段2：分辨率更新(第N+1帧)
        a.Screen.width/height更新
        b.Screen.safeArea（依赖尺寸的物理属性）。
        c.OnRectTransformDimensionsChange 被调用
        d.UI系统标记布局重建，Canvas.willRenderCanvases事件触发
 *************************************************************************/

using System;
using UnityEngine;
using Xicheng.Utility;

namespace Xicheng.Screens
{
    [Flags]
    public enum EScreenMode : byte
    {
        None,
        Portrait = 1 << 0, //竖屏模式
        LandscapeLeft = 1 << 1, //横屏：顶部栏左侧
        LandscapeRight = 1 << 2, //横屏：顶部栏右侧
        SquarePortrait = 1 << 3, //方形屏，宽高比小于1.5，算竖屏。（1.5是经验值）
        SquareLandscape = 1 << 4, //方形屏，宽高比大于1.5，算横屏。
    }

    public class ScreenManager : MonoSingleton<ScreenManager>
    {
        [Header("是否支持横屏")]
        public bool isSupportLandScape = false; //是否支持横屏

        //游戏窗口屏幕像素宽高
        public Vector2 ScreenSize => new(Screen.width, Screen.height);

        //设备原生屏幕像素宽高
        public Vector2Int DeviceSize => new(Display.main.systemWidth, Display.main.systemHeight);

        //当前屏幕模式
        public EScreenMode CurScreenMode { get; private set; }

 
        public Action<EScreenMode, Vector2Int> OnScreenSwitch;

        public bool IsPortrait { get; private set; } //true:竖屏  false:横屏
        private readonly float _squareScreenThreshold = 1.5f; //方形屏阈值判断(1.5是经验值)

        public void OnStartUp()
        {
            SetScreenOrientation();
            SetScreenMode();
        }

        private void Start()
        {
            SetScreenOrientation();
        }


        
        private void SetScreenOrientation()
        {
            Screen.orientation = ScreenOrientation.AutoRotation; //屏幕自动旋转
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = isSupportLandScape;
            Screen.autorotateToLandscapeRight = isSupportLandScape;
        }

        private void SetScreenMode()
        {
            var width = Screen.width;
            var height = Screen.height;
            IsPortrait = width < height;
            CurScreenMode = GetByScreenOrientation();
            //无论是横屏还是竖屏，都能计算出屏幕固有的纵横比
            // ReSharper disable once PossibleLossOfFraction
            float scale = Mathf.Max(width, height) / Mathf.Min(width, height);
            //小于1.5认为是方形屏幕, 大于等于1.5认为是正常
            if (scale < _squareScreenThreshold)
            {
                if (width < height)
                {
                    CurScreenMode = EScreenMode.SquarePortrait;
                }
                else if (width > height)
                {
                    CurScreenMode = EScreenMode.SquareLandscape;
                }
                else if (Mathf.Approximately(width, height)) //长等于高
                {
                    CurScreenMode = IsPortrait ? EScreenMode.SquarePortrait : EScreenMode.SquareLandscape;
                }
            }
        }

        public void OnRectTransformDimensionsChange()
        {
            ULog.InfoRed("[Screen] OnRectTransformDimensionsChange");
            SetScreenMode();
            OnScreenSwitch?.Invoke(CurScreenMode, new Vector2Int(Screen.width, Screen.height));
        }
 
        private EScreenMode GetByScreenOrientation()
        {
            EScreenMode screenMode = EScreenMode.None;

            switch (Screen.orientation)
            {
                case ScreenOrientation.Portrait:
                    screenMode = EScreenMode.Portrait;
                    break;
                case ScreenOrientation.LandscapeLeft:
                    screenMode = EScreenMode.LandscapeLeft;
                    break;
                case ScreenOrientation.LandscapeRight:
                    screenMode = EScreenMode.LandscapeRight;
                    break;
            }

            return screenMode;
        }
    }
}