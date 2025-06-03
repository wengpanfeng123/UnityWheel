using cfg.ui;
using xicheng.common;
using xicheng.module.ui;
using System.Collections.Generic;
using System.Linq;
using Hotfix.DataTable;
using Main.Module.Log;
using UnityEngine;
namespace xicheng.ui
{
    
    [System.Serializable]
    public class LayerConfig
    {
        public UILayerType type;
        public int baseOrder;
        public bool exclusive;
    }

    public class UILayerMgr
    {
        private Dictionary<int, LayerData> _layerDic;
        public UILayerMgr()
        {
            _layerDic = new Dictionary<int, LayerData>();
            GenerateLayerNode();
        }
        

        // 互斥关系配置
        private Dictionary<UILayerType, List<UILayerType>> _exclusionRules = new()
        {
            {
                UILayerType.SystemTop, new List<UILayerType>
                {
                    UILayerType.Function, UILayerType.Popup
                }
            },
            {
                UILayerType.Popup, new List<UILayerType>
                {
                    UILayerType.Function
                }
            }
        };
        
        private void GenerateLayerNode()
        {
            Transform uiRoot = UIManager.Inst.UIRoot;
            foreach (UILayer item in DT.Table.TbUILayer.DataList)
            {
                // 创建一个空的GameObject  
                GameObject emptyNode = new GameObject(item.Id+"_"+item.Name);
                // 如果你想，可以将这个空节点设置为当前脚本所在GameObject的子节点  
                emptyNode.transform.SetParent(uiRoot.transform);
                emptyNode.transform.localPosition = Vector3.zero;
                emptyNode.transform.localScale = Vector3.one;
                emptyNode.AddComponent<Canvas>();
                emptyNode.AddComponent<CanvasRenderer>();
                emptyNode.layer = LayerMask.NameToLayer("UI");
                RectTransform rectTransform = emptyNode.GetComponent<RectTransform>();
                // 1. 设置锚点为四周对齐
                rectTransform.anchorMin = Vector2.zero; // 左下角 (0,0)
                rectTransform.anchorMax = Vector2.one;   // 右上角 (1,1)
                // 2. 强制重置偏移量为0
                rectTransform.offsetMin = Vector2.zero; // 左/下偏移
                rectTransform.offsetMax = Vector2.zero; // 右/上偏移
                _layerDic.Add(item.Id,new LayerData((UILayerType)item.Id, emptyNode));
            }
        }
        
        private LayerData GetLayerData(int layerId)
        {
            return _layerDic.GetValueOrDefault(layerId);
        }


        public void AddToLayer(UIBase panel)
        {
            //添加进对应层级中。
            AddToLayerData(panel);
            //检查互斥关系
            //HandleExclusiveLayers(panel);
        }

        private void AddToLayerData(UIBase panel)
        {
            int layerId = GetLayerId(panel.UIKey);
            LayerData layer = GetLayerData(layerId);
            if (layer ==null)
            {
                Log.Error($"获取层级数据失败！ uikey = {panel.UIKey}");
            }
            layer.AddUI(panel);
        }

        private int GetLayerId(UIKey uiKey)
        {
            return DT.Table.TbUIPanel.Get((int)uiKey).Layer;
        }

 

        private void HandleExclusiveLayers(UIBase newPanel)
        {
            // UILayer layerCfg = Mod.Tb.TbUILayer.Get(newPanel.LayerId);
            // if (layerCfg == null)
            // {
            //     Log.Error($"[HandleExclusiveLayers]不存在层级数据！ uikey = {newPanel.UIKey}");
            //     return;
            // }
            //
            // var exclusiveLayers = layerCfg.ExclusiveLayers;
            // var _globalOrderList = UIManager.Inst._globalOrderList;
            // // 动态记录隐藏关系
            // var currentNode = _globalOrderList.Last;
            // currentNode.Value.HiddenChildren = _globalOrderList
            //     .Where(n => exclusiveLayers.Contains(n.Instance.Layer))
            //     .Select(n => n.Instance.UIKey)
            //     .ToList();
            //
            // // 执行隐藏
            // foreach (var key in currentNode.Value.HiddenChildren) {
            //     if (_instancesDict.TryGetValue(key, out var stack)) {
            //         foreach (var ui in stack) {
            //             ui.gameObject.SetActive(false);
            //         }
            //     }
            // }
        }

        public void OnRemoveUI(UIBase panel)
        {
            int layerId = GetLayerId(panel.UIKey);
            LayerData layer = GetLayerData(layerId);
            if (layer ==null)
            {
                Log.Error($"[OnRemoveUI]获取层级数据失败！ uikey = {panel.UIKey}");
                return;
            }
            layer.RemoveUI(panel);
        }
        
        
        //动态提层：运行时动态修改UI的层级
        public void ChangeUILayer(UIBase ui, int newLayerId) {
            // 从原层级移除
            var oldLayer = GetLayerData(ui.LayerId);
            oldLayer?.RemoveUI(ui);
            // 更新配置
            ui.LayerId = newLayerId;
            // 加入新层级
            var newLayer = GetLayerData(newLayerId);
            newLayer.AddUI(ui);
    
            // 处理互斥关系
            UIManager.Inst.HandleExclusive(ui);
        }

        private void UpdateInputBlock()
        {
            // bool blockLower = false;
            // for (int i = _activeLayers.Count - 1; i >= 0; i--)
            // {
            //     UIBase panel = _activeLayers[i];
            //     CanvasGroup group = panel.GetComponent<CanvasGroup>();
            //
            //     group.blocksRaycasts = !blockLower;
            //     blockLower |= GetConfig((UILayerType)panel.LayerId).exclusive;
            // }
        }

        public void BringToTop(UIBase panel)
        {
            int layerId = GetLayerId(panel.UIKey);
            LayerData layer = GetLayerData(layerId);
            if (layer ==null)
            {
                Log.Error($"[OnRemoveUI]获取层级数据失败！ uikey = {panel.UIKey}");
                return;
            }
            layer.BringToTop(panel);
        }
    }
}