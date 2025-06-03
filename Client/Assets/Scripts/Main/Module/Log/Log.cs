using UnityEngine;

namespace Main.Module.Log
{
    public static class Log
    {
        private static ILogHandler _handler = new UnityConsoleLogHandler();
        public static bool EnableInfo = true;
        public static bool EnableWarning = true;
        public static bool EnableError = true;

        public static void SetHandler(ILogHandler handler)
        {
            _handler = handler;
        }

        public static void Info(string message, UnityEngine.Object context = null)
        {
            if (EnableInfo)
                _handler.Log(LogLevel.Info, message, context);
        }
        
        public static void Info(string message, Color c)
        {
            if (EnableInfo)
            {
                ColorUtility.ToHtmlStringRGB(c);
                //_handler.Log(LogLevel.Info, message, context);
                Debug.Log((object) string.Format("<color=#{0}> {1} </color>", (object) ColorUtility.ToHtmlStringRGB(c), message));
            }
        }
        

        public static void Warning(string message, UnityEngine.Object context = null)
        {
            if (EnableWarning)
                _handler.Log(LogLevel.Warning, message, context);
        }

        public static void Error(string message, UnityEngine.Object context = null)
        {
            if (EnableError)
                _handler.Log(LogLevel.Error, message, context);
        }

        public static void Exception(System.Exception e, UnityEngine.Object context = null)
        {
            _handler.Log(LogLevel.Exception, e.Message, context);
        }
    }
}