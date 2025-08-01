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
        private Dictionary<Type, ILogic> _logicDict;
        private static List<GameModule> _modules; //游戏模块

        [SerializeField]
        private long maxTimeSlice = 30; //最小时间片段
        private Stopwatch _watch;
        private long _frameTime;
        
        
        /// <summary>
        /// 处理器是否繁忙
        /// </summary>
        public bool IsBusy => _watch.ElapsedMilliseconds - _frameTime >= maxTimeSlice;

        public void OnAwake()
        {
            _logicDict = new();
            //通过这里注册的模块，走统一的生命周期管理。
            _modules = new(4)
            {
                new DataModule(),
                new SystemModule()
            };
        }


        /// <summary>
        /// 初始化所有标记为启动初始化的逻辑
        /// </summary>
        public void OnStart()
        {
            if (_isInitialized)
                return;
            _watch = Stopwatch.StartNew();

            foreach (var module in _modules)
                module.OnStartUp();
        }
        
        private void Update()
        {
            _frameTime = _watch.ElapsedMilliseconds;
            
            foreach (var t in _modules)
            {
                t.OnUpdate(Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            OnClose();
            foreach (var t in _modules)
                t.OnDestroy();
        }

        void OnApplicationQuit()
        {
            foreach (var t in _modules)
                t.OnAppQuit();
            GameEvent.OnRelease();
        }

        void OnApplicationPause(bool isPause)
        {
            foreach (var t in _modules)
            {
                t.OnAppPause(isPause);
            }
        }

        /// <summary>
        /// 关闭模块。
        /// </summary>
        public void OnClose()
        {
            foreach (var logic in _modules)
            {
                logic.OnRelease();
            }

            _logicDict.Clear();
            _logicDict = null;
            _isInitialized = false;
        }

        #region 获取系统或者逻辑类

        public static T GetModel<T>() where T : BaseModel, new()
        {
            return ((DataModule)_modules[0]).GetModel<T>();
        }

        public static T GetSystem<T>() where T : class, ILogic
        {
            return ((SystemModule)_modules[1]).GetSystem<T>();
        }

        #endregion
    }
}