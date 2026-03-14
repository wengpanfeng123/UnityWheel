using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RedPointNodePathDrawer.cs
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XiCheng.RedSystem;

[CustomPropertyDrawer(typeof(RedPointNodeSelectorAttribute))]
public class RedPointNodePathDrawer : PropertyDrawer
{
    private const string SEARCH_PLACEHOLDER = "Search nodes...";
    private static List<string> cachedPaths = new List<string>();
    private string searchQuery = "";
    private bool showSelector;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 创建支持富文本的按钮样式
        GUIStyle richTextButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            richText = true, // 关键设置
            alignment = TextAnchor.MiddleLeft
        };

        var ds = property.serializedObject.targetObject;
        
        var attr = attribute as RedPointNodeSelectorAttribute;
 
        var buttonRect = new Rect(position.x, position.y, position.width - 60, position.height);
        var refreshRect = new Rect(position.xMax - 55, position.y, 50, position.height);

        // 绘制主按钮
        if (GUI.Button(buttonRect, GetDisplayText(property.stringValue),richTextButtonStyle))
        {
            CacheNodePaths();
            showSelector = true;
            SearchablePopup.Show(buttonRect, cachedPaths, selected =>
            {
                property.stringValue = selected;
                property.serializedObject.ApplyModifiedProperties();
            }, searchQuery);
        }

        // 刷新按钮
        if (GUI.Button(refreshRect, "Refresh"))
        {
            CacheNodePaths(true);
        }

        // 显示当前路径
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            var labelRect = new Rect(position.x, position.y+ position.height*0.5f, position.width, 16);
            EditorGUI.LabelField(labelRect, $"{attr.Name}: {property.stringValue}", EditorStyles.miniLabel);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 
               (string.IsNullOrEmpty(property.stringValue) ? 0 : 18);
    }

    private string GetDisplayText(string path)
    {
        return string.IsNullOrEmpty(path) ? 
            "Select RedPoint Node" : 
            $"<color=#4EC9B0>{path}</color>";
    }

    private static void CacheNodePaths(bool forceRefresh = false)
    {
        if (cachedPaths.Count == 0 || forceRefresh)
        {
            cachedPaths.Clear();
            
            // 获取所有预定义的节点路径（需配合配置系统）
            // var config = Resources.Load<RedPointConfig>("RedPointConfig");
            // if (config != null)
            // {
            //     cachedPaths.AddRange(config.PredefinedPaths);
            // }
            cachedPaths.AddRange(RedPointHelper.GetAllPaths());
            // 获取运行时已注册的节点
            if (Application.isPlaying)
            {
                var system = RedPointSystem.Inst;
                if (system != null)
                {
                    cachedPaths.AddRange(system.GetAllNodePaths());
                }
            }
            // 去重排序
            cachedPaths = new List<string>(new HashSet<string>(cachedPaths));
            cachedPaths.Sort(CompareNodePaths);
        }
    }

    private static int CompareNodePaths(string a, string b)
    {
        // 按层级深度排序
        var aDepth = a.Split('/').Length;
        var bDepth = b.Split('/').Length;
        return aDepth == bDepth ? 
            string.Compare(a, b, System.StringComparison.Ordinal) : 
            aDepth.CompareTo(bDepth);
    }
}
#endif
