using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HsEditor
{
    [InitializeOnLoad]
    public class SceneSwitchLeftButton
    {
        private static readonly string SceneMain = "Launcher";

        static SceneSwitchLeftButton()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        }

        static readonly string ButtonStyleName = "Tab middle";
        static GUIStyle _buttonGuiStyle;
        static GUIStyle _buttonRightGuiStyle;

        static void OnLeftToolbarGUI()
        {
            _buttonGuiStyle ??= new GUIStyle(ButtonStyleName)
            {
                padding = new RectOffset(2, 8, 2, 2),
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(
                    new GUIContent("Launcher", EditorGUIUtility.FindTexture("PlayButton"), $"Start Scene Launcher"),
                    _buttonGuiStyle))
            {
                SceneHelper.StartScene(SceneMain);
            }
        }
        
        //不起作用：代码执行，但UI没有。
        static void OnRightToolbarGUI()
        {
            // _buttonRightGuiStyle ??= new GUIStyle(ButtonStyleName)
            // {
            //     padding = new RectOffset(2, 8, 2, 2),
            //     alignment = TextAnchor.MiddleCenter,
            //     fontStyle = FontStyle.Bold
            // };
            //
            // // GUILayout.FlexibleSpace();
            // if (GUILayout.Button(
            //         new GUIContent("GameStart", EditorGUIUtility.FindTexture("PlayButton"), $"Start Scene Launcher")))
            // {
            //     SceneHelper.StartScene(SceneMain);
            // }
 
        }
        
    }

    static class SceneHelper
    {
        static string _sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            _sceneToOpen = sceneName;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (_sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string[] guids = AssetDatabase.FindAssets("t:scene " + _sceneToOpen, null);
                if (guids.Length == 0)
                {
                    Debug.LogWarning("Couldn't find scene file");
                }
                else
                {
                    string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    EditorApplication.isPlaying = true;
                }
            }

            _sceneToOpen = null;
        }
    }
}