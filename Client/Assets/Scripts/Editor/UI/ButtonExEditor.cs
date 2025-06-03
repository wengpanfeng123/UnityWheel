//- Created:       wjs
// - CreateTime:      2021/07/09 11:07:05
// - Description:   


namespace UnityEditor.UI
{//CustomEditor(typeof(ButtonEx), true)]
      
    [CanEditMultipleObjects]
    public class ButtonExEditor: SelectableEditor
    {
     
        private SerializedProperty m_OnClickProperty;
        private SerializedProperty m_KeyCode;
        protected override void OnEnable()
        { 
            base.OnEnable();
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_KeyCode = serializedObject.FindProperty("ActionKeyCode");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            this.serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_OnClickProperty);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_KeyCode);
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
