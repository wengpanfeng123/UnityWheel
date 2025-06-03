using UnityEngine;

namespace xicheng.common
{
    public class MainConst
    {
        public static string LoadingScenePath = "Assets/AssetsPackage/Scenes/Loading.unity";
        
        public static string HotfixDllName ="Hotfix.Runtime";
        public static string HotUpdateDllDir = "AssetsPackage/HotUpdateDll";
        public static string PanelPrefabPath = "Assets/AssetsPackage/ui/panels";
        public static string UIGenCsCodePath = "Assets/Scripts/Hotfix/Module/ui/logic";
        public static string UIKeyCsCodePath = "Assets/Scripts/Hotfix/Module/ui";
        public static string ExcelRoot = Application.dataPath + "/../../Common/Configs";
        public static string ExcelPath = ExcelRoot+"/DataTables/Datas";
        public static string ExcelBatPath = ExcelRoot+"/gen_table.bat";
        public static string ExcelShPath = ExcelRoot+"/gen.sh";
    }
}