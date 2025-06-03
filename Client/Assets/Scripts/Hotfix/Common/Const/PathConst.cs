using UnityEngine;

namespace Hotfix.Common
{
    public class PathConst
    {
        public const string AssetFolder = "AssetsPackage";
        public static string HotUpdateDestinationPath = $"{Application.dataPath}/{AssetFolder}/HotUpdateDlls/HotUpdateDll/";
        
        public static string HotfixDllName ="HotUpdate";
        public static string HotfixDllName2 ="HotUpdate.dll.bytes";
        public static string PanelPrefabPath = "Assets/AssetsPackage/ui/panels";
        public static string UIGenCsCodePath = "Assets/Scripts/Hotfix/Module/ui/logic";
        public static string UIKeyCsCodePath = "Assets/Scripts/Hotfix/Module/ui";
        public static string ExcelRoot = Application.dataPath + "/../../Common/Configs";
        public static string ExcelPath = ExcelRoot+"/DataTables/Datas";
        public static string ExcelBatPath = ExcelRoot+"/gen_table.bat";
        public static string ExcelShPath = ExcelRoot+"/gen.sh";
        
    }
}