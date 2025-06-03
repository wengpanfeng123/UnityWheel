//- Created:       wjs
// - CreateTime:      2021/10/18 15:03:27
// - Description:   


using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Hotfix;

//[CustomEditor(typeof(TextEx))]
public class TextExEditor : Editor
{
    private SerializedProperty m_LLKEY;

    protected void OnEnable()
    {
        m_LLKEY = serializedObject.FindProperty("m_LanguageKey");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
       
        // EditorGUI.LabelField(Rect.zero,"efefefefefe");
        // EditorGUILayout.PropertyField(m_LanguageKey);
    }
}
