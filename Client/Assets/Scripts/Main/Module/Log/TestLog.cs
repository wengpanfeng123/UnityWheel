using System;
using UnityEngine;

namespace Main.Module.Log
{
    public class TestLog : MonoBehaviour
    {
        private void Start()
        {
            Log.Info("Hello World!", this);
            Log.Warning("玩家血量不足", this);
            Log.Error("玩家死亡逻辑异常", this);
            
            ILogHandler consoleLogHandler = new UnityConsoleLogHandler();
            ILogHandler fileHandler = new UnityConsoleLogHandler();
            Log.SetHandler(consoleLogHandler); //可以切换日志处理器
            
            
            //打包自动关闭日志 TODO:
            //管理类中可以处理
            Log.EnableInfo = false;
            Log.EnableWarning = false; 
            
            // 在代码中动态调整粒子系统参数
            var particleSystem = GetComponent<ParticleSystem>();
            var main = particleSystem.main;
        }
    }
}