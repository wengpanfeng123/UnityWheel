using Hotfix.Module.Scene;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    public string ScenePath1 = "Assets/Scenes/TestScene1.unity";
    public string ScenePath2 = "Assets/Scenes/TestScene2.unity";
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "Loading-scene"))
        {
            GameScene.Inst.LoadingSceneAsync(ScenePath2);
        }
        
        if (GUI.Button(new Rect(0, 105, 200, 100), "load scene"))
        {
            GameScene.Inst.LoadSceneAsync(ScenePath1);
        }
    }

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
