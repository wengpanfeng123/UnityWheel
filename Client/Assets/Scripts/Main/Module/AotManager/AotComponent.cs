using UnityEngine;
using Xicheng.Utility;

namespace Xicheng.aot
{
    public class AotComponent:MonoSingleton<AotComponent>
    {
        /// <summary>
        /// 获取框架组件
        /// </summary>
        /// <typeparam name="T">要获取的框架组件类型</typeparam>
        public static T Get<T>() where T : BaseAotComp
        {
            return AotComponentManager.GetComp<T>();
        }
    }
}