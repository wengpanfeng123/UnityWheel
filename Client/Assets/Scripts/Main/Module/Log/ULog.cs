using Xicheng.log.Log;
using UnityEngine;
using Xicheng.log;


public static class ULog
{
    private static ILogHandler _handler = new UnityConsoleLogHandler();
    public static bool EnableInfo = true;
    public static bool EnableWarning = true;
    public static bool EnableError = true;

    public static void SetHandler(ILogHandler handler)
    {
        _handler = handler;
        //TODO:通过DEV和Release 来动态为EnableInfo负值。
    }

    public static void Info(string message, Object context = null)
    {
        if (EnableInfo)
            _handler.Log(LogLevels.Info, message, context);
    }

    public static void Info(string message, Color c)
    {
        if (EnableInfo)
        {
            ColorUtility.ToHtmlStringRGB(c);
            //_handler.Log(LogLevel.Info, message, context);
            Debug.Log((object)string.Format("<color=#{0}> {1} </color>", (object)ColorUtility.ToHtmlStringRGB(c),
                message));
        }
    }


    public static void Warning(string message, UnityEngine.Object context = null)
    {
        if (EnableWarning)
            _handler.Log(LogLevels.Warning, message, context);
    }

    public static void Error(string message, UnityEngine.Object context = null)
    {
        if (EnableError)
            _handler.Log(LogLevels.Error, message, context);
    }

    public static void Exception(System.Exception e, UnityEngine.Object context = null)
    {
        _handler.Log(LogLevels.Exception, e.Message, context);
    }
}