using UnityEngine;

namespace Xicheng.log
{
    //默认控制台输出实现
    public class UnityConsoleLogHandler:ILogHandler
    {
        public void Log(LogLevels levels, string message, UnityEngine.Object context = null)
        {
            switch (levels)
            {
                case LogLevels.Info:
                    Debug.Log(message, context);
                    break;
                case LogLevels.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case LogLevels.Error:
                    Debug.LogError(message, context);
                    break;
                case LogLevels.Exception:
                    Debug.LogException(new System.Exception(message), context);
                    break;
            }
        }
    }
}