using System;
using System.Collections.Generic;
using System.Linq;
using Xicheng.Datable;
using xicheng.tcp;
using Xicheng.UI;

namespace Hotfix
{
    /// <summary>
    /// 逻辑管理类。
    /// 统一处理各个逻辑类的初始化，更新，释放。
    /// </summary>
    public class LogicSystem
    {
        private bool _isInitialized;
        private Dictionary<Type, ILogic> _logicDict = new();
        private List<IUpdateLogic> _logicUpdateList = new();

        private void ManualRegisterLogic()
        {
            //---------p0顺序(数据类)----------
            RegisterLogic<ILogic>(ModelManager.Inst);
            RegisterLogic<ILogic>(DataTableManager.Inst);
            //---------p1级别顺序------------ 启动时需要的
            RegisterLogic<ILogic>(UserProxy.Instance);
          
            RegisterLogic<ILogic>(EventMgr.Inst);
            RegisterLogic<ILogic>(UIManager.Inst);
            //---------p2级别顺序------------ 使用时的用到模块
        }

        // 手动注册逻辑实例（用于依赖注入）
        private void RegisterLogic<T>(T logic) where T : class, ILogic
        {
            Type type = typeof(T);
            if (_logicDict.ContainsKey(type))
            {
                throw new InvalidOperationException($"Logic {type} already registered");
            }

            _logicDict.Add(type, logic);

            if (logic is IUpdateLogic updateLogic)
            {
                _logicUpdateList.Add(updateLogic);
            }
        }

        /// <summary>
        /// 初始化所有标记为启动初始化的逻辑
        /// </summary>
        public void InitializeStartupLogics()
        {
            if (_isInitialized)
                return;

            ManualRegisterLogic();
            int logicCount = 0;
            foreach (var logic in _logicDict.Values)
            {
                if (logic.InitStartUp)
                {
                    logicCount++;
                    logic.OnInit();
                }
            }

            _isInitialized = true;
            ULog.Info($"LogicSystem initialized with {logicCount} startup logics");
        }

        public void OnUpdate(float deltaTime)
        {
            if (_logicUpdateList is { Count: > 0 })
            {
                foreach (var logic in _logicUpdateList)
                {
                    logic.OnUpdate(deltaTime);
                }
            }
        }

        /// <summary>
        /// 获取逻辑实例（如果未初始化且需要延迟初始化，则进行初始化）
        /// </summary>
        public T GetLogic<T>() where T : class, ILogic
        {
            var type = typeof(T);

            // 如果逻辑尚未注册,抛出异常
            if (!_logicDict.TryGetValue(type, out var logic))
            {
                throw new Exception($"类型{type}未注册，请手动在ManualRegisterLogic()方法中注册！");
            }

            if (!logic.InitStartUp)
            {
                logic.OnInit();
            }

            return logic as T;
        }

        // 自动发现并注册逻辑
        private T DiscoverAndRegisterLogic<T>() where T : class, ILogic
        {
            // 获取所有程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !a.FullName.StartsWith("System") && !a.FullName.StartsWith("Microsoft") &&
                            !a.FullName.StartsWith("UnityEngine")).ToArray();

            // 查找匹配类型
            var logicType = assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => typeof(T).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (logicType == null)
                return null;

            // 创建实例
            var instance = Activator.CreateInstance(logicType) as T;

            // 注册实例
            RegisterLogic(instance);

            return instance;
        }


        public void OnRelease()
        {
            if (_logicUpdateList != null)
            {
                _logicUpdateList.Clear();
                _logicUpdateList = null;
            }

            if (_logicDict != null)
            {
                foreach (var logic in _logicDict.Values)
                {
                    logic.OnRelease();
                }

                _logicDict.Clear();
                _logicDict = null;
            }
            _isInitialized = false;
        }
    }
}