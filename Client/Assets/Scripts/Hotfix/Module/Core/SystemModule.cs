using System;
using System.Collections.Generic;
using kcp2k;

namespace Hotfix
{
    public class SystemModule : GameModule
    {
        private Dictionary<string, ILogic> _systems = new();
        private List<IUpdateLogic> _logicUpdateList = new();
        
        public override void OnStartUp()
        {
            base.OnStartUp();
            //TODO: 注册系统\逻辑类
            
            RegisterSystem<ILogic>(new MyTestSystem());
            
        }
        
        private T RegisterSystem<T>(T logic) where T : class, ILogic
        {
            Type type = typeof(T);
            if (!_systems.TryAdd(type.Name, logic))
            {
                ULog.Info($"[ModuleSystem] Logic {type} already registered");
                return null;
            }

            if (logic is IUpdateLogic updateLogic)
            {
                _logicUpdateList.Add(updateLogic);
            }

            return logic;
        }

        /// <summary>
        ///  获取系统
        /// </summary>
        public T GetSystem<T>() where T : class, ILogic
        {
            var type = typeof(T);
            if (!_systems.TryGetValue(type.Name, out var sys))
            {
                Log.Error($"[ModuleSystem] 类型{type}未注册，请手动在OnStartUp()方法中注册！");
                // RegisterSystem();
                return null;
            }
            return sys as T;
        }

        public override void OnClose()
        {
            base.OnClose();
            foreach (var sys in _systems.Values)
            {
                sys.OnClose();
            }

            _systems?.Clear();
            _systems = null;
            _logicUpdateList?.Clear();
            _logicUpdateList = null;
        }
    }
}