using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif
using TMPro;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace HsJam.TextStyle
{
    [CustomEditor(typeof(TextStylePreset))]
    public class TextStylePresetEditor : OdinEditor
    {
        [ValueDropdown(nameof(GetAllPresetOptions))]
        private TextStylePreset _preset;

        private bool _autoApply = true;
        private readonly List<StyledText> _affectedTexts = new();

        protected override void OnEnable()
        {
            _preset = target as TextStylePreset;
            FindAffectedTexts();
        }

        public override void OnInspectorGUI()
        {
            // 自动应用选项
            _autoApply = EditorGUILayout.Toggle("实时应用更改", _autoApply);

            // 显示影响范围
            EditorGUILayout.LabelField($"影响文本: {_affectedTexts.Count}个");

            // 修改前记录版本
            // int oldVersion = _preset.version;

            // 绘制默认UI
            base.OnInspectorGUI();

            //渐变设置
            DrawGradient();

            if (GUI.changed)
            {
                _preset.MarkModified();

                // 自动应用更改
                if (_autoApply)
                {
                    ApplyChanges();
                }
            }
        }

        private void ApplyChanges()
        {
            if (_preset?.materialAsset == null)
            {
                Debug.LogError("材质资产未分配！");
                return;
            }

            // 获取当前预设的材质ID（用于比较变化）
            var currentPresetMaterialID = _preset.materialAsset.GetInstanceID();
            foreach (var text in _affectedTexts)
            {
                if (text == null)
                    continue; // 防止对象被销毁

                var tmpComponent = text.GetComponent<TextMeshProUGUI>();
                if (tmpComponent == null)
                    continue;

                // 仅当材质实际变化时才重置实例
                bool materialChanged = currentPresetMaterialID != _preset.materialAsset.GetInstanceID();

                if (materialChanged ||
                    tmpComponent.fontSharedMaterial == null ||
                    tmpComponent.fontSharedMaterial != _preset.materialAsset)
                {
                    // 使用新实例避免材质污染
                    tmpComponent.fontSharedMaterial = null;
                    tmpComponent.fontMaterial = _preset.materialAsset;
                }

                // 应用预设更新
                text.ApplyPreset();
                EditorUtility.SetDirty(text);
            }

            // 延迟刷新界面
            EditorApplication.delayCall += () =>
            {
                SceneView.RepaintAll();
                EditorApplication.QueuePlayerLoopUpdate(); //更新游戏循环

                // 强制刷新Inspector
                ActiveEditorTracker.sharedTracker.ForceRebuild();
            };
        }

        private void FindAffectedTexts()
        {
            //通过 GUID 而非引用比较，解决预设克隆体或子资产的识别问题。
            _affectedTexts.Clear();
            var presetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_preset));
            var allTexts = FindObjectsOfType<StyledText>();
            foreach (var text in allTexts)
            {
                var textPresetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(text.stylePreset));
                if (textPresetGuid == presetGuid)
                {
                    _affectedTexts.Add(text);
                }
            }
        }

        private void DrawGradient()
        {
            if (!_preset.showGradient)
            {
                return;
            }

            // 渐变模式选择
            _preset.gradientMode = (TextStylePreset.GradientMode)EditorGUILayout.EnumPopup(
                "渐变模式",
                _preset.gradientMode);

            // 根据渐变模式显示不同的颜色字段
            switch (_preset.gradientMode)
            {
                case TextStylePreset.GradientMode.Single:
                    _preset.topColor = EditorGUILayout.ColorField("颜色", _preset.topColor);
                    break;

                case TextStylePreset.GradientMode.Horizontal:
                    _preset.topColor = EditorGUILayout.ColorField("左侧颜色", _preset.topColor);
                    _preset.bottomColor = EditorGUILayout.ColorField("右侧颜色", _preset.bottomColor);
                    break;

                case TextStylePreset.GradientMode.Vertical:
                    _preset.topColor = EditorGUILayout.ColorField("顶部颜色", _preset.topColor);
                    _preset.bottomColor = EditorGUILayout.ColorField("底部颜色", _preset.bottomColor);
                    break;

                case TextStylePreset.GradientMode.FourCorners:
                    _preset.topColor = EditorGUILayout.ColorField("左上颜色", _preset.topColor);
                    _preset.bottomColor = EditorGUILayout.ColorField("左下颜色", _preset.bottomColor);
                    _preset.topRightColor = EditorGUILayout.ColorField("右上颜色", _preset.topRightColor);
                    _preset.bottomRightColor = EditorGUILayout.ColorField("右下颜色", _preset.bottomRightColor);
                    break;
            }
        }

        private ValueDropdownList<TextStylePreset> GetAllPresetOptions()
        {
            var dropdownList = new ValueDropdownList<TextStylePreset>();


            // 在编辑器中查找所有预设
            var presetGUIDs = AssetDatabase.FindAssets("t:TextStylePreset");
            foreach (var guid in presetGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var preset = AssetDatabase.LoadAssetAtPath<TextStylePreset>(path);

                // 添加到带有图标的下拉选项
                dropdownList.Add($"{preset.name} [Preset]", preset);
            }

            // 添加特殊选项或处理空值情况
            if (dropdownList.Count == 0)
            {
                dropdownList.Add("(No Presets Found)", null);
            }

            return dropdownList;
        }
    }
}
#endif