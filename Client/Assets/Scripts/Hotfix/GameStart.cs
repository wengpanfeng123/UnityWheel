using System;
using Hotfix;
using Hotfix.Model;
using Hotfix.Module;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this); 
        Application.focusChanged += OnFocusChanged;
    }
    
    private void Start()
    {
          new GameObject("HotfixEntry").AddComponent<HotfixEntry>();
    }

    private void OnFocusChanged(bool obj)
    {
         
    }


}
