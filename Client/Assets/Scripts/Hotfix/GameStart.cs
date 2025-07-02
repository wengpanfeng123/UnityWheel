using Hotfix;
using Hotfix.Model;
using UnityEngine;
using xicheng.events;

public class GameStart : MonoBehaviour
{
    private void Awake()
    { }
    
    void Start()
    {
        GameModel.OnInit();
        GameLogic.OnInit();
    }
    

    // Update is called once per frame
    void Update()
    {
        GameLogic.OnUpdate(Time.deltaTime);
    }

 

    private void OnApplicationQuit()
    {
        GameLogic.OnRelease();
        GameEvent.OnRelease();
        GameModel.ExitGame();
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
    }
}
