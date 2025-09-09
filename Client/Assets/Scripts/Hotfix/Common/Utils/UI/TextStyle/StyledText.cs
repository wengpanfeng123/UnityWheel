// 文本组件绑定预设

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace HsJam.TextStyle
{
    [ExecuteInEditMode]
    public class StyledText : MonoBehaviour
    {
        [OnValueChanged("OnStylePresetChange")]
        public TextStylePreset stylePreset;

        private TextMeshProUGUI textComponent;
        private int lastPresetVersion;

        void OnEnable()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            ApplyPreset();
            EditorApplication.update += EditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            // 编辑器模式下检测预设变更
            if (!Application.isPlaying && stylePreset != null)
            {
                if (stylePreset.version != lastPresetVersion)
                {
                    ApplyPreset();
                    lastPresetVersion = stylePreset.version;
                }
            }
        }

        void OnStylePresetChange(TextStylePreset preset)
        {
            if (preset != stylePreset && preset != null)
            {
                // textComponent.fontMaterial = preset.materialAsset;
                textComponent.fontSharedMaterial = preset.materialAsset;
                EditorUtility.SetDirty(textComponent);
                Debug.Log("OnStylePresetChange----" + textComponent.fontSharedMaterial.name);
            }
        }


        [Button("(材质不正确)手动应用材质")]
        public void ApplyMat()
        {
            textComponent.fontMaterial = stylePreset.materialAsset;
            Debug.Log($"{textComponent.fontSharedMaterial.name}");
        }


        public void ApplyPreset()
        {
            if (stylePreset != null && textComponent != null)
            {
                // 释放旧材质引用
                textComponent.fontSharedMaterial = null;
                // 应用新预设
                stylePreset.ApplyTo(textComponent);
                lastPresetVersion = stylePreset.version;
            }
        }
    }
}
#endif