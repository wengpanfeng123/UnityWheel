using System;
using System.Collections.Generic;
using System.IO;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using UnityEditor;
using UnityEngine;

/*
 * 就是把生成的dll 放到指定的文件夹下 ，要说一下，元数据dll其实是读的HybridCLRGenerate/AOTGenericReferences 里面的PatchedAOTAssemblyList
 */
public class HyBridCLR_GenDLL
{
    static string HotUpdateDllPath =>
        $"{Application.dataPath}/../HybridCLRData/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}/";

    private static string HotUpdateDestinationPath => $"{Application.dataPath}/AssetsPackage/HotUpdateDlls/HotUpdateDll/";

    static string MetaDataDLLPath =>
        $"{Application.dataPath}/../HybridCLRData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}/";

    static string MetaDataDestinationPath => $"{Application.dataPath}/AssetsPackage/HotUpdateDlls/MetaDataDll/";

    //TODO:元数据引用路径。AOTGenericReferences--自动根据项目去引用元数据dll--
    static string AOTGenericReferencesPath => $"{Application.dataPath}/HybridCLRGenerate/AOTGenericReferences.cs";
    
    private const string UNITY_JDK_11_PATH = "/Library/Java/JavaVirtualMachines/jdk-11.jdk/Contents/Home/";
    private const string SYSTEM_JDK_17_PATH = "/Library/Java/JavaVirtualMachines/jdk-17.0.15.jdk/Contents/Home/";

    
    
    [MenuItem("Build/HybridCLR/第一次GenerateAll拷贝热更dll以及元数据dll")]
    private static void HybridCLRCopyDll()
    {
        // 是否有安装HybridCLR
        var controller = new InstallerController();
        if (!controller.HasInstalledHybridCLR())
        {
            Debug.LogError("HybridCLR is not Installer");
            return;
        }
        // 在生成方法中添加
        //Environment.SetEnvironmentVariable("SKIP_JDK_VERSION_CHECK", "true");

        //执行HybridCLR
        PrebuildCommand.GenerateAll();
        CopyHotUpdateDll();
        CopyMetaDataDll();
    }

    [MenuItem("Build/HybridCLR/CompileDllActiveBuildTarget拷贝以及热更dll")]
    private static void HybridCLRHofixCopyDll()
    {
        // 是否有安装HybridCLR
        var controller = new InstallerController();
        if (!controller.HasInstalledHybridCLR())
        {
            Debug.LogError("HybridCLR is not Installer");
            return;
        }

        CompileDllCommand.CompileDllActiveBuildTarget();
        CopyHotUpdateDll();
    }
    
    [MenuItem("Build/HybridCLR/CompileDllActiveBuildTarget拷贝以及元数据dll")]
    private static void HybridCLRHofixCopyMetaDll()
    {
    
        var controller = new InstallerController();
        if (!controller.HasInstalledHybridCLR())
        {
            Debug.LogError("HybridCLR is not Installer");
            return;
        }

        CompileDllCommand.CompileDllActiveBuildTarget();
        CopyMetaDataDll();
    }



    private static void CopyHotUpdateDll()
    {
        var assemblies = SettingsUtil.HotUpdateAssemblyNamesExcludePreserved;
        var dir = new DirectoryInfo(HotUpdateDllPath);
        var files = dir.GetFiles();
        var destDir = HotUpdateDestinationPath;
        if (Directory.Exists(destDir))
            Directory.Delete(destDir, true);
        Directory.CreateDirectory(destDir);
        foreach (var file in files)
        {
            if (file.Extension == ".dll" && assemblies.Contains(file.Name.Substring(0, file.Name.Length - 4)))
            {
                var desPath = destDir + file.Name + ".bytes";
                file.CopyTo(desPath, true);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("copy hot update dlls success!");
    }

    private static void CopyMetaDataDll()
    {
        List<string> assemblies = GetMetaDataDllList();
        var dir = new DirectoryInfo(MetaDataDLLPath);
        var files = dir.GetFiles();
        var destDir = MetaDataDestinationPath;
        if (Directory.Exists(destDir))
            Directory.Delete(destDir, true);
        Directory.CreateDirectory(destDir);
        foreach (var file in files)
        {
            if (file.Extension == ".dll" && assemblies.Contains(file.Name))
            {
                var desPath = destDir + file.Name + ".bytes";
                file.CopyTo(desPath, true);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("copy meta data dll success!");
    }

    private static List<string> GetMetaDataDllList()
    {
        var aotGenericRefPath = AOTGenericReferencesPath;
        List<string> result = new List<string>();
        using (StreamReader reader = new StreamReader(aotGenericRefPath))
        {
            var lineStr = "";
            while (!reader.ReadLine().Contains("new List<string>"))
            {
            }

            reader.ReadLine();
            while (true)
            {
                lineStr = reader.ReadLine().Replace("\t", "");
                if (lineStr.Contains("};"))
                    break;
                var dllName = lineStr.Substring(1, lineStr.Length - 3);
                result.Add(dllName);
            }
        }

        return result;
    }
}