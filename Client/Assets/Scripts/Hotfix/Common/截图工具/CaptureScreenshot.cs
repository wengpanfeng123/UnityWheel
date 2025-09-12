/*************************************************************************
 * Copyright  xicheng. All rights reserved.
 *------------------------------------------------------------------------
 * File     : CaptureScreenshot.cs
 * Author   : xicheng
 * Date     : 2025-09-03 14:58
 * Tips     : xicheng知识库
 * Description :截屏工具
 *************************************************************************/

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace HsJam
{
    /// <summary>
    /// 捕捉屏幕截图
    /// </summary>
    public class CaptureScreenshot
    {
        /// <summary>
        /// (方式1)自定义截图方法
        /// </summary>
        public static IEnumerator ScreenshotPixels(string fileName, TextureFormat format = TextureFormat.ARGB32,
            Action<string> onComplete = null)
        {
            yield return new WaitForEndOfFrame();

            int width = Screen.width;
            int height = Screen.height;

            Texture2D tex = new Texture2D(width, height, format, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            byte[] pngData = tex.EncodeToPNG();
            UnityEngine.Object.Destroy(tex);

            string folderPath = GetScreenshotFolderPath();
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);
            File.WriteAllBytes(filePath, pngData);
            onComplete?.Invoke(filePath);
        }

        private static string GetScreenshotFolderPath()
        {
#if UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath, "Screenshots");
#elif UNITY_IOS
            return Path.Combine(Application.temporaryCachePath, "Screenshots");
#else
            return Path.Combine(Application.dataPath, "Screenshots");
#endif
        }

        
        /// <summary>
        /// (方式2)Unity自带的截图方法
        /// </summary>
        public static IEnumerator UnityCaptureScreen(string fileName, Action<string> onComplete)
        {
            yield return new WaitForEndOfFrame();

            string folderPath = GetScreenshotFolderPath();
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, fileName);

            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log($"Screenshot saved to: {filePath}");

#if UNITY_ANDROID
            // 等待一帧以确保截图文件生成
            yield return new WaitForSeconds(0.1f);
#endif
            onComplete?.Invoke(filePath);
        }
    }
}