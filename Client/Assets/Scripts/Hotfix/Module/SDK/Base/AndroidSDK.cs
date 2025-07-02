using UnityEngine;

namespace Hotfix.Module.SDK.Base
{
    /*
1. AndroidJavaClass
      用于访问 Java 中的静态类（static class）或调用其静态方法。
      你可以把它看作是对 Java 类的一个引用。
2. AndroidJavaObject
      Java 类的一个实例对象。你可以通过它调用实例方法，访问或设置实例字段。 
     */
    public class AndroidSDK:SDKBase
    {
        protected string ActivityPlayerName;
        private static AndroidJavaClass _sdkBridge;
       
        public override void Init()
        {
            ActivityPlayerName = "com.unity3d.player.UnityPlayer";
            //获取UnityPlayer类
            _sdkBridge ??= new AndroidJavaClass(ActivityPlayerName);
            //获取当前活动（Activity）
            AndroidJavaObject jo = _sdkBridge.GetStatic<AndroidJavaObject>("currentActivity");
        }
        
        #region 静态方法调用
        /// <summary>
        /// 调用java中的静态方法
        /// </summary>
        /// <param name="methodName"></param>
        public void CallStatic(string methodName)
        {
            _sdkBridge.CallStatic(methodName);
        }
        
        public void CallStatic(string methodName,string param1)
        {
            _sdkBridge.CallStatic(methodName,param1);
        }
        #endregion

        public void Call(string methodName)
        {
            AndroidJavaObject javaObject = _sdkBridge.Call<AndroidJavaObject>(methodName);
            javaObject.Call(methodName);
        }
    }
}