using UnityEngine;
using System.IO;
namespace Xicheng.log.Log
{
    //如果我们希望将日志信息输出到一个文本文件中，则可以新建一个FileLogHandler实现 ILogHandler，并将日志写入文件：
    public class FileLogHandler : ILogHandler
    {
        private string logFilePath = Application.persistentDataPath + "/log.txt";

        public void Log(LogLevel level, string message, UnityEngine.Object context = null)
        {
            string formatted = $"[{System.DateTime.Now}][{level}] {message}";
            File.AppendAllText(logFilePath, formatted + "\n");
        }
    }
}