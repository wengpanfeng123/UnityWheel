using System;
using System.IO;
using UnityEngine;


/// <summary>
/// 文件夹操作
/// </summary>
public partial class FileUtils
{
    /// <summary>
    /// 根据文件路径创建文件夹
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static bool EnsureFolder(string filepath)
    {
        var folder = Path.GetDirectoryName(filepath);
        if (null != folder && !Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (System.UnauthorizedAccessException)
            {
                //没有权限
                UnityEngine.Debug.LogError("EnsureFolder failed with UnauthorizedAccessException");
                return false;
            }
        }

        return true;
    }
    
    /// <summary>
    /// 拷贝文件夹到目标文件夹
    /// </summary>
    /// <param name="srcPath"></param>
    /// <param name="destPath"></param>
    /// <param name="destSuffix"></param>
    public static void CopyDirectoryTo(string srcPath, string destPath, string destSuffix = null)
    {
        try
        {　　　　
            DirectoryInfo dir = new DirectoryInfo(srcPath);　　　　
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
            foreach (FileSystemInfo i in fileInfo)
            {
                if (i is DirectoryInfo)     //判断是否文件夹
                {
                    if (!Directory.Exists(destPath+"\\"+i.Name))
                    {
                        Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                    }

                    CopyDirectoryTo(i.FullName, destPath + "\\" + i.Name, destSuffix); //递归调用复制子文件夹
                }
                else
                {
                    if (i.FullName.EndsWith(".meta"))
                        continue;
                    string destName = destPath + "/" + i.Name + destSuffix;
                    File.Copy(i.FullName, destName,true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }
    
    public static void SafeCopy(string source, string dest)
    {
        foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(source, dest));
        }

        foreach (string filePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(filePath, filePath.Replace(source, dest), true);
        }
    }
}