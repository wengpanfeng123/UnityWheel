using UnityEngine;
using System.Collections.Generic;
using cfg.ui;
using Hotfix.DataTable;

namespace xicheng.ui
{
    public class LayerData
    {
        public UILayerType LayerType;
        public GameObject LayerNode;
        public Canvas LayerCanvas;
        
        private List<UIBase> _uiQueues;
        private List<UIKey> _hideUIs = new();
        
        public UILayer UILayer=> DT.Table.TbUILayer.Get((int)LayerType);
        
        public LayerData(UILayerType type, GameObject layerNode)
        {
            _uiQueues = new List<UIBase>();
            LayerType = type;
            LayerNode = layerNode;
            LayerCanvas = layerNode.GetComponent<Canvas>();
        }

        public void AddUI(UIBase panel)
        {
            panel.transform.SetParent(LayerNode.transform);
            panel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            panel.transform.localScale = Vector2.one;
            
            //升序排列。
            _uiQueues.Sort((p1, p2) => p1.Depth.CompareTo(p2.Depth));
            _uiQueues.Add(panel);
            //设置深度
            CalcRenderOrder(panel);
            // 保证在目标层级内置顶
            panel.transform.SetAsLastSibling(); 
        }

        public void RemoveUI(UIBase panel)
        {
            if (!_uiQueues.Contains(panel))
                return;
            _uiQueues.Remove(panel);
            RecalculateDepths();
        }

        
        private void CalcRenderOrder(UIBase panel,int depthStep = 10)
        {
            var maxDepth = GetMaxDepth();
            if (maxDepth + depthStep <= UILayer.DepthRange[1])
            {
                panel.Depth = maxDepth + depthStep;
            }
            else
            {
                // 重新分配所有UI的深度
                int baseDepth = UILayer.DepthRange[0];
                foreach (var ui in _uiQueues)
                {
                    ui.Depth = baseDepth;
                    baseDepth += depthStep;
                }
                // 设置新面板的深度
                panel.Depth = baseDepth;
            }
            // 确保深度不超过最大值
            panel.Depth = Mathf.Min(panel.Depth, UILayer.DepthRange[1]);
        }
        
        public void RecalculateDepths(int step = 10) 
        {
            _uiQueues.Sort((a, b) => a.Depth.CompareTo(b.Depth));
            int baseDepth = UILayer.DepthRange[0];
            foreach (var ui in _uiQueues) {
                ui.Depth = baseDepth;
                baseDepth += step;
            }
        }
        
        //当前层下，UI的最大深度值。
        private int GetMaxDepth()
        {
            return _uiQueues.Count == 0 ? UILayer.DepthRange[0] : _uiQueues[^1].Depth;;
        }


        public List<UIKey> Hide()
        {
            _hideUIs.Clear();
            LayerCanvas.enabled = false;
            for (int i = _uiQueues.Count - 1; i >= 0; i++)
            {
                var panel = _uiQueues[i];
                if (panel == null || panel.UIStatus == UIStatus.StatusHiding)
                    continue;
                _hideUIs.Add(panel.UIKey);
                UIManager.Inst.HideUI(panel);
            }

            return _hideUIs;
        }

        public void Show()
        {
            LayerCanvas.enabled = true;
        }

        public void BringToTop(UIBase panel)
        {
            panel.transform.SetAsLastSibling();
        }
    }

}