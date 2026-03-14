using UnityEngine;
using Hotfix;

public class HotfixEntry : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 120;
        //在对话中时UI建议使用
        //UnityEngine.Rendering.OnDemandRendering.renderFrameInterval = n(将渲染降低至原有的1/n) 对渲染进行降频
        GameManager.Inst.OnAwake();
    }
    
    private void Start()
    {
        GameManager.Inst.OnStart();
    }
}
