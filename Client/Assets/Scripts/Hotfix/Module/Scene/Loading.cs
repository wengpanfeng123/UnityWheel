using System.Text;
using Xicheng.Secne;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textProgress;
    private StringBuilder _builder;

    private AsyncOperationHandle<SceneInstance> _sceneAsync; 
    private string _assetPath;
    private LoadSceneMode _loadSceneMode;
    private bool _activeOnLoad;
    private bool _loadFinish;
    private GameScene  _gameScene;
    private void Awake()
    {
        _builder = new StringBuilder();
        _gameScene = GameScene.Inst;
    }

    void Start()
    {
        _assetPath = GameScene.Inst.TargetScenePath;
        _loadSceneMode = GameScene.Inst.LoadSceneMode;
        _activeOnLoad = GameScene.Inst.ActiveOnLoad;
        _loadFinish = false;
        LoadScene();
    }

    private void LoadScene()
    {
        if (string.IsNullOrEmpty(_assetPath))
        {
            Debug.LogError("[Loading]未设置目标场景路径");
            return;
        }

        _sceneAsync = Addressables.LoadSceneAsync(_assetPath, _loadSceneMode, _activeOnLoad);
        // 注册进度回调
        _sceneAsync.Completed += OnSceneLoaded;
        // 更新进度条
        UpdateProgress(0f);
    }
    
    
    private void UpdateProgress(float progress)
    {
        // 更新UI显示
        if (slider != null)
        {
            slider.value = progress;
        }
        
        Debug.Log($"场景加载进度: {progress * 100f}%");
    }
    
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ULog.Info($"场景{_gameScene.TargetScenePath}加载成功");
        }
        else
        {
            ULog.Error($"场景{_gameScene.TargetScenePath}加载失败: {handle.OperationException}");
        }

        _loadFinish = true;
        
        // 释放句柄
        Addressables.Release(handle);
    }
    
 
    void Update()
    {
        if (slider == null ||  _loadFinish)
            return;
        _builder.Clear();
        float value = _sceneAsync.PercentComplete;
        slider.value = value;
        textProgress.text = _builder.Append(value*100).ToString();
        ULog.Info($"场景加载进度: {value * 100f}%");
    }

    private void OnDestroy()
    {
        ULog.Info("Loading OnDestroy");

        // 确保释放资源
        if (_sceneAsync.IsValid())
        {
            Addressables.Release(_sceneAsync);
        }
        _builder = null;
        _assetPath = null;
    }
}
