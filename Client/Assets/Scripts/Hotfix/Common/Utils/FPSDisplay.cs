using Sirenix.OdinInspector;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    // FPS显示配置
    [Header("显示设置")]
    public bool showFPS = true;
    //public Color textColor = Color.white;
    public int fontSize = 40;
    public Vector2 positionOffset = new(10, 10);

    [LabelText("显示fps底板")]
    public bool showFpsBg = true;
    // FPS计算参数
    private float _updateInterval = 0.1f; // 更新间隔（秒）
    private float _accumulatedFPS = 0f;
    private int _framesCounted = 0;
    private float _timeLeft;
    private float _currentFPS;
    
    // GUI元素
    private GUIStyle _style;
    private GUIStyle _bgStyle;
    private Rect _fpsRect;
    private Rect _bgRect;
    private Texture2D _bgTexture;

    private void Start()
    {
        // 初始化FPS计算
        _timeLeft = _updateInterval;
        
        // 创建背景纹理
        _bgTexture = new Texture2D(1, 1);
        _bgTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.7f));
        _bgTexture.Apply();
        
        // 初始化样式
        UpdateStyles();
    }

    // 更新GUI样式
    private void UpdateStyles()
    {
        _style = new GUIStyle
        {
            fontSize = fontSize,
            //normal = { textColor = textColor },
            alignment = TextAnchor.UpperRight
        };
        
        _bgStyle = new GUIStyle
        {
            normal = { background = _bgTexture }
        };
    }

    private void Update()
    {
        // 更新FPS计算
        _timeLeft -= Time.deltaTime;
        _accumulatedFPS += Time.timeScale / Time.deltaTime;
        _framesCounted++;
        
        // 间隔结束时更新显示的FPS值
        if (_timeLeft <= 0f)
        {
            _currentFPS = _accumulatedFPS / _framesCounted;
            _timeLeft = _updateInterval;
            _accumulatedFPS = 0f;
            _framesCounted = 0;
        }
        
        // 根据FPS值动态改变颜色
        if (_currentFPS > 60)
        {
            _style.normal.textColor = Color.green;
        }
        else if (_currentFPS > 30)
        {
            _style.normal.textColor = Color.yellow;
        }
        else
        {
            _style.normal.textColor = Color.red;
        }
        
        // 计算显示位置和尺寸
        CalculateRects();
    }

    private void CalculateRects()
    {
    
        // 计算文本尺寸
        var content = new GUIContent($"FPS: {_currentFPS:0.}");
        Vector2 textSize = _style.CalcSize(content);
        
        // 设置背景矩形
        float bgWidth = textSize.x + 20; // 增加边距
        float bgHeight = fontSize + 10; // 增加边距
        _bgRect = new Rect(Screen.width - bgWidth - positionOffset.x, positionOffset.y, 
                          bgWidth, bgHeight);
        
        // 设置FPS文本矩形（居中在背景内）
        float textX = _bgRect.x + (bgWidth - textSize.x) / 2;
        float textY = _bgRect.y + (bgHeight - fontSize) / 2;
        _fpsRect = new Rect(textX, textY, textSize.x, textSize.y);
    }

    private void OnGUI()
    {
        if (!showFPS)
            return;
        
        // 检查是否需要更新样式（如编辑器内修改字体大小）
        if (_style.fontSize != fontSize)
        {
            UpdateStyles();
        }
        
        // 绘制背景和文本
        if(showFpsBg)
            GUI.Box(_bgRect, GUIContent.none, _bgStyle);
        GUI.Label(_fpsRect, $"FPS: {_currentFPS:0.}", _style);
    }
    
    private void OnDestroy()
    {
        // 清理资源
        if (_bgTexture != null)
        {
            Destroy(_bgTexture);
        }
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // 在编辑器修改时立即更新样式
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UpdateStyles();
        }
    }
    #endif
}