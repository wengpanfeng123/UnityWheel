using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using cfg.ui;
using Xicheng.module.ui;

namespace Xicheng.UI
{
    public enum UIStatus
    {
        None=0,
        StatusLoading = 1,        //加载中
        StatusActive = 2,         //激活中
        StatusHiding = 3,         //隐藏中
        StatusDestroy = 4,        //加载中移除
    }
    
    public abstract partial class UIBase :MonoBehaviour, IUIBase
    {
        private UIKey _uiKey;
        private UIStatus _uiStatus = UIStatus.None;
        protected UILayerType LayerType = UILayerType.MainHUD;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;

        private int _layerId;
        private long _instId; //位唯一实例ID
        private object _args; //onshow参数
        private int _depthOrder;
        private int _depthInterval = 10;
        
        public long RecycleTime; //进入对象池后才会赋值
        
        public UIStatus UIStatus =>_uiStatus;
        public long InstId => _instId;
        public int LayerId
        {
            get => GetCfg().Layer;
            set => _layerId = value;
        }
        
        public int Depth
        {
            get => _depthOrder;
            set
            {
                _depthOrder = value;
                if (_canvas != null)
                    _canvas.sortingOrder = value;
            }
        }
        private static long _instIdCounter = 0;
        public abstract UIKey UIKey { get; }

        protected virtual void Awake()
        {
            _axisEventDic = new Dictionary<UI_Event, UI_Event.AxisEventDelegate>();
            _baseEventDic = new Dictionary<UI_Event, UI_Event.BaseEventDelegate>();
            _pointerEventDic = new Dictionary<UI_Event, UI_Event.PointerEventDelegate>();
            AfterAwake();
        }
        
        protected virtual void AfterAwake()
        {
        }

        public virtual void OnInit(UIKey uiKey)
        {
            _uiKey = uiKey;
            _canvas = gameObject.GetComponent<Canvas>();
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            SetInteractive(true);
            _canvasGroup.alpha = 1;
            _instId = Interlocked.Increment(ref _instIdCounter); //原子递增
            LayerType = (UILayerType)LayerId;
        }
        
        public virtual void OnShow(object args=null)
        {
            _args = args;
            gameObject.SetActive(true);
            SetUIState(UIStatus.StatusActive);
            RecycleTime = -1;
        }
        

        protected void SetInteractive(bool state)
        {
            _canvasGroup.interactable = state;
            _canvasGroup.blocksRaycasts = state;
        }
        
  
        public virtual void OnHide()
        {
            gameObject.SetActive(false);
            SetUIState(UIStatus.StatusHiding);
        }

        public virtual void OnClose()
        {
            gameObject.SetActive(false);
            SetUIState(UIStatus.StatusDestroy);
            ClearEvent();
        }
        
        public void Redisplay()
        {
            gameObject.SetActive(true);
            SetUIState(UIStatus.StatusActive);
            RecycleTime = -1;
        }
        
        public virtual void OnRecycle()
        {
            gameObject.SetActive(false);
            transform.SetParent(UIManager.Inst.UIRoot); // 重置父节点
            transform.localPosition = Vector3.zero; // 重置位置
            RecycleTime = (long)(Time.realtimeSinceStartup * 1000); // 毫秒时间戳
        }
        
        public void DestroyUI()
        {
            if (gameObject != null) 
                Destroy(gameObject);
        }
        
        private void ResetData()
        {
            LayerType = UILayerType.MainHUD;
            _uiStatus = UIStatus.None;
            _canvas = null;
            _canvasGroup = null;
            _depthOrder = -1;
            _layerId = -1;
            _instId = -1; //位唯一实例ID
            ClearEvent();
        }

        public UIPanel GetCfg()
        {
            var panel = UIManager.Inst.GetUICfg(UIKey);
            return panel;
        }

        public object GetShowMethodArgs()
        {
            return _args;
        }

        public void SetUIState(UIStatus uiStatus)
        {
            _uiStatus = uiStatus;
        }
    }
}