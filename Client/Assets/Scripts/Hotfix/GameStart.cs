using System.Collections.Generic;
using Hotfix;
using UnityEngine;
using xicheng.tcp;

public class GameStart : MonoBehaviour
{ 
    private readonly List<IController> _controllers = new();
    
    /* 1.各种模块的初始化。
     * 2.
     */
    void Start()
    {
        RegisterControllers();
        InitCtrl();
        GameController.Inst.EnterGame();
    }
    
    private void RegisterControllers()
    {
       
        _controllers.Add(UserProxy.Instance);
    }
    
    private void InitCtrl()
    {
        foreach (var ctr in _controllers)
            ctr.OnInit();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = _controllers.Count-1; i >=0 ; i--)
        {
            _controllers[i].OnUpdate(Time.deltaTime);
        }
    }

    void OnRelease()
    {
        foreach (var ctr in _controllers)
        {
            ctr.OnRelease();
        }
    }


    private void OnApplicationQuit()
    {
        
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
    }
}
