//- Created:       #xicheng#
// - CreateTime:      #CreateTime#
// - Description:   C#游戏工具类

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using xicheng.res;
using Debug = UnityEngine.Debug;

namespace Xicheng.Common
{
    public static class GameUtility
    {
        /// /////////////////////////IO相关////////////////////////////

        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SafeWriteAllLines failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }


    
        

        public static void DeleteDir(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            foreach (var directoryInfo in dir.GetDirectories())
                directoryInfo.Delete(true); //删除子目录和文件
            foreach (var file in dir.GetFiles())
                file.Delete();
        }

        public static void OpenFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;
            global::OpenFolder.Execute(folderPath);
            
            //Application.OpenURL(folderPath);
        }

     
        /// /////////////////////////Transform相关////////////////////////////
        /// <summary>
        /// 面向目标方向
        /// </summary>
        /// <param name="targetDirection">目标方向</param>
        /// <param name="transform">需要转向的对象</param>
        /// <param name="rotationSpeed">转向速度</param>
        public static void LookAtTarget(Vector3 targetDirection, Transform transform, float rotationSpeed)
        {
            if (targetDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
            }
        }

        /// <summary>
        /// 查找子物体（递归查找）
        /// </summary>
        /// <param name="trans">父物体</param>
        /// <param name="goName">子物体的名称</param>
        /// <returns>找到的相应子物体</returns>
        public static Transform FindChild(Transform trans, string goName)
        {
            Transform child = trans.Find(goName);
            if (child != null)
                return child;

            Transform go = null;
            for (int i = 0; i < trans.childCount; i++)
            {
                child = trans.GetChild(i);
                go = FindChild(child, goName);
                if (go != null)
                    return go;
            }

            return null;
        }
        
        public static string GetPath(Transform root, Transform cur)
        {
            StringBuilder sb = new StringBuilder();
            while (cur != root && cur != null)
            {
                sb.Insert(0, $"/{cur.name}");
                cur = cur.parent;
            }

            sb.Remove(0, 1);
            if(cur==null)
                throw new Exception("error root");
            return sb.ToString();
        }
        
        public static void CollectAllChildren(Transform cur, bool addSelf, ref List<Transform> children)
        {
            if (addSelf)
            {
                children.Add(cur);
            }

            for (int i = 0; i < cur.childCount; i++)
            {
                CollectAllChildren(cur.GetChild(i), true, ref children);
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
}
