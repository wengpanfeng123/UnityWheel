using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HsEditor
{
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static int toolCount;
        private static GUIStyle commandStyle;

        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        // Toolbar dimensions
        private const float Space = 8f;
        private const float LargeSpace = 20f;
        private const float ButtonWidth = 32f;
        private const float DropdownWidth = 80f;
        private const float PlayPauseStopWidth = 140f;

        static ToolbarExtender()
        {
            // Determine tool count based on Unity version
            Type toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            string fieldName = GetToolCountFieldName();
            FieldInfo toolIconsField = toolbarType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            toolCount = toolIconsField != null ? GetToolCount(toolIconsField) : GetDefaultToolCount();

            // Subscribe to GUI events
            ToolbarCallback.OnToolbarGUI = OnGUI;
            ToolbarCallback.OnToolbarGUILeft = DrawLeftGUI;
            ToolbarCallback.OnToolbarGUIRight = DrawRightGUI;
        }

        private static string GetToolCountFieldName()
        {
#if UNITY_2019_1_OR_NEWER
            return "k_ToolCount";
#else
            return "s_ShownToolIcons";
#endif
        }

        private static int GetToolCount(FieldInfo field)
        {
#if UNITY_2019_3_OR_NEWER
            return (int)field.GetValue(null);
#elif UNITY_2018_1_OR_NEWER
            return ((Array)field.GetValue(null)).Length;
#else
            return 5; // Default for older versions
#endif
        }

        private static int GetDefaultToolCount()
        {
#if UNITY_2019_3_OR_NEWER
            return 8;
#elif UNITY_2019_1_OR_NEWER
            return 7;
#else
            return 6;
#endif
        }

        private static void OnGUI()
        {
            if (commandStyle == null)
            {
                commandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;
            float playButtonPosition = Mathf.RoundToInt((screenWidth - PlayPauseStopWidth) / 2);

            // Calculate left and right toolbar areas
            Rect leftRect = GetLeftToolbarRect(screenWidth, playButtonPosition);
            Rect rightRect = GetRightToolbarRect(screenWidth, playButtonPosition);

            // Draw toolbars
            DrawToolbarArea(leftRect, LeftToolbarGUI);
            DrawToolbarArea(rightRect, RightToolbarGUI);
        }

        private static Rect GetLeftToolbarRect(float screenWidth, float playButtonPosition)
        {
            Rect rect = new Rect(0, 0, screenWidth, Screen.height);
            rect.xMin += Space + toolCount * ButtonWidth; // Adjust for tool buttons
            rect.xMin += 2 * 64; // Pivot buttons
#if UNITY_2019_3_OR_NEWER
            rect.xMin += Space; // Spacing for Unity 2019.3+
#else
            rect.xMin += LargeSpace;
#endif
            rect.xMax = playButtonPosition;
            rect = AddMargins(rect);
            return rect;
        }

        private static Rect GetRightToolbarRect(float screenWidth, float playButtonPosition)
        {
            Rect rect = new Rect(0, 0, screenWidth, Screen.height);
            rect.xMin = playButtonPosition + commandStyle.fixedWidth * 3; // Play buttons
            rect.xMax = screenWidth - (Space + DropdownWidth * 3 + ButtonWidth + 78); // Right toolbar components
            rect = AddMargins(rect);
            return rect;
        }

        private static Rect AddMargins(Rect rect)
        {
#if UNITY_2019_3_OR_NEWER
            rect.y = 4;
            rect.height = 22;
#else
            rect.y = 5;
            rect.height = 24;
#endif
            rect.xMin += Space;
            rect.xMax -= Space;
            return rect;
        }

        private static void DrawToolbarArea(Rect rect, List<Action> handlers)
        {
            if (rect.width <= 0) return;

            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            foreach (var handler in handlers)
            {
                handler();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private static void DrawLeftGUI()
        {
            DrawHandlers(LeftToolbarGUI);
        }

        private static void DrawRightGUI()
        {
            DrawHandlers(RightToolbarGUI);
        }

        private static void DrawHandlers(List<Action> handlers)
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in handlers)
            {
                handler();
            }
            GUILayout.EndHorizontal();
        }
    }
}
