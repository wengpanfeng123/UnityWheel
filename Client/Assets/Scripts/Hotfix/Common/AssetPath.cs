namespace Hotfix.Common
{
    public class AssetPath
    {
        public const string AssetFolder = "Assets/AssetsPackage";

        /// <summary>
        /// 获取音频路径
        /// </summary>
        /// <param name="clipName">带路径</param>
        /// <returns></returns>
        public static string GetAudio(string clipName)
        {
            return  $"{AssetFolder}/Audio/{clipName}";
        }
        
        /// <summary>
        /// 获取音频路径
        /// </summary>
        /// <returns></returns>
        public static string GetUIPrefab(string prefabName)
        {
            return  $"{AssetFolder}/Audio/{prefabName}.prefab";
        }
    }
}