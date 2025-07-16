using System;
using cfg;
using Hotfix;
using Hotfix.Model;
using UnityEngine;
using xicheng.aot;
using xicheng.archive;
using xicheng.events;
using xicheng.ui;

public class HotfixEntry : MonoBehaviour
{
    #region 逻辑类访问入口
    
    //1.通过下面的方式访问逻辑类
    //2.通过HotfixEntry.GetLogic<T>()获取逻辑类
    private static ModelManager _model;
    public static ModelManager Model    {
        get
        {
            if (_model == null)
                _model = GetLogic<ModelManager>();
            return _model;
        }
    }
    
    private static UIManager _ui;
    public static UIManager UI{
        get
        {
            if (_ui == null)
                _ui = GetLogic<UIManager>();
            return _ui;
        }
    }
    
    private static DataTableManager _dataTableManager;
    public static Tables DT
    {
        get
        {
            if (_dataTableManager == null)
                _dataTableManager = GetLogic<DataTableManager>();
            return _dataTableManager.Table;
        }
    }
    
    public static Tables Table => DataTableManager.Inst.Table;
 
    #endregion

    private static LogicSystem _logicSystem;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // 初始化游戏存档数据
        GameArchive.OnInit();
        // 初始化逻辑
        _logicSystem = new LogicSystem();
        _logicSystem.InitializeStartupLogics();

    }
    
    void Update()
    {
        _logicSystem?.OnUpdate(Time.deltaTime);
    }


    /// <summary>
    /// 获取框架组件
    /// </summary>
    /// <typeparam name="T">要获取的框架组件类型</typeparam>
    /// <returns>想要获取的组件</returns>
    public static T Get<T>() where T : BaseAotComp
    {
        var comp = AotComponentManager.GetComponent(typeof(T));
        return (T)comp;
    }
    
    public static T GetLogic<T>()  where T : class, ILogic
    {
        var logic = _logicSystem.GetLogic<T>();
        return logic;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Model.SaveData(true);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        Model.SaveData(true);
    }


    private void OnApplicationQuit()
    {
        Model.OnRelease();
        GameArchive.Close();
        
        GameEvent.OnRelease();
        _logicSystem?.OnRelease();
    }
}