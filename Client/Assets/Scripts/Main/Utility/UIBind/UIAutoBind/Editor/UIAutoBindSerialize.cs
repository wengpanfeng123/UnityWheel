using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

namespace UI.Gen
{
    public static class UIAutoBindSerialize
    {
        private static string _uiScriptRootDir = "Assets/Scripts/Hotfix/Module/UI/";
        [MenuItem("Assets/生成 UI View 脚本并自动绑定",false,11)]
        public static void GenerateAndBindUIScript()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                Debug.LogError("请选择一个 UI 预制体");
                return;
            }

            var config = LoadConfig();
            if (config == null)
            {
                Debug.LogError("未找到 UIAutoBindConfig，请先创建配置");
                return;
            }

            string className = go.name;
            string path = config.outputPath;
            string ns = config.@namespace;

            // 1. 生成脚本文件
            GenerateScriptFiles(go, className, path, ns, out var scriptPath);

            // 2. 添加脚本组件到预制体
            AddScriptComponent(go, scriptPath, ns, className);

            // 3. 自动绑定组件
            AutoBindComponents(go, className, ns);

            AssetDatabase.Refresh();
            Debug.Log($"UI View 生成并绑定完成: {className}");
        }

        private static void GenerateScriptFiles(GameObject go, string className, string path, string ns, out string scriptPath)
        {
            path = path + "/" + go.name;
            Directory.CreateDirectory(path);
            // scriptPath = Path.Combine(path, $"{className}.designer.cs");
            scriptPath = Path.Combine(path, $"{className}.cs");
            // 主类
            string mainPath = Path.Combine(path, className + ".cs");
            if (!File.Exists(mainPath))
            {
                var mainSb = new StringBuilder();
                mainSb.AppendLine("using UnityEngine;");
                mainSb.AppendLine();
                if (!string.IsNullOrEmpty(ns))
                    mainSb.AppendLine($"namespace {ns} {{");
                mainSb.AppendLine($"\tpublic partial class {className} : MonoBehaviour");
                mainSb.AppendLine("\t{");
                mainSb.AppendLine("\t    private void Awake()");
                mainSb.AppendLine("\t    {");
                //mainSb.AppendLine("	        __AutoBindComponents();");
                mainSb.AppendLine("\t       // TODO: 添加自定义初始化逻辑");
                mainSb.AppendLine("\t    }");
                mainSb.AppendLine("\t}");
                if (!string.IsNullOrEmpty(ns)) 
                    mainSb.AppendLine("}");

                Directory.CreateDirectory(path);
                File.WriteAllText(mainPath, mainSb.ToString());
            }
            // designer 文件
            var designerSb = new StringBuilder();
            designerSb.AppendLine("using UnityEngine;");
            designerSb.AppendLine("using UnityEngine.UI;");
            designerSb.AppendLine("using TMPro;");
            if (!string.IsNullOrEmpty(ns))
                designerSb.AppendLine($"namespace {ns} {{");
            designerSb.AppendLine($"\tpublic partial class {className} ");
            designerSb.AppendLine("\t{");
            foreach (var component in GetBindableComponents(go))
            {
                //designerSb.AppendLine("    [SerializeField]");
                designerSb.AppendLine($"\t   public {component.type} {ToFieldName(component.name)};");
                //designerSb.AppendLine($"    public {component.type} {component.name} => {ToFieldName(component.name)};");
                designerSb.AppendLine();
            }
            
            designerSb.AppendLine("\t}");
            if (!string.IsNullOrEmpty(ns))
                designerSb.AppendLine("}");
            
            File.WriteAllText(Path.Combine(path, className + ".designer.cs"), designerSb.ToString());
            AssetDatabase.Refresh();
            Debug.Log($"成功生成 UI View：{className}.cs 和 {className}.designer.cs");
        }

        private static void AddScriptComponent(GameObject go, string scriptPath, string ns, string className)
        {
            string compName = $"{ns}.{className}";
            // 移除旧组件（如果存在）
            var oldComponent = go.GetComponent(compName);
            if (oldComponent != null)
            {
                Object.DestroyImmediate(oldComponent, true);
            }

            // 添加新组件
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);
            if (script != null)
            {
                Type classType = script.GetClass();
                var component = go.AddComponent(classType);
                EditorUtility.SetDirty(go);
            }
        }

        private static void AutoBindComponents(GameObject go, string className, string ns)
        {
            string compName = $"{ns}.{className}";
            var component = go.GetComponent(compName);
            if (component == null)
            {
                Debug.LogError("AutoBindComponents 获取组件失败。"+compName);
                return;
            }

            var so = new SerializedObject(component);
            
            foreach (var bindable in GetBindableComponents(go))
            {
                var fieldName = $"{ToFieldName(bindable.name)}";
                var prop = so.FindProperty(fieldName);
                
                if (prop != null && bindable.component != null)
                {
                    prop.objectReferenceValue = bindable.component;
                    EditorUtility.SetDirty(go);
                }
            }
            
            so.ApplyModifiedProperties();
            //PrefabUtility.ApplyPrefabInstance(go, InteractionMode.UserAction);
        }

        private static IEnumerable<(string name, string type, Component component)> GetBindableComponents(GameObject root)
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                var type = GetComponentTypeByPrefix(t.name);
                if (type == null) continue;

                var component = t.GetComponent(type);
                if (component != null)
                {
                    yield return (t.name, type, component);
                }
            }
        }

        private static string ToFieldName(string originalName)
        {
            return originalName;
            // 转换命名格式：Btn_Start -> btnStart
            var parts = originalName.Split('_');
            if (parts.Length < 2) return originalName.ToLower();
            
            return parts[0] + 
                   string.Join("", parts.Skip(1).Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)));

            return parts[0].ToLower() + 
                   string.Join("", parts.Skip(1).Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)));
        }

        private static string GetComponentTypeByPrefix(string name)
        {
            if (name.StartsWith("Btn")) return "Button";
            if (name.StartsWith("Tmp")) return "TextMeshProUGUI";
            if (name.StartsWith("Txt")) return "Text";
            if (name.StartsWith("Img")) return "Image";
            if (name.StartsWith("Tog")) return "Toggle";
            if (name.StartsWith("Input")) return "InputField";
            return null;
        }

        private static UIAutoBindConfig LoadConfig()
        {
            var guids = AssetDatabase.FindAssets("t:UIAutoBindConfig");
            return guids.Length > 0 ? 
                AssetDatabase.LoadAssetAtPath<UIAutoBindConfig>(AssetDatabase.GUIDToAssetPath(guids[0])) : 
                null;
        }
    }
}