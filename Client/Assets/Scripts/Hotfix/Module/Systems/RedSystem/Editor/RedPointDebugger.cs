#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/*
 * 通过调试器执行以下操作：
   1. 展开"Main"节点
   2. 定位到"Main/Tasks/Daily"节点
   3. 将数值从5改为99+
   4. 将类型改为Exclamation
   5. 观察父节点状态自动更新
   6. 验证UI显示是否正确
 */
public class RedPointDebugger : EditorWindow
{
    [MenuItem("Tools/-------RedPoint Debugger")]
    public static void ShowWindow()
    {
        GetWindow<RedPointDebugger>("RedPoint Monitor").minSize = new Vector2(400, 300);
    }

    private class NodeViewState
    {
        public bool expanded = false;
    }

    private Dictionary<string, NodeViewState> nodeStates = new Dictionary<string, NodeViewState>();
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private float lastUpdateTime;
    private const float UPDATE_INTERVAL = 0.5f;

    void OnGUI()
    {
        DrawToolbar();
        DrawNodeTree();
    }

    void Update()
    {
        if (EditorApplication.timeSinceStartup - lastUpdateTime > UPDATE_INTERVAL)
        {
            Repaint();
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
        }
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        {
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchFilter = GUILayout.TextField(searchFilter, GUI.skin.FindStyle("ToolbarSeachTextField"));
            
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchFilter = "";
                GUI.FocusControl(null);
            }
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Expand All", EditorStyles.toolbarButton, GUILayout.Width(80)))
                SetAllExpanded(true);
            
            if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton, GUILayout.Width(80)))
                SetAllExpanded(false);
        }
        GUILayout.EndHorizontal();
    }

    private void DrawNodeTree()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        var system = RedPointSystem.Inst;
        
        if (system != null)
        {
            var rootNodes = new List<RedPointNode>();
            foreach (var node in system.nodes.Values)
            {
                if (node.parent == null)
                    rootNodes.Add(node);
            }

            foreach (var node in rootNodes)
                DrawNode(node, 0);
        }
        GUILayout.EndScrollView();
    }

    private void DrawNode(RedPointNode node, int indentLevel)
    {
        if (!MatchesSearchFilter(node.Path)) return;

        var state = GetNodeState(node.Path);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(indentLevel * 16);

            //var state = GetNodeState(node.Path);
            var hasChildren = node.children.Count > 0;

            if (hasChildren)
            {
                var icon = state.expanded ? "IN foldout on" : "IN foldout";
                if (GUILayout.Button(GUIContent.none, GUI.skin.GetStyle(icon), GUILayout.Width(16)))
                    state.expanded = !state.expanded;
            }
            else
            {
                GUILayout.Space(16);
            }

            GUILayout.Label(node.Path, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            var stateData = node.GetState();
            EditorGUI.BeginChangeCheck();
            
            // 值编辑
            var newValue = EditorGUILayout.IntField(stateData.totalValue, GUILayout.Width(60));
            // 类型编辑
            var newType = (RedPointType)EditorGUILayout.EnumPopup(stateData.displayType, GUILayout.Width(100));
            
            if (EditorGUI.EndChangeCheck())
            {
                node.SetValue(newValue, newType);
            }

            // 状态指示
            var color = stateData.isActive ? Color.green : Color.gray;
            DrawColorDot(color);
        }
        GUILayout.EndHorizontal();

        if (state.expanded)
        {
            foreach (var child in node.children)
                DrawNode(child, indentLevel + 1);
        }
    }

    private void DrawColorDot(Color color)
    {
        var rect = GUILayoutUtility.GetRect(16, 16);
        var prevColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, EditorGUIUtility.IconContent("d_winbtn_mac_max").image);
        GUI.color = prevColor;
    }

    private bool MatchesSearchFilter(string path)
    {
        return string.IsNullOrEmpty(searchFilter) || 
               path.ToLower().Contains(searchFilter.ToLower());
    }

    private NodeViewState GetNodeState(string path)
    {
        if (!nodeStates.TryGetValue(path, out var state))
        {
            state = new NodeViewState();
            nodeStates[path] = state;
        }
        return state;
    }

    private void SetAllExpanded(bool expanded)
    {
        foreach (var state in nodeStates.Values)
            state.expanded = expanded;
    }

    [InitializeOnLoad]
    public static class DebuggerInitializer
    {
        // 退出PlayMode时自动关闭调试器
        static DebuggerInitializer()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                var window = GetWindow<RedPointDebugger>();
                if (window != null) window.Close();
            }
        }
    }
}
#endif