// UIDebugWindow.cs

#if UNITY_EDITOR


using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Xicheng.UI;

public class UIDebugWindow : OdinEditorWindow
{
    [FoldoutGroup("Style Settings", Order = 99)]
    [ColorPalette("StatusColors")]
    public Color ActiveColor = new Color(0.3f, 0.8f, 0.3f); // 绿色

    [ColorPalette("StatusColors")]
    public Color HidingColor = new Color(0.9f, 0.8f, 0.1f); // 黄色

    [ColorPalette("StatusColors")]
    public Color DestroyColor = new Color(0.8f, 0.2f, 0.2f); // 红色
    
    
    [ShowInInspector]
    [TableList(ShowIndexLabels = true)]
    public List<UIDebugInfo> UIDebugInfos { get; private set; } = new();

    [MenuItem("Window/UI Debugger")]
    private static void OpenWindow()
    {
        GetWindow<UIDebugWindow>("UI Debugger").Show();
        // ObjectPool<UIBase> pool = new ObjectPool<UIBase>( OnCrate);
        // pool.Get(out var ui);
        // pool.Release(ui);
        // pool.Clear();
        // pool.Dispose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // 每帧更新数据
        EditorApplication.update += UpdateData;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EditorApplication.update -= UpdateData;
    }

    private void UpdateData()
    {
        if (Application.isPlaying && UIManager.Inst != null)
        {
            UIDebugInfos = UIManager.Inst.GetAllUIStateNodes()
                .Select(node => new UIDebugInfo(node))
                .ToList();
        }
        else
        {
            UIDebugInfos.Clear();
        }
    }

    [System.Serializable]
    public class UIDebugInfo
    {
        [DisplayAsString]
        [TableColumnWidth(120)]
        public UIKey UIKey;

        [DisplayAsString]
        [TableColumnWidth(80)]
        public long InstId;

        [TableColumnWidth(100)]
        [GUIColor(nameof(StatusColor))] // 正确属性名称
        public UIStatus Status;

        [DisplayAsString]
        [TableColumnWidth(100)]
        public UILayerType Layer;

        [DisplayAsString]
        [TableColumnWidth(150)]
        public string OpenTime;

        // 状态颜色映射逻辑
        private Color StatusColor => Status switch
        {
            UIStatus.StatusActive => new Color(0.3f, 0.8f, 0.3f), // 绿色
            UIStatus.StatusHiding => new Color(0.9f, 0.8f, 0.1f),  // 黄色
            UIStatus.StatusDestroy => new Color(0.8f, 0.2f, 0.2f),// 红色
            _ => Color.gray
        };

        public UIDebugInfo(UIStateNode node)
        {
            UIKey = node.Instance._UIKey_;
            InstId = node.Instance.InstId;
            Status = node.Instance.UIStatus;
            Layer = (UILayerType)node.Instance.LayerId;
            OpenTime = node.OpenTime.ToString("HH:mm:ss");
        }
    }
}
#endif