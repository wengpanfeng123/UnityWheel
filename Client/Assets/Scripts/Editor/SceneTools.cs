//- Created:       #AuthorName#
// - CreateTime:      #CreateTime#
// - Description:   

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneTools  
{
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
