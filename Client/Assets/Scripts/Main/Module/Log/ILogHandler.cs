public interface ILogHandler
{
    void Log(LogLevel level, string message, UnityEngine.Object context = null);   
}