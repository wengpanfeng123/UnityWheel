using System;
using SuperScrollView;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Finder 
{
    public static Button Button(Transform transform, string path)
    {
        try
        {
            return transform.Find(path).GetComponent<Button>();
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat(
                "Finder.Button: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                transform.name, path, GetFullPathInScene(transform));
            Debug.LogException(ex);
            return null;
        }
    }

    public static Text Text(Transform transform, string path)
    {
        try
        {
            return transform.Find(path).GetComponent<Text>();
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat(
                "Finder.Text: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                transform.name, path, GetFullPathInScene(transform));

            Debug.LogException(ex);
            return null;
        }
    }

    public static Image Image(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<Image>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.Image: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }

        public static RawImage RawImage(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<RawImage>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.RawImage: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }

        public static Toggle Toggle(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<Toggle>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.Toggle: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }

        public static InputField InputField(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<InputField>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.InputField: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }
        

        public static Slider Slider(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<Slider>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.Slider: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }
        
        // public static TextMeshProUGUI TextMeshProUGUI(Transform transform, string path)
        // {
        //     try
        //     {
        //         return transform.Find(path).GetComponent<TextMeshProUGUI>();
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogErrorFormat(
        //             "Finder.TextMeshProUGUI: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
        //             transform.name, path, GetFullPathInScene(transform));
        //         Debug.LogException(ex);
        //         return null;
        //     }
        // }
        //
        
        public static TextMeshProUGUI TextMeshPro(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<TextMeshProUGUI>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.TextMeshPro: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }
        
        public static LoopListView2 LoopListView2(Transform transform, string path)
        {
            try
            {
                return transform.Find(path).GetComponent<LoopListView2>();
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Finder.LoopListView2: Transform:{0} 查找 Path:{1} 失败, Transform节点全路径:{2}",
                    transform.name, path, GetFullPathInScene(transform));
                Debug.LogException(ex);
                return null;
            }
        }
        
        
        public static Animator FindAnimator(Transform transform)
        {
            return transform.GetComponent<Animator>();
        }

  

        public static RectTransform FindRectTransform(Transform transform, string path)
        {
            var t = transform.Find(path);
            if (t is RectTransform)
            {
                return t as RectTransform;
            }
            else
            {
                return null;
            }
        }

        public static RectTransform GetRectTransform(Transform transform)
        {
            if (transform is RectTransform)
            {
                return transform as RectTransform;
            }

            return null;
        }

        
    public static GameObject GameObject(Transform transform, string path)
    {
        var trans = transform.Find(path);
        if (trans == null)
        {
            // Debug.LogErrorFormat("Finder.GameObject: Transform:{0} 查找 Path:{1} 失败,\n{2}",
            //     transform.name, path, Boot.Boot.GetLuaStackTrace());
        }

        return trans.gameObject;
    }
    
    public static object FindComponent(Transform transform, string path, Type type)
    {
        try
        {
            return transform.Find(path).GetComponent(type);
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("Finder.Component: Transform:{0} 查找 Path:{1} 失败",
                transform.name, path);
            Debug.LogException(ex);
            return null;
        }
    }
    public static string GetFullPathInScene(Transform transform)
    {
        string name = transform.name;
        Transform p = transform.parent;
        while (p != null)
        {
            name = p.name + "/" + name;
            transform = p;
            p = transform.parent;
        }

        return name;
    }
    
}
