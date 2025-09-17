#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
// SearchablePopup.cs
public class SearchablePopup : PopupWindowContent
{
    private const float WINDOW_WIDTH = 300;
    private const float WINDOW_HEIGHT = 300;
    private const int ITEM_HEIGHT = 20;

    private System.Action<string> onSelect;
    private List<string> items;
    private Vector2 scrollPos;
    private string searchQuery = "";

    public static void Show(Rect buttonRect, List<string> options, System.Action<string> callback, string initialSearch = "")
    {
        var popup = new SearchablePopup
        {
            items = options,
            onSelect = callback,
            searchQuery = initialSearch
        };

        PopupWindow.Show(buttonRect, popup);
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
    }

    public override void OnGUI(Rect rect)
    {
        // 搜索框
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUI.SetNextControlName("SearchField");
            searchQuery = GUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("", GUI.skin.GetStyle("ToolbarSeachCancelButton")))
            {
                searchQuery = "";
                GUI.FocusControl("SearchField");
            }
        }
        GUILayout.EndHorizontal();

        // 筛选结果
        var filtered = string.IsNullOrEmpty(searchQuery) ?
            items :
            items.Where(p => p.ToLower().Contains(searchQuery.ToLower())).ToList();

        // 绘制列表
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        {
            foreach (var path in filtered)
            {
                var parts = path.Split('/');
                var indent = (parts.Length - 1) * 10;

                GUILayout.BeginHorizontal();
                GUILayout.Space(indent);
                if (GUILayout.Button(path, GetButtonStyle(parts)))
                {
                    onSelect?.Invoke(path);
                    EditorWindow.GetWindow<PopupWindow>().Close();
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }

    private GUIStyle GetButtonStyle(string[] pathParts)
    {
        var style = new GUIStyle(EditorStyles.miniButton)
        {
            alignment = TextAnchor.MiddleLeft,
            fixedHeight = ITEM_HEIGHT
        };

        // 根据层级设置不同颜色
        switch (pathParts.Length)
        {
            case 1:
                style.normal.textColor = new Color(0.8f, 0.9f, 1f);
                break;
            case 2:
                style.normal.textColor = new Color(0.7f, 0.8f, 1f);
                break;
            default:
                style.normal.textColor = Color.white;
                break;
        }

        return style;
    }
}
#endif