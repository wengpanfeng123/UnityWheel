public interface ILogHandler
{
    void Log(LogLevels levels, string message, UnityEngine.Object context = null);   
}