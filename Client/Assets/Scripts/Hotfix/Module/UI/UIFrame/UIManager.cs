using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cfg.ui;
using DG.Tweening;
using Xicheng.DataTable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Xicheng.Utility;

namespace Xicheng.UI
{
    public partial class UIManager : MonoSingleton<UIManager>
    {
        private Camera _uiCamera;
        private Transform _uiRoot;
        private List<UIKey> _loadingList;
        private UILayerMgr _layerMgr;
        private UIBase _activityUI;
        
        #region 属性
        public Transform UIRoot => _uiRoot;
        public Camera UICam => _uiCamera;
        #endregion
        
        // 新方案  新增成员变量
        // 按类型管理实例栈（支持多实例）
        private Dictionary<UIKey, Stack<UIBase>> _instancesDict;
        // 全局打开顺序链表（解决循环检测）
        private LinkedList<UIStateNode> _globalOrderList;

        public bool InitStartUp => true;

        public void OnStartUp()
        {
            InitData();
            StartCacheCleanupCoroutine();
        }


        private void InitData()
        {
            _uiRoot = GameObject.Find("UIRoot").transform;
            _uiCamera = _uiRoot.Find("UICamera").GetComponent<Camera>();
            _instancesDict = new();
            _globalOrderList = new();
            _layerMgr = new UILayerMgr();
            _loadingList = new List<UIKey>();
        }

        /// <summary>
        /// 打开UI（异步）
        /// </summary>
        /// <param name="args"></param>
        /// <param name="onComplete"></param>
        /// <typeparam name="T"></typeparam>
        public void OpenUI<T>(object args = null,Action<T> onComplete =null) where T : UIBase
        {
            StartCoroutine(Open(args,onComplete));
        }

        private IEnumerator Open<T>(object args = null,Action<T> onComplete =null) where T : UIBase
        {
            UIKey uiKey = Type2Key<T>();
            var uiConfig = GetUICfg(uiKey);

            bool isMultiInstance = uiConfig.IsMultiInstance;
            
            //如果是单实例。UI多次被打开处理方案：UI循环检测：A->B->C->A.--->置顶栈。B-C-A.
            /*  直接忽略
             *  关闭后重新打开 
             *  提升到栈顶等--->我们采用这种方案     
             * 
             */
            if (!isMultiInstance)
            {
                var existingNode = _globalOrderList.FirstOrDefault(n => n.Instance.UIKey == uiKey);
                //是否被打开过。
                if (existingNode != null)
                {
                    // 1. 移动节点到栈顶
                    _globalOrderList.Remove(existingNode);
                    _globalOrderList.AddLast(existingNode);
                    existingNode.UpdateOpenTime();
                    // 2. 刷新数据
                    existingNode.Instance.OnShow(args);
                    _activityUI = existingNode.Instance;

                    // 3. 重新计算互斥层级
                    HandleExclusive(existingNode.Instance);

                    // 4. 强制置顶显示层级
                    _layerMgr.BringToTop(existingNode.Instance);

                    onComplete?.Invoke((T)existingNode.Instance);
                    yield break;
                }
            }

            //加载中不再创建
            if (_loadingList.Contains(uiKey))
            {
                ULog.Info($"{uiKey} is loading");
                yield break;
            }

            UIBase instance = null;
            // 从池获取或创建
            if (!_instancesDict.TryGetValue(uiKey, out var stack))
            {
                stack = new Stack<UIBase>();
                _instancesDict[uiKey] = stack;
            }

            if (stack is { Count: > 0 })
            {
                instance = stack.Pop();
                instance.RecycleTime = -1;
            }
            else
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(uiConfig.Path);
                yield return handle;
                var uiGameObject = Instantiate(handle.Result);
                //释放句柄
                Addressables.Release(handle); //TODO:
                instance = uiGameObject.GetComponent<T>();
                if (instance == null)
                    instance = uiGameObject.AddComponent<T>();
            }

            if (instance == null)
            {
                ULog.Error($"{uiKey} instance is null。实例化失败!" + uiConfig.Path);
                yield break;
            }
            
            var newNode = new UIStateNode {
                Instance = instance,
                HiddenChildren = new List<UIKey>(),
                OpenTime = DateTime.Now
            };
            _globalOrderList.AddLast(newNode); //维护全局顺序
            
            instance.OnInit(uiKey); 
            //添加到层管理器
            _layerMgr.AddToLayer(instance); 
            instance.OnShow(args);  
            _loadingList.Remove(uiKey);
            _activityUI = instance; //设置为活动界面
            onComplete?.Invoke((T)instance);
            // 处理互斥层级
            HandleExclusive(instance); 
        }
        
        public void HandleExclusive(UIBase newPanel)
        {
            UILayer layerCfg = DT.Table.TbUILayer.Get(newPanel.LayerId);
            if (layerCfg == null)
            {
                ULog.Error($"[HandleExclusiveLayers]不存在层级数据！ uikey = {newPanel.UIKey}");
                return;
            }
            
            //处理互斥层级中激活状态的UI
            var exclusiveLayers = layerCfg.ExclusiveLayers;
            var currentNode = _globalOrderList.Last;
            currentNode.Value.HiddenChildren = new List<UIKey>();
            foreach (var node in _globalOrderList.Where(n => n.Instance.UIStatus == UIStatus.StatusActive))
            {
                //层级互斥 和同层互斥
                bool isExclusive = exclusiveLayers.Contains(node.Instance.LayerId);
                bool isSameLayerExclusive = false;
                if (!isExclusive)
                {
                    isSameLayerExclusive = node.Instance.LayerId == newPanel.LayerId
                                           && newPanel.GetCfg().MutexRule == (int)UIMutexRule.Mutex
                                           && node.Instance.GetCfg().IgnoreMutex;
                }
                //处理互斥内容
                if (isExclusive || isSameLayerExclusive)
                {
                    currentNode.Value.HiddenChildren.Add(node.Instance.UIKey);
                    node.Instance.OnHide();
                }
            }
        }
        
        //依然保留栈中数据，只是被隐藏而已。
        public void HideUI(UIBase instanceUI)
        {
            if (_globalOrderList.Count <= 0)
                return;
            // 使用LINQ检查是否存在
            bool containsUI = _globalOrderList.Any(n => n.Instance == instanceUI);
            if (containsUI)
            {
                instanceUI.OnHide(); 
            }
        }

        public void CloseUI(UIBase instanceUI)
        {
            if (_globalOrderList.Count <= 0)
            {
                ULog.Error("[UIMgr] CloseUI 打开列表为空");
                return;
            }
            UIStateNode node = _globalOrderList.FirstOrDefault(n => n.Instance == instanceUI);
            if (node != null)
            {
                UIClose(node);
                if(_loadingList.Contains(node.Instance.UIKey))
                    _loadingList.Remove(node.Instance.UIKey); 
            }
        }

        //关闭目标界面。
        //关闭操作不会触发恢复操作。
        //目前的方案来说，如果关闭中间界面，上层界面继续保持原来状态
        //支持关闭单实例和多实例。如果要关闭多实例类型UI，传入uiBase实例即可
        public void CloseUI<T>(UIBase instanceUI = null) where T : UIBase
        {
            if (_globalOrderList.Count <= 0)
            {
                ULog.Error("[UIMgr] CloseUI 打开列表为空"); // 修改：改为记录错误日志
                return;
            }

            UIKey uiKey = Type2Key<T>();
            var uiCfg = GetUICfg(uiKey);
            if (uiCfg == null)
            {
                ULog.Error($"[CloseUI] UI配置表 id =  {uiKey} not found!");
                return;
            }

            bool isMulti = uiCfg.IsMultiInstance;
            UIStateNode closeUINode = null;
            if (!isMulti)
            {
                closeUINode = _globalOrderList.FirstOrDefault(n => n.Instance.UIKey == uiKey);
            }
            else
            {
                if (instanceUI == null)
                {
                    ULog.Error($"[CloseUI] {uiKey}是多实例UI，请传入正确的InstId参数！"); // 修改：改为记录错误日志
                    return;
                }
                closeUINode = _globalOrderList.FirstOrDefault(n => n.Instance.InstId == instanceUI.InstId);
            }

            if (closeUINode != null)
            {
                UIClose(closeUINode);
                _loadingList.Remove(closeUINode.Instance.UIKey); 
            }
        }
        
   
        //回退逻辑
        // backUI操作只会从栈顶取出topUI,如果topUI和backUI是同一个，则执行回退操作。如果不是同一个(原因：操作不规范，可能调用了CloseUI,)，则不执行回退操作,回退失败。
        public void BackUI(UIBase ui)
        {
            if (_globalOrderList.Count <= 1) //至少存在一个主UI
                return;
            
            // 当前节点（顶部）
            var current = _globalOrderList.Last.Value;
            
            if (ui != current.Instance)
            {
                ULog.Warning($"回退失败，当前{ui.UIKey}不是顶部UI，请检查逻辑！");
                return;
            }
            
            UIClose(current);
            
            //显示上个节点。
            LinkedListNode<UIStateNode> prevNode = _globalOrderList.Last.Previous;
            
            if (prevNode != null && prevNode.Value.HiddenChildren.Count > 0) 
            {
                foreach (var key in prevNode.Value.HiddenChildren) 
                {
                    var node = _globalOrderList.FirstOrDefault(n => n.Instance.UIKey == key);
                    if (node != null && node.Instance.UIStatus == UIStatus.StatusHiding)
                    {
                        node.Instance.Redisplay();
                    }
                    else
                    {
                        ULog.Warning($"UIKey {key} 已被关闭或不存在");
                    }
                }
            }
        }
        
        private void UIClose(UIStateNode node)
        {
            if (node != null && node.Instance != null)
            {
                node.Instance.OnClose();
                node.HiddenChildren.Clear();
                _layerMgr.OnRemoveUI(node.Instance);
                Recycle(node.Instance); //回收UI
                _globalOrderList.Remove(node);
            }
        }
        
        // 获取当前顶层UI
        public UIStateNode GetTopUI()
        {
            return _globalOrderList.Count > 0 ? _globalOrderList.Last.Value : null;
        }
        
       
        //切场景时UI处理
        public void OnSceneChange()
        {
            List<UIStateNode> nodesToRemove = new List<UIStateNode>();
            //遍历标识哪些UI需要移除。IsPersistent-持久化显示
            foreach (var node in _globalOrderList)
            {
                // 获取层级配置，假设UILayer配置新增IsPersistent字段
                var cfg =  GetUICfg(node.Instance.UIKey);
                if (cfg is { IsResident: false })
                {
                    nodesToRemove.Add(node);
                }
            }
            // 逆序关闭以避免修改遍历中的链表
            foreach (var node in nodesToRemove.OrderByDescending(n => n.OpenTime))
            {
                CloseUI(node.Instance);
            }
        }
        
        
        public UIPanel GetUICfg(UIKey key)
        {
            return DT.Table.TbUIPanel.Get((int)key);
        }

        private UIKey Type2Key<T>() where T:UIBase
        {
            string input = typeof(T).Name;
            UIKey uiKey = (UIKey)Enum.Parse(typeof(UIKey), input);
            return uiKey;
        }

        //入场动画
        private IEnumerator PlayOpenAnim(UIBase ui,object args)
        {
            // 实现你的入场动画逻辑
            ui.transform.localScale = Vector3.zero;
            yield return ui.transform.DOScale(Vector3.one, 0.2f).WaitForCompletion();
            ui.OnShow(args);
        }
        
        //退场动画
        private IEnumerator PlayExitAnim(UIBase ui)
        {
            // 实现你的退场动画逻辑 
            yield return ui.transform.DOScale(Vector3.zero, 0.2f).WaitForCompletion();
            ui.OnHide();
        }
        
        private int _uiIndex=0;
        public int UIIndex => _uiIndex++;
        
        public UILayerMgr LayerMgr => _layerMgr;

        public IEnumerable<UIStateNode> GetAllUIStateNodes()
        {
            return new List<UIStateNode>(_globalOrderList);
        }

        private void OnDestroy()
        {

        }

        public void OnRelease()
        {
            _instancesDict.Clear();
            _globalOrderList.Clear();
            _globalOrderList = null;
            _instancesDict = null;
        }
    }
}
