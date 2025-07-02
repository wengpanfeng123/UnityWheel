using UnityEngine;

namespace xicheng.aot
{
    public class AotComp:MonoBehaviour
    {
        public static AotComp Inst { get; private set; }

        private void Awake()
        {
            Inst = this;
        }
        
        /// <summary>
        /// 获取框架组件
        /// </summary>
        /// <typeparam name="T">要获取的框架组件类型</typeparam>
        public static T GetComp<T>() where T : AotBaseComp
        {
            return AotComponentManager.GetComp<T>();
        }
    }
}