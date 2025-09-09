﻿//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Diagnostics;
using Hotfix.Common;
using UnityEngine;

/// <summary>
/// 打开文件夹相关的实用函数。
/// </summary>
public static class OpenFolders
{
#if UNITY_EDITOR
    /// <summary>
    /// 打开Excel表文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/ExcelPath", false, 9)]
    public static void OpenDataTablePath()
    {
        Execute(PathConst.ExcelPath);
    }


    /// <summary>
    /// 打开 Data Path 文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/Data Path", false, 10)]
    public static void OpenFolderDataPath()
    {
        Execute(Application.dataPath);
    }

    /// <summary>
    /// 打开 Persistent Data Path 文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/Persistent Data Path", false, 11)]
    public static void OpenFolderPersistentDataPath()
    {
        Execute(Application.persistentDataPath);
    }

    /// <summary>
    /// 打开 Streaming Assets Path 文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/Streaming Assets Path", false, 12)]
    public static void OpenFolderStreamingAssetsPath()
    {
        Execute(Application.streamingAssetsPath);
    }

    /// <summary>
    /// 打开 Temporary Cache Path 文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/Temporary Cache Path", false, 13)]
    public static void OpenFolderTemporaryCachePath()
    {
        Execute(Application.temporaryCachePath);
    }


    /// <summary>
    /// 打开 Console Log Path 文件夹。
    /// </summary>
    [UnityEditor.MenuItem("Tools/Open Folder/Console Log Path", false, 14)]
    public static void OpenFolderConsoleLogPath()
    {
        Execute(System.IO.Path.GetDirectoryName(Application.consoleLogPath));
    }

#endif

    /// <summary>
    /// 打开指定路径的文件夹。
    /// </summary>
    /// <param name="folder">要打开的文件夹的路径。</param>
    public static void Execute(string folder)
    {
        folder = $"\"{folder}\"";

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                break;

            case RuntimePlatform.OSXEditor:
                Process.Start("open", folder);
                break;

            default:
                throw new Exception($"Not support open folder on '{Application.platform}' platform.");
        }
    }
}