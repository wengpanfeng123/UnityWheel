using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hotfix;
using UnityEditor;
using UnityEngine;
using xicheng.common;
using Debug = UnityEngine.Debug;

public class ExcelExport : MonoBehaviour {
    [MenuItem("Tools/Gen表", false, 3)]
    static void DoExport2()
    {
        string doCmd = String.Empty;
#if UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
        doCmd = "gen.sh";
#elif UNITY_STANDALONE_WIN
        doCmd = "gen_table.bat";
#endif

        CreateShellExProcess(doCmd, "", MainConst.ExcelRoot);

        Debug.Log("Gen表结束");
        UITools.CreateUIKey();
        Debug.Log("生成UIKeys");
    }

    private static Process CreateShellExProcess(string cmd, string args, string workingDir = "")
    {
        var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = true;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;
        if (!string.IsNullOrEmpty(workingDir))
            pStartInfo.WorkingDirectory = workingDir;
        return System.Diagnostics.Process.Start(pStartInfo);
    }
}