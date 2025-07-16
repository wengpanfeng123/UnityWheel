using UnityEngine;

namespace xicheng.log
{
    //默认控制台输出实现
    public class UnityConsoleLogHandler:ILogHandler
    {
        public void Log(LogLevel level, string message, UnityEngine.Object context = null)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log(message, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message, context);
                    break;
                case LogLevel.Exception:
                    Debug.LogException(new System.Exception(message), context);
                    break;
            }
        }
    }
}