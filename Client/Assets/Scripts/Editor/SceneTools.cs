//- Created:       #AuthorName#
// - CreateTime:      #CreateTime#
// - Description:   

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneTools  
{
    [MenuItem("Scene/Launcher-AOT",false,1)]
    static void OpenLauncher()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/GameLauncher.unity");
    }
        
    [MenuItem("Scene/GameStart-HotUpdate",false,5)]
    static void OpenGameStart()
    {
        EditorSceneManager.OpenScene("Assets/AssetsPackage/Scenes/GameStart.unity");
    }
    
    
    [MenuItem("Scene/拼UI场景",false,10)]
    static void openForrestScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/拼UI场景.unity");
    }
    
}
