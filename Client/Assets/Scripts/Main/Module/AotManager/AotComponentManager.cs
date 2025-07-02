
using System;
using System.Collections.Generic;
using UnityEngine;
using xicheng.common;

public enum CloseType
{
    Restart, //重启
    Quit, //退出
}


namespace xicheng.aot
{
    public static class AotComponentManager
    {
        private static readonly Dictionary<string, AotBaseComp> AotBaseDict = new();

        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="component">要获取的框架组件类型</param>
        internal static void RegisterComponent(AotBaseComp component)
        {
            if (!component)
            {
                Debug.LogError("Current RegisterComponent is null");
                return;
            }

            string fullName = component.GetType().FullName;

            if (AotBaseDict.TryGetValue(fullName, out AotBaseComp value))
            {
                Debug.LogError($"RegisterComponent {fullName} is already exist");
                return;
            }

            AotBaseDict.Add(fullName, component);
        }

        /// <summary>
        /// 获取框架组件
        /// </summary>
        public static T GetComp<T>() where T : AotBaseComp
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// 获取框架组件
        /// </summary>
        public static AotBaseComp GetComponent(Type type)
        {
            if (null == type)
            {
                Debug.LogError("type is null");
                return null;
            }

            // if (!HsApp.Instance)
            // {
            //     return null;
            // }
            
            if (!AotBaseDict.TryGetValue(type.FullName, out AotBaseComp value))
            {
                GameObject go = new GameObject(type.Name);
                go.transform.SetParent(AotComp.Inst.transform);
                value = go.AddComponent(type) as AotBaseComp;
            }
            return value;
        }

        /// <summary>
        /// 获取框架组件
        /// </summary>
        public static AotBaseComp GetComponent(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
            {
                Debug.LogError("typeFullName is empty");
                return null;
            }
            AotBaseDict.TryGetValue(typeFullName, out AotBaseComp value);
            return value;
        }

        /// <summary>
        /// 重启框架所有组件
        /// </summary>
        public static void ReStart()
        {
            using (Dictionary<string, AotBaseComp>.Enumerator enumerator = AotBaseDict.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.ReStart();
                }
            }
        }

        /// <summary>
        /// 关闭框架所有组件
        /// </summary>
        /// <param name="closeType">关闭类型</param>
        public static void Close(CloseType closeType)
        {
            using (Dictionary<string, AotBaseComp>.Enumerator enumerator = AotBaseDict.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Close(closeType);
                }
            }

            if (CloseType.Quit == closeType)
            {
                AotBaseDict.Clear();
            }
        }
    }
}