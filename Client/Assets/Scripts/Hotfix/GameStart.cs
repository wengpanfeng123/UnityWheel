using System.Collections;
using System.Collections.Generic;
using Hotfix.Module;
using UnityEngine;
using xicheng.tcp;

public class GameStart : MonoBehaviour
{
    private readonly List<IController> _controllers = new();
    
    
    void Start()
    {
        RegisterControllers();

        InitCtrl();
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
        for (int i = _controllers.Count; i >=0 ; i--)
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
}
