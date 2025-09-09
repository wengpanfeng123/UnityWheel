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
        /// 获取UI预制体路径
        /// </summary>
        /// <returns></returns>
        public static string GetUIPrefab(string prefabName)
        {
            return  $"{AssetFolder}/UI/Prefabs/{prefabName}.prefab";
        }
        
        /// <summary>
        /// 获取UI预制体路径
        /// </summary>
        /// <returns></returns>
        public static string GetUISprite(string spriteName)
        {
            return  $"{AssetFolder}/UI/Sprites/{spriteName}.png";
        }
        
        /// <summary>
        /// 获取场景路径
        /// </summary>
        public static string GetScene(string sceneName)
        {
            return  $"{AssetFolder}/Scenes/{sceneName}.unity";
        }
        
        
    }
}