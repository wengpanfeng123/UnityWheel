using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// RedPointConfig.cs

[CreateAssetMenu(fileName = "RedPointConfig", menuName = "RedPoint System/Config")]
public class RedPointConfig : ScriptableObject
{
    public List<string> PredefinedPaths = new List<string>
    {
        "Main",
        "Main/Tasks",
        "Main/Mail",
        "Main/Shop",
        "Main/Social"
    };

    public void AddPath(string path)
    {
        if (!PredefinedPaths.Contains(path))
        {
            PredefinedPaths.Add(path);
            PredefinedPaths.Sort();
        }
    }
}

public static class RedPointPathCfg
{
    private static List<string> PredefinedPaths = new()
    {
        "None",
        "Main",
        "Main/Tasks",
        "Main/Mail",
        "Main/Social/AA/BB",
        "Main/Shop",
        "Main/Social",
        "Main/Social/AA",
    };

    public static List<string> GetPaths()
    {
        PredefinedPaths.Sort();
        return PredefinedPaths;
    }
    
    public static void AddPath(string path)
    {
        if (!PredefinedPaths.Contains(path))
        {
            PredefinedPaths.Add(path);
            PredefinedPaths.Sort();
        }
    }
}