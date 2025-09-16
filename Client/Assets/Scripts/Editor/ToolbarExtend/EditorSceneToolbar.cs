using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


namespace HsEditor
{
    /// <summary>
    /// 如何在EditortiaoPlay按钮旁边添加按钮条件
    /// </summary>
    [InitializeOnLoad]
    public static class EditorSceneSwitchToolbar
    {
        private const string ToolbarTypeName = "UnityEditor.Toolbar";
        private const string RootFieldName = "m_Root";
        private const string TargetElementNameRight = "ToolbarZoneRightAlign";//play按钮的右侧区域
        private const string TargetElementNameLeft = "ToolbarZoneLeftAlign"; //play按钮的左侧区域
        private static Type s_ToolbarType;
        private static ScriptableObject s_CurrentToolbar;
        private static VisualElement s_CustomToolbarParent;
        private static IMGUIContainer s_CustomContainer;

        private static bool s_Initialized = false;
        private static bool s_InitializationFailed = false;

        static EditorSceneSwitchToolbar()
        {
            return; //如果用这种方式的EditorButton,去掉本行return代码.
            EditorApplication.update += OnUpdate;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }

        private static void OnUpdate()
        {
            if (s_Initialized || s_InitializationFailed)
                return;

            try
            {
                InitializeToolbar();
            }
            catch (Exception e)
            {
                s_InitializationFailed = true;
                Debug.LogError($"Failed to initialize CruToolbar: {e.Message}");
                Cleanup();
            }
        }

        private static void InitializeToolbar()
        {
            // 1. 获取Toolbar类型
            if (s_ToolbarType == null)
            {
                s_ToolbarType = typeof(Editor).Assembly.GetType(ToolbarTypeName);
                if (s_ToolbarType == null)
                {
                    throw new Exception($"Could not find type: {ToolbarTypeName}");
                }
            }

            // 2. 获取Toolbar实例
            if (s_CurrentToolbar == null)
            {
                UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(s_ToolbarType);
                if (toolbars == null || toolbars.Length == 0)
                {
                    // Toolbar可能还没创建，等待下一帧
                    return;
                }

                s_CurrentToolbar = (ScriptableObject)toolbars[0];
            }

            // 3. 获取root VisualElement
            FieldInfo rootField = s_ToolbarType.GetField(RootFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null)
            {
                throw new Exception($"Could not find field: {RootFieldName}");
            }

            VisualElement rootVisualElement = rootField.GetValue(s_CurrentToolbar) as VisualElement;
            if (rootVisualElement == null)
            {
                throw new Exception($"Could not get root VisualElement");
            }

            // 4. 查找目标容器
            VisualElement toolbarZone = rootVisualElement.Q(TargetElementNameLeft);
            if (toolbarZone == null)
            {
                throw new Exception($"Could not find element: {TargetElementNameRight}");
            }


            // 5. 创建自定义UI
            s_CustomToolbarParent = new VisualElement()
            {
                style =
                {
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    marginLeft = 0, // 添加一些间距，看起来更自然
                    paddingLeft = 450,
                }
            };

            s_CustomContainer = new IMGUIContainer(OnGuiBody)
            {
                style =
                {
                    flexGrow = 0,
                    marginLeft = 4, // 按钮之间的间距
                }
            };

            s_CustomToolbarParent.Add(s_CustomContainer);
            toolbarZone.Add(s_CustomToolbarParent);

            s_Initialized = true;
            EditorApplication.update -= OnUpdate;

            Debug.Log("CruToolbar initialized successfully.");
        }

        private static void OnGuiBody()
        {
            // 确保即使在异常情况下也不会破坏整个工具栏
            try
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(new GUIContent("InitScene", EditorGUIUtility.FindTexture("PlayButton")),
                        EditorStyles.toolbarButton))
                {
                    OnClickInitScene();
                }

                if (GUILayout.Button(new GUIContent("GameScene", EditorGUIUtility.FindTexture("PlayButton")),
                        EditorStyles.toolbarButton))
                {
                    OnClickGameScene();
                }

                if (GUILayout.Button(new GUIContent("SET", EditorGUIUtility.FindTexture("PlayButton")),
                        EditorStyles.toolbarButton))
                {
                    OnClickInitScene();
                }

                if (GUILayout.Button(new GUIContent("Launcher", EditorGUIUtility.FindTexture("PlayButton")),
                        EditorStyles.toolbarButton))
                {
                    OnClickGameScene();
                }

                GUILayout.EndHorizontal();
            }
            catch (Exception e)
            {
                // 记录错误但不会影响其他UI
                Debug.LogError($"Error in CruToolbar GUI: {e.Message}");
            }
        }

        private static void OnBeforeAssemblyReload()
        {
            Cleanup();
        }
        
        private static void Cleanup()
        {
            // 移除事件监听
            EditorApplication.update -= OnUpdate;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;

            // 清理UI元素
            if (s_CustomToolbarParent != null && s_CustomToolbarParent.parent != null)
            {
                s_CustomToolbarParent.parent.Remove(s_CustomToolbarParent);
            }

            s_CustomContainer = null;
            s_CustomToolbarParent = null;
            s_CurrentToolbar = null;
            s_ToolbarType = null;

            s_Initialized = false;

            Debug.Log("CruToolbar cleaned up.");
        }

// 可选：添加一个菜单项来手动重新初始化
/*[MenuItem("Tools/CruToolbar/Reinitialize")]
private static void Reinitialize()
{
    Cleanup();
    s_InitializationFailed = false;
    EditorApplication.update += OnUpdate;
    Debug.Log("CruToolbar reinitialization requested.");
}*/

// 可选：添加一个菜单项来检查状态
/*[MenuItem("Tools/CruToolbar/Check Status")]
private static void CheckStatus()
{
    Debug.Log($"CruToolbar Status: Initialized={s_Initialized}, Failed={s_InitializationFailed}");
}*/

        private static void OnClickInitScene()
        {
            string scenePath = "Assets/Scenes/LoadScene.unity";
            UnityEngine.Object sceneAsset = AssetDatabase.LoadMainAssetAtPath(scenePath);
            if (sceneAsset != null && sceneAsset is SceneAsset)
            {
                // 检查当前场景是否有未保存的更改
                if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty)
                {
                    // 提示用户保存
                    if (EditorUtility.DisplayDialog("Scene Modified",
                            "Do you want to save the changes to the current scene?",
                            "Save", "Don't Save"))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    }
                }

                // 打开新场景
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            }
        }

        private static void OnClickGameScene()
        {
            string scenePath = "Assets/Scenes/GameScene.unity";
            UnityEngine.Object sceneAsset = AssetDatabase.LoadMainAssetAtPath(scenePath);
            if (sceneAsset != null && sceneAsset is SceneAsset)
            {
                // 检查当前场景是否有未保存的更改
                if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty)
                {
                    // 提示用户保存
                    if (EditorUtility.DisplayDialog("Scene Modified",
                            "Do you want to save the changes to the current scene?",
                            "Save", "Don't Save"))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    }
                }

                // 打开新场景
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}