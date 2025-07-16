using System;
using UnityEngine;

namespace xicheng.log.Log
{
    public class TestULog : MonoBehaviour
    {
        private void Start()
        {
            ULog.Info("Hello World!", this);
            ULog.Warning("玩家血量不足", this);
            ULog.Error("玩家死亡逻辑异常", this);
            
            ILogHandler consoleLogHandler = new UnityConsoleLogHandler();
            ILogHandler fileHandler = new UnityConsoleLogHandler();
            ULog.SetHandler(consoleLogHandler); //可以切换日志处理器
            
            
            //打包自动关闭日志 TODO:
            //管理类中可以处理
            ULog.EnableInfo = false;
            ULog.EnableWarning = false; 
            
            // 在代码中动态调整粒子系统参数
            var particleSystem = GetComponent<ParticleSystem>();
            var main = particleSystem.main;
        }
    }
}