#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hotfix.Common;
using Xicheng.log.Log;
using UnityEditor;
using Xicheng.UIsystem.gen;
using UnityEngine;

namespace Xicheng.Common
{
    public class FileGen
    {
        private string tagStart = "/* [[ AUTO GENERATE START ]] */";
        private string tagEnd = "/* [[ AUTO GENERATE END ]] */";

        List<Node> addedNodes;
        private HashSet<string> addedNames;

        public string module;
        public Transform transform;

        public FileGen(string module, Transform transform)
        {
            this.module = module;
            this.transform = transform;
        }

        string getComName(Transform com)
        {
            var comName = transform.name;
            if (comName.StartsWith("Com_"))
                comName = comName.Substring(4);
            return comName;
        }

        bool IsComName(Transform com)
        {
            var comName = transform.name;
            return comName.Contains("Com_");
        }
        
        string GenLocalFind(List<Node> nodes)
        {
            HashSet<string> nodeTypes = new HashSet<string>();
            List<Node> typeNodes = new List<Node>();
            foreach (Node node in nodes)
            {
                if (nodeTypes.Contains(node.getLocalFind()))
                {
                    continue;
                }
                nodeTypes.Add(node.getLocalFind());
                typeNodes.Add(node);
            }

            StringBuilder sb = new StringBuilder();
            
            foreach (var node in typeNodes)
            {
                sb.AppendLine(node.getLocalFind());
            }
            sb.ToString().TrimStart();
            return sb.ToString();
        }

        string GenVarDefines(List<Node> nodes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var node in nodes)
            {
                sb.AppendLine(node.getVarDefine());
            }

            return sb.ToString();
        }

        void GetNodes(Transform transform, bool addSelf)
        {
            if (addSelf)
            {
                tryAddNode(transform);
                if (transform.name.StartsWith("Com_"))
                {
                    return;
                }
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                GetNodes(transform.GetChild(i), true);
            }
        }

        void tryAddNode(Transform nodeTransform)
        {
            var name = nodeTransform.name;
            Node node = null;
            if (name.StartsWith("Com_"))
            {
                node = new ComNode(nodeTransform, transform);
            }
            else if (name.StartsWith("Button_"))
            {
                node = new ButtonNode(nodeTransform, transform);
            }
            else if (name.StartsWith("GO_"))
            {
                node = new GoNode(nodeTransform, transform);
            }
            else if (name.StartsWith("Image_"))
            {
                node = new ImageNode(nodeTransform, transform);
            }
            else if (name.StartsWith("RawImage_"))
            {
                node = new RawImageNode(nodeTransform, transform);
            }
            else if (name.StartsWith("Text_") && name.Length > 5 && !name.Contains("#"))
            {
                node = new TextNode(nodeTransform, transform);
            }
            else if (name.StartsWith("Text") || name.StartsWith("t_"))
            {
                node = new TextOtherNode(nodeTransform, transform);
            }
            else if (name.StartsWith("InputField_"))
            {
                node = new InputFieldNode(nodeTransform, transform);
            }
            else if (name.StartsWith("Slider_"))
            {
                node = new SliderNode(nodeTransform, transform);
            }
            else if (name.StartsWith("TMP_"))
            {
                node = new TMPNode(nodeTransform, transform);
            }
            else if (name.StartsWith("LoopList_"))
            {
                node = new LoopListNode(nodeTransform, transform);
            }

            if (node != null)
            {
                if (addedNames.Contains(name))
                {
                    var n = nodeTransform;
                    string p = "";
                    while (n!=null)
                    {
                        p = n.gameObject.name+"/"+p;
                        n = n.parent;
                    }
                    p = p.TrimEnd('/');
                    Debug.LogError($"[FileGen] 重复的命名:{name},   path: \n"+p);
                    occurError = true;
                }

                addedNodes.Add(node);
                if (!(node is TextOtherNode))
                    addedNames.Add(name);
            }
        }

        private string leftL = "{";
        private string rightL = "}";
        
        bool occurError;
        public void GenToFile()
        {
            addedNodes = new List<Node>();
            addedNames = new HashSet<string>();

            occurError = false;
            GetNodes(transform, false);

            if (occurError)
            {
                throw new Exception("生成文件失败");
            }

            var comName = getComName(transform);
            string inherit = "PanelBase"; //继承类
            string awakeString = $" public override void Awake(){leftL}\n        base.Awake();\n        ParseRef();";
            if (comName.StartsWith("Com_"))
                comName = comName.Substring(4);
            if (IsComName(transform))
            {
                inherit = "MonoBehaviour";
                awakeString = $" private void Awake(){leftL}\n        ParseRef();";
            }
            
            var luaPath = $"ui.{module}.{comName}";
            var content = $@"{tagStart}
using UnityEngine;
using UnityEngine.UI;
using xicheng.module.ui;
using TMPro;
using YooAsset;
using DG.Tweening;

public class {comName}:{inherit} 
{leftL}
{GenVarDefines(addedNodes)}
    public void ParseRef(){leftL}
{GenLocalFind(addedNodes)}
    {rightL}
    {awakeString}
    {rightL}
    {tagEnd}
{rightL}";

            luaPath = luaPath.Replace("ui.",string.Empty);
            var filePath = $"{PathConst.UIGenCsCodePath}/{luaPath.Replace('.', '/')}.cs";
            if (File.Exists(filePath))
            {
                string oriTxt = File.ReadAllText(filePath);
                var startIdx = oriTxt.IndexOf(tagStart, StringComparison.Ordinal);
                var endIdx = oriTxt.IndexOf(tagEnd, StringComparison.Ordinal) + tagEnd.Length;
                if (startIdx >= 0)
                {
                    var contentAhead = oriTxt.Substring(0, startIdx);
                    var contentAfter = oriTxt.Substring(endIdx, oriTxt.Length - endIdx);
                    int length = content.LastIndexOf("\r\n", StringComparison.Ordinal);
                    length = content.LastIndexOf("}");
                    if (length > 0)
                    {
                        content = content.Substring(0, length);//如果存在内容，最后多余的一行内容
                    }
                    var newContent = $"{contentAhead}{content}{contentAfter}";
                    File.WriteAllText(filePath, newContent);
                }
                else
                {
                    File.WriteAllText(filePath, content);
                }
            }
            else
            {
                FileUtils.EnsureFolder(filePath);
                // File.WriteAllText(filePath, $"{content}\n\nreturn {comName}");
                File.WriteAllText(filePath, $"{content}");
            }

            ULog.Info($"生成{filePath}",Color.cyan);
            Debug.Log($"生成{filePath}");
            AssetDatabase.Refresh();
        }
    }
}
#endif
