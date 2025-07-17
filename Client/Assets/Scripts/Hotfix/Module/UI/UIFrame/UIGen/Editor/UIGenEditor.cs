using System.IO;
using UnityEditor;
using UnityEngine;
using Xicheng.Common;

#if UNITY_EDITOR
[CustomEditor(typeof(UIGen))]
public class UIGenEditor : Editor
{
    [MenuItem("Assets/生成UI",false,10)]
    public static void GenByMenu()
    {
        var selectGo = Selection.activeObject as GameObject;
        if (selectGo == null)
            return;
        UIGen uiGen = selectGo.GetComponent<UIGen>();
        if (uiGen == null)
            uiGen = selectGo.AddComponent<UIGen>();
        var prefabPath = AssetDatabase.GetAssetPath(selectGo);
        var fileName = Path.GetFileName(prefabPath);
        if (!fileName.StartsWith("panel") && !fileName.StartsWith("Panel") && !fileName.StartsWith("Com_"))
        {
            Debug.LogError($"只能生成Panel,panel,Com_开头的预制体,fileName:{fileName}");
            return;
        }

        uiGen.Generate(prefabPath);
    }

    private string searchStr = "";

    public override void OnInspectorGUI()
    {
        var comSettings = serializedObject.FindProperty("comSettings");
        if (comSettings.arraySize > 0)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("搜索",GUILayout.MaxWidth(32));
            searchStr = EditorGUILayout.TextField(searchStr);
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginVertical();
        for (var i = 0; i < comSettings.arraySize; i++)
        {
            var comSetting = comSettings.GetArrayElementAtIndex(i);
            var transform = comSetting.FindPropertyRelative("transform");
            if(transform.objectReferenceValue==null)
                continue;
            var transformName = ((Transform) transform.objectReferenceValue).name.ToLower();
            var gen = comSetting.FindPropertyRelative("gen");
            GUILayout.BeginHorizontal();
            if (searchStr.Length > 0 && transformName.Contains(searchStr.ToLower()))
            {
                GUI.backgroundColor = Color.red;
                GUI.color = Color.red;
            }

            else
            {
                GUI.backgroundColor = Color.gray;
                GUI.color = Color.white;
            }

            EditorGUILayout.ObjectField(transform, GUIContent.none);
            EditorGUILayout.PropertyField(gen, GUIContent.none);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
