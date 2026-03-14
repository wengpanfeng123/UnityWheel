// 样式预设管理器（编辑器工具）
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace HsJam.TextStyle
{
    public class TextStylePresetWin : EditorWindow
    {
        private Vector2 _scrollPos;
        private readonly List<TextStylePreset> _allPresets = new();

        [MenuItem("Tools/UI/文本样式管理器")]
        public static void ShowWindow()
        {
            GetWindow<TextStylePresetWin>("文本样式预设库");
        }

        void OnGUI()
        {
            // 刷新预设列表
            if (GUILayout.Button("刷新预设库"))
            {
                RefreshPresets();
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // 显示所有预设
            foreach (var preset in _allPresets)
            {
                EditorGUILayout.BeginHorizontal();
                // 预设预览
                EditorGUILayout.LabelField(preset.name, GUILayout.Width(150));
                // 应用按钮
                if (GUILayout.Button("应用到选中文本", GUILayout.Width(120)))
                {
                    ApplyPresetToSelection(preset);
                }

                // 编辑按钮
                if (GUILayout.Button("编辑", GUILayout.Width(60)))
                {
                    Selection.activeObject = preset;
                }

                // 定位按钮
                if (GUILayout.Button("定位", GUILayout.Width(60)))
                {
                    LocatePreset(preset);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void LocatePreset(TextStylePreset preset)
        {
            // 高亮显示资源
            EditorGUIUtility.PingObject(preset);

            // 选中资源
            Selection.activeObject = preset;

            // 聚焦Project窗口
            FocusProjectWindow();
        }

        private void FocusProjectWindow()
        {
            // 获取Project窗口并聚焦
            var projectWindowType = Type.GetType("UnityEditor.ProjectBrowser,UnityEditor");
            var projectWindow = EditorWindow.GetWindow(projectWindowType);
            projectWindow.Focus();
        }

        private void RefreshPresets()
        {
            _allPresets.Clear();
            var guids = AssetDatabase.FindAssets("t:TextStylePreset");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var preset = AssetDatabase.LoadAssetAtPath<TextStylePreset>(path);
                if (preset != null) _allPresets.Add(preset);
            }
        }

        private void ApplyPresetToSelection(TextStylePreset preset)
        {
            foreach (var obj in Selection.gameObjects)
            {
                var text = obj.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    preset.ApplyTo(text);
                    EditorUtility.SetDirty(text);
                }
            }
        }
    }
}
#endif