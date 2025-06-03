 
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public partial class FileUtils
    {
        /// <summary>
        /// 根据路径获取文件名(带后缀)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("传入参数不正确，请检查！");
                return string.Empty;
            }

            path = path.Replace("\\", "/");
            var s = path.Split('/');
            var fileName = s[^1];
            return fileName;
        }
    }
 