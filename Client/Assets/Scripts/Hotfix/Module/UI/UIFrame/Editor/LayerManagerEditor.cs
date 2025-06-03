using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using xicheng.ui;

namespace xicheng.ui
{
#if UNITY_EDITOR
   // [CustomEditor(typeof(LayerManager))]
    public class LayerManagerEditor : UnityEditor.Editor
    {
        // public override void OnInspectorGUI()
        // {
        //     base.OnInspectorGUI();
        //
        //     if (GUILayout.Button("Generate Default Layers"))
        //     {
        //         LayerManager manager = (LayerManager)target;
        //         manager.LayerConfigs = new List<LayerManager.LayerConfig>
        //         {
        //             NewConfig(UILayerType.Debug, 0, false),
        //             NewConfig(UILayerType.Background, 100, false),
        //             NewConfig(UILayerType.MainHUD, 200, true),
        //             NewConfig(UILayerType.Function, 300, true),
        //             NewConfig(UILayerType.Popup, 400, true),
        //             NewConfig(UILayerType.Toast, 500, false),
        //             NewConfig(UILayerType.Animation, 1000, false),
        //             NewConfig(UILayerType.Cinema, 2000, true),
        //             NewConfig(UILayerType.SystemTop, 3000, true)
        //         };
        //     }
        // }
        //
        // private LayerManager.LayerConfig NewConfig(UILayerType type, int order, bool exclusive)
        // {
        //     return new LayerManager.LayerConfig
        //     {
        //         type = type,
        //         baseOrder = order,
        //         exclusive = exclusive
        //     };
        // }
    }
#endif
}