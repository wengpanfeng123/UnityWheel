using UnityEngine;

/// <summary>
/// 资源配置（文件夹，路径各种配置）
/// </summary>
public class ResConst
{
    /*关于路径的相关描述。
 * 解析路径:string fullPath =  Application.dataPath + "ABRes/"+'UI/panel/meet/panel_mainui_meet.prefab'
 *     1. [fullPath]                              --叫做：fullPath
 *     2. Application.dataPath + "ABRes/";        --叫做：资源根路径 [resRootPath]
 *     3.'Assets/ABRes/UI/panel/meet/panel_mainui_meet.prefab' --叫做：assetPath.
       4. UI/panel/meet/panel_mainui_meet.prefab' --叫做：assetResPath.  资源路径
 *     5. panel_mainui_meet.prefab                --叫做：assetName.
 *     6.".ab"                                    --叫做：assetBundle路径后缀
 *
     *
     * 打包策略方式：
     * (1)第一种方式
     *    主资源 按照指定方式打包。
     *    依赖资源 按照策略打包。策略：引用次数 + 大小 +（如果是图片，尺寸大小）。
     *    (依赖资源如果是主资源，则不处理它)
     *  这种方式实现有难度。灵活。
     * 
     * (2)AssetsPackage目录下所有的文件和其依赖资源。按照 引用次数和大小 进行打包。
     *  这种方式比较简单。但是缺乏自定义文件夹指定打包方式。
     * 
 */
    /*MenuItem*/
    public static string RemoteRootUrl = "http://192.168.31.191/ServerFiles/";
    
    public static readonly string VersionName =  "version.json";
    public static readonly string AssetMapName = "assetMap.json";
    public static readonly string AbMapName = "abMap.json";
    public static readonly string hotfixJsonName = "hotfix.json";
    
    public static string _Temp = "_Temp";
    public static string TmpSuffix = ".tmp";
    public static string PatchDir = "Patch";
    public static string PersistentTemp;//持久化目录的temp 目录
    public static string PersistentSave; //资源下载的本地目录
    public static string StreamingAssetsSave =$"{Application.streamingAssetsPath}/{PatchDir}";
    
    public static void Initialize()
    {
        PersistentTemp = $"{Application.persistentDataPath}/{_Temp}";
        PersistentSave =$"{Application.persistentDataPath}/{PatchDir}";
    }

    public const string AssetFolder = "AssetsPackage";
    public const string MenuName = "AssetsPackage";
    
    public const string SetAssetBundleMark = MenuName + "/AssetBundle(旧)/SetAssetBundleMark";
    public const string ClearAssetBundleMark = MenuName + "/AssetBundle(旧)/ClearAssetBundleMark";
    public const string BuildScenes = MenuName + "/AssetBundle(旧)/BuildScenes";
    public const string BuildAssetBundle = MenuName + "/AssetBundle(旧)/BuildAssetBundle";
    public const string CheckDependency = MenuName + "/分析选中物体依赖文件";
    public const string CollectResInfo = MenuName + "/采集资源信息";
    public const string CheckLoop = MenuName + "/检查循环引用";
    public const string CheckLoop2 = MenuName + "/检查循环引用(BFS)";

    public static string PackageRootPath = $"{Application.dataPath}/{AssetFolder}";
    public static string PackageRootPathTest = $"{Application.dataPath}/{AssetFolder}";
    public static string AssetResRoot = $"Assets/{AssetFolder}";
    public static string UpdateDllAbsolutePath = $"{Application.dataPath}/{AssetFolder}/HotUpdateDll/Hotfix.Runtime.dll.bytes";
    public static string HotfixJsonAbsolutePath = $"{Application.dataPath}/{AssetFolder}/HotUpdateDll/{hotfixJsonName}";
    
    public static string AssetBundleManifest = "working";
    public static string PackageAssetBundleSuffix = ".ab";
    public static string AssetBundleOutputPath = Application.dataPath + "/../AssetBundle/working"; //打包文件的输出目录
    public static string AssetBundleRootOutput = Application.dataPath + "/../AssetBundle"; //输出目录根目录

  
    public static string AssetMapPath = $"{PackageRootPath}/{AssetMapName}";
    public static string BundleMapPath = $"{PackageRootPath}/{AbMapName}";


    public static int ThresholdDepCount = 2; //被依赖2次会被单独打包。
    public static int ThresholdSize = 200;  //大小超过会被打包。
    public static string DepAssetABName = "depasset_";     //依赖资源打包的名称前缀

    public static float MAX_CACHE_TIME = 5; //最大缓存时长。
    public static string ResVersion { get; set; }
    public static string AppVersion { get; set; }
    public static string NotifyVersion { get; set; }
    
    
    public static string hotUpdateDllByteName = "Hotfix.Runtime.dll.bytes";
    public static string hotUpdateNameDll = "Hotfix.Runtime.dll";
    
}
