using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
 

namespace UI.Gen
{
    public static class UIAutoBindGenerator
    {
        [MenuItem("GameObject/生成UI-View脚本", false, 11)]
        public static void GenerateUIScript()
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                Debug.LogError("请选择一个 UI 根节点");
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
            bool useAttr = config.useSerializeField;
            string ns = config.@namespace;

            var fields = new List<(string type, string name, string relPath)>();

            foreach (var t in go.GetComponentsInChildren<Transform>(true))
            {
                string type = GetComponentTypeByPrefix(t.name);
                if (type == null) continue;
                string relPath = GetRelativePath(go.transform, t);
                fields.Add((type, t.name, relPath));
            }

            // 主类
            string mainPath = Path.Combine(path, className + ".cs");
            if (!File.Exists(mainPath))
            {
                var mainSb = new StringBuilder();
                mainSb.AppendLine("using UnityEngine;");
                mainSb.AppendLine();
                if (!string.IsNullOrEmpty(ns)) mainSb.AppendLine($"namespace {ns} {{");
                mainSb.AppendLine($"\tpublic partial class {className} : MonoBehaviour");
                mainSb.AppendLine("\t{");
                mainSb.AppendLine("\t    private void Awake()");
                mainSb.AppendLine("\t    {");
                mainSb.AppendLine("\t        __AutoBindComponents();");
                mainSb.AppendLine("\t        // TODO: 添加自定义初始化逻辑");
                mainSb.AppendLine("\t    }");
                mainSb.AppendLine("\t}");
                if (!string.IsNullOrEmpty(ns)) mainSb.AppendLine("}");

                Directory.CreateDirectory(path);
                File.WriteAllText(mainPath, mainSb.ToString());
            }

            // designer 文件
            var designerSb = new StringBuilder();
            designerSb.AppendLine("using UnityEngine;");
            designerSb.AppendLine("using UnityEngine.UI;");
            designerSb.AppendLine("using TMPro;");
            
            designerSb.AppendLine();
            if (!string.IsNullOrEmpty(ns)) designerSb.AppendLine($"namespace {ns} {{");
            designerSb.AppendLine($"public partial class {className}");
            designerSb.AppendLine("{");

            foreach (var (type, name, _) in fields)
            {
                if (useAttr)
                    designerSb.AppendLine("    [SerializeField]");
                designerSb.AppendLine($"    public {type} {name};");
            }

            designerSb.AppendLine();
            designerSb.AppendLine("    private void __AutoBindComponents()");
            designerSb.AppendLine("    {");

            foreach (var (type, name, relPath) in fields)
            {
                designerSb.AppendLine($"        {name} = transform.Find(\"{relPath}\").GetComponent<{type}>();");
            }

            designerSb.AppendLine("    }");
            designerSb.AppendLine("}");
            if (!string.IsNullOrEmpty(ns)) designerSb.AppendLine("}");

            File.WriteAllText(Path.Combine(path, className + ".designer.cs"), designerSb.ToString());
            AssetDatabase.Refresh();

            Debug.Log($"成功生成 UI View：{className}.cs 和 {className}.designer.cs");
        }

        //绑定规则
        private static string GetComponentTypeByPrefix(string name)
        {
            if (name.StartsWith("Btn_")) return "Button";
            if (name.StartsWith("TMP_")) return "TextMeshProUGUI";
            if (name.StartsWith("Text_")) return "Text";
            if (name.StartsWith("Img_")) return "Image";
            if (name.StartsWith("Tog_")) return "Toggle";
            if (name.StartsWith("Input_")) return "InputField";
            return null;
        }

        private static string GetRelativePath(Transform root, Transform target)
        {
            var path = target.name;
            while (target.parent != null && target.parent != root)
            {
                target = target.parent;
                path = target.name + "/" + path;
            }

            return path;
        }

        private static UIAutoBindConfig LoadConfig()
        {
            var guids = AssetDatabase.FindAssets("t:UIAutoBindConfig");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<UIAutoBindConfig>(path);
            }

            return null;
        }
    }
}