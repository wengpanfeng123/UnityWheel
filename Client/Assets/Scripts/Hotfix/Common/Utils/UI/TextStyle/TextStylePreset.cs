// 创建可复用的样式预设

using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/字体样式预设")]
public class TextStylePreset : ScriptableObject
{
    [HideInInspector] public int version = 0; //添加版本控制

    [Header("基础样式")]
    public TMP_FontAsset fontAsset;
    
    [Required("Material不能为空,指定正确的[样式材质]")]
    public Material materialAsset;
    [ReadOnly]public float fontSize = 36;
    [ReadOnly]public int characterSpace = 0;
    public Color fontColor = Color.white;
    public FontStyles fontStyle = FontStyles.Normal;
    public TextAlignmentOptions alignment = TextAlignmentOptions.Center;

    // [Header("描边效果")] public bool useOutline;
    // public float outlineWidth = 0.2f;
    // public Color outlineColor = Color.black;
    //
    // [Header("阴影效果")] public bool useShadow;
    // [ShowIf("useShadow")]public Color shadowColor = Color.white;
    // [ShowIf("useShadow")][Range(-1, 1)] public float offsetX = 1f;
    // [ShowIf("useShadow")][Range(-1, 1)] public float offsetY = 1f;
    // [ShowIf("useShadow")][Range(-1, 1)] public float dilate = 1f;
    // [ShowIf("useShadow")][Range(0, 1)] public float softness = 0;


    [Header("渐变效果")]
    [PropertyTooltip("是否需要渐变")]public bool showGradient;
    [HideInInspector] public GradientMode gradientMode;
    [HideInInspector] public Color topColor = Color.white;
    [HideInInspector] public Color bottomColor = Color.white;
    [HideInInspector] public Color bottomRightColor = Color.white; // 四角渐变专用
    [HideInInspector] public Color topRightColor = Color.white; // 四角渐变专用

    public enum GradientMode
    {
        Single,
        Horizontal,
        Vertical,
        FourCorners
    }

    private TextMeshProUGUI _textComponent;

    // 应用预设到文本组件
    public void ApplyTo(TextMeshProUGUI text)
    {
        _textComponent = text;
        Debug.Log($"ApplyTo---- {_textComponent.name}  {materialAsset.name}");
        text.font = fontAsset;
        //text.fontSize = fontSize; //不应用大小
        text.color = fontColor;
        text.fontStyle = fontStyle;
        //text.characterSpacing = characterSpace;
        // 使用共享材质避免内存泄漏
        text.fontSharedMaterial = materialAsset; 

        ApplyGradient(text);
    }

    
    // private void ApplyShadow(TextMeshProUGUI textComponent)
    // {
    //     if (useShadow)
    //     {
    //         materialAsset.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, offsetX);
    //         materialAsset.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, offsetY);
    //         materialAsset.SetFloat(ShaderUtilities.ID_UnderlayDilate, dilate);
    //         materialAsset.SetFloat(ShaderUtilities.ID_UnderlaySoftness, softness);
    //         materialAsset.SetColor(ShaderUtilities.ID_UnderlayColor, shadowColor);
    //         materialAsset.EnableKeyword("UNDERLAY_ON");
    //     }
    //     else
    //     {
    //         materialAsset.DisableKeyword("UNDERLAY_ON");
    //     }
    //
    //     MarkMaterialModified();
    // }

    private void ApplyGradient(TextMeshProUGUI textComponent)
    {
        textComponent.enableVertexGradient = showGradient;
        if (!showGradient)
        {
            return;
        }
        VertexGradient gradient = new VertexGradient();
        switch (gradientMode)
        {
            case GradientMode.Single:
                gradient = new VertexGradient(topColor, topColor, topColor, topColor);
                break;
            case GradientMode.Horizontal:
                gradient = new VertexGradient(topColor, bottomColor, topColor, bottomColor);
                break;
            case GradientMode.Vertical:
                gradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);
                break;
            case GradientMode.FourCorners:
                gradient = new VertexGradient(
                    topColor,
                    topRightColor,
                    bottomColor,
                    bottomRightColor
                );
                break;
        }
        textComponent.colorGradient = gradient;
    }

    private void MarkMaterialModified()
    {
#if UNITY_EDITOR
        if (materialAsset != null)
        {
            //标记外部材质需要保存
            EditorUtility.SetDirty(materialAsset);
        }
#endif
    }
    
    public void MarkModified()
    {
        version++;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}