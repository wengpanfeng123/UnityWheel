using System.IO;
using UnityEditor;

public class ScriptTemplateEditor : AssetModificationProcessor
{
    private static void OnWillCreateAsset(string path)
    {
        if(!path.EndsWith(".cs.meta"))
        {
            return;
        }

        string originalFilePath = AssetDatabase.GetAssetPathFromTextMetaFilePath(path);
        string allText = File.ReadAllText(originalFilePath);
        allText = allText.Replace("#SCRIPTFULLNAME#", Path.GetFileName(path));
        allText = allText.Replace("#AUTHOR#","xicheng");
        allText = allText.Replace("#DATE#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        allText = allText.Replace("#TIPS#","xicheng知识库");
        File.WriteAllText(originalFilePath, allText);
    }
}