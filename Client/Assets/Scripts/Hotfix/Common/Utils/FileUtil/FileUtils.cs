using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// 文件操作
/// </summary>
public partial class FileUtils 
{
    /// <summary>
    /// 文件读取为Byte数据
    /// </summary>
    /// <param name="filePath">文件绝对路径</param>
    /// <returns></returns>
    public static byte[] ReadAllBytes(string filePath )
    {
        using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        try
        {
            byte[] buffur = new byte[fs.Length];
            fs.Read(buffur, 0, (int)fs.Length);
            return buffur;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
 
    /// <summary>
    /// 获取文件的MD5码
    /// </summary>
    /// <param name="absolutePath">绝对路径路径</param>
    /// <returns></returns>
    public static string GetFileMD5(string absolutePath)
    {
        try
        {
            FileStream file = new FileStream(absolutePath, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
    
    /// <summary>
    /// 获取字符串MD5
    /// </summary>
    /// <param name="str">需要计算MD5的字符串</param>
    /// <returns>32位的字符串</returns>
    public static string GetStringMd5(string str)
    {
        var md5 = MD5.Create(); // or var md5 = new MD5CryptoServiceProvider();
        var bytValue = Encoding.UTF8.GetBytes(str);
        var bytHash = md5.ComputeHash(bytValue);
        StringBuilder sb = new StringBuilder();
        foreach (var b in bytHash)
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }
    
    //获取文件大小。
    public static float GetFileSize(string path, FileUnit unit)
    {
        FileInfo fi = new FileInfo(path);
        string stringValue = "-1";
        if (fi.Exists)
        {
            switch (unit)
            {
                case FileUnit.Kb:
                    stringValue =(fi.Length / 1024.0).ToString("0.000");
                    break;
                case FileUnit.Mb:
                    stringValue =(fi.Length / 1024.0 / 1024).ToString("0.000");
                    break;
            }
        }
        float val = float.Parse(stringValue);
        return val;
    }
    
    /// <summary>
    /// 拷贝文件到目标文件夹
    /// </summary>
    /// <param name="sourceFile">资源绝对路径</param>
    /// <param name="targetFolder">目标文件夹</param>
    public static void CopyFile(string sourceFile, string targetFolder)
    {
        try
        {
            // 确保目标文件夹存在
            if(!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // 构造目标文件路径（保持原文件名）
            string fileName = Path.GetFileName(sourceFile);
            string destFile = Path.Combine(targetFolder, fileName);

            // 复制文件（覆盖已存在的文件）
            File.Copy(sourceFile, destFile, overwrite: true);

            Debug.Log($"文件已复制到：{destFile}");
        }
        catch (Exception ex)
        {
            Debug.Log($"复制失败：{ex.Message}");
        }
    }
}

public enum FileUnit
{
    Kb = 0,
    Mb = 1,
}
