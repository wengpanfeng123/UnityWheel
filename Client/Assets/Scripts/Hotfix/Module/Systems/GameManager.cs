using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hotfix.Model;
using UnityEngine;
using Xicheng.events;
using Xicheng.Utility;

namespace Hotfix
{
    /// <summary>
    /// 游戏总管理类。
    /// 统一处理各个逻辑类的初始化，更新，释放。
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        private bool _isInitialized;
        private static DataModule _dataModule;
        private static SystemModule _systemModule;
        
        [SerializeField]
        private long maxTimeSlice = 30; //最小时间片段
        private Stopwatch _watch;
        private long _frameTime;
        
        public Action<bool> ApplicationPause;
        public Action ApplicationQuit;
        /// <summary>
        /// 处理器是否繁忙
        /// </summary>
        public bool IsBusy => _watch.ElapsedMilliseconds - _frameTime >= maxTimeSlice;

        public void OnAwake()
        {
            _dataModule = new();
            _systemModule = new();
        }
        
        /// <summary>
        /// 初始化所有标记为启动初始化的逻辑
        /// </summary>
        public void OnStart()
        {
            if (_isInitialized)
                return;
            _watch = Stopwatch.StartNew();

            _dataModule.OnStartUp();
            _systemModule.OnStartUp();
        }
        
        private void Update()
        {
            _frameTime = _watch.ElapsedMilliseconds;
            _dataModule.OnUpdate(Time.deltaTime);
            _systemModule.OnUpdate(Time.deltaTime);
        }

        void OnApplicationPause(bool isPause)
        {
            _systemModule.OnAppPause(isPause);
            _dataModule.OnAppPause(isPause);

            ApplicationPause?.Invoke(isPause);
        }

        void OnApplicationQuit()
        {
            _systemModule.OnAppQuit();
            _dataModule.OnAppQuit();
            
            ApplicationQuit?.Invoke();
            GameEvent.OnRelease();
        }
        
        private void OnDestroy()
        {
            _dataModule.OnDestroy();
            _systemModule.OnDestroy();
            
            OnClose();
        }

        /// <summary>
        /// 关闭模块。
        /// </summary>
        public void OnClose()
        {
            _dataModule.OnClose();
            _systemModule.OnClose();
            _isInitialized = false;
        }

        #region 获取系统或者逻辑类

        
        public static T GetModel<T>() where T : BaseModel, new()
        {
            return _dataModule.GetModel<T>();
        }

        public static T GetSystem<T>() where T : class, ILogic
        {
            return _systemModule.GetSystem<T>();
        }

        #endregion
    }
}