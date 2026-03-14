using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SimpleJSON;
using Xicheng.Common;

namespace Hotfix
{
    public class UITools
    {
        private static Dictionary<string, string> m_PathDic = new Dictionary<string, string>();

        [MenuItem("Tools/UI/生成UI路径")]
        static void OpenGameMainScene()
        {
            m_PathDic.Clear();
            string uipathRoot = "Assets/ABRes/UI";

            if (!Directory.Exists(uipathRoot))
                Debug.LogError("路径不存在，请检查 " + uipathRoot);

            DirectoryInfo dir = new DirectoryInfo(uipathRoot);
            FileInfo[] files = dir.GetFiles(".", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (file.Name.EndsWith(".meta"))
                    continue;
                string key = file.Name.Split('.')[0];
                int index = file.FullName.IndexOf("Assets", StringComparison.Ordinal);
                string value = file.FullName.Substring(index).Replace('\\', '/');
                Debug.Log($"({key},{value})");
                m_PathDic[key] = value;
            }

            GentateUIPath();
        }

        private static void GentateUIPath()
        {

            string types = "" + '\t' + '\t' + '\t' + '\t' + "\n";
            foreach (KeyValuePair<string, string> kp in m_PathDic)
            {
                string str = $"           m_AllUIPath[\"{kp.Key}\"] = \"{kp.Value}\" ;" + '\n';
                types += str;
            }

            string text = @"
using System.Collections.Generic;
namespace Hotfix
{
    /// <summary>
    /// 所有UI界面的路徑(自动生成)
    /// </summary>
    public class UIPath
    {" + '\n'
       + "	    private Dictionary<string, string> m_AllUIPath ;" + '\n'
       + " 	    public UIPath() {" + '\n'
       + "            m_AllUIPath = new Dictionary<string, string>();" + '\n'
       + " 	        InitPath();" + '\n'
       + " 	    }" + '\n'
       + '\n'
       + " 	    void InitPath(){" + '\n'
       + types + '\t'
       + "    } " + '\n' + "    " + '\n'
       + " 	    public string GetPathByID(UIEnum pEnum){" + '\n'
       + "            return m_AllUIPath[pEnum.ToString()];" + '\n'
       + "        }" + '\n' + '\n'
       + " 	    public string GetPathByName(string name){" + '\n'
       + "            return m_AllUIPath[name];" + '\n'
       + "        }" + '\n'
       +
       @"    }
}";
            string filePath = Application.dataPath + @"/Scripts/Hotfix/UI/Base/UIPath.cs";
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllText(filePath, text, Encoding.UTF8);
            AssetDatabase.Refresh();
        }
        
        
        [MenuItem("Assets/生成UIKeys",false,11)]
        public static void CreateUIKey()
        {
            StringBuilder sb = new StringBuilder();
            string filePath = Application.dataPath + $@"/{ResConst.AssetFolder}/DataTable/ui_tbuipanel.json";
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                JSONNode  jNode = SimpleJSON.JSONNode.Parse(json);
                foreach (var node in jNode.AsArray)
                {
                    string path = node.Value["path"];
                    string sp = path.Split('/').Last();
                    string key = sp.Split('.').First();
              
                    int id = int.Parse(node.Value["id"]);
                    string line = "\t"+$"  {key} = {id},"+ '\n';
                    sb.Append(line);
                }
                CreateKeyClass(sb.ToString());
                sb.Clear();
            }
            AssetDatabase.Refresh();
        }
        
        private static void CreateKeyClass(string content)
        {

            string text = @"
//namespace xicheng.module.ui
//{
    /// <summary>
    /// 所有UI界面的路徑Key(自动生成)
    /// </summary>
    public enum UIKey
    {" + '\n'
       
       + content     + '\n'
       +
       @"    }
//}";
            string filePath = MainConst.UIKeyCsCodePath + @"/UIKeys.cs";
            if (File.Exists(filePath))
                File.Delete(filePath);
            File.WriteAllText(filePath, text, Encoding.UTF8);
            AssetDatabase.Refresh();
            ULog.Info("xc.[UIKey] 生成成功！");
        }
    }
}