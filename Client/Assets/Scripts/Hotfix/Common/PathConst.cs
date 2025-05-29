using UnityEngine;

namespace Hotfix.Common
{
    public class PathConst
    {
        public const string AssetFolder = "AssetsPackage";
        public static string HotUpdateDestinationPath = $"{Application.dataPath}/{AssetFolder}/HotUpdateDlls/HotUpdateDll/";
    }
}