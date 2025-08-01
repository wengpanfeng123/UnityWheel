using UnityEngine;
using Hotfix;

public class HotfixEntry : MonoBehaviour
{
    
    private void Awake()
    {
        DontDestroyOnLoad(this); 
        
        GameManager.Inst.OnAwake();
    }
    
    private void Start()
    {
        GameManager.Inst.OnStart();
    }
}
