using Sirenix.OdinInspector;
using UnityEngine;

public class RedPointTest : MonoBehaviour
{
    void Start()
    {
        // 创建节点层级
        // RedPointSystem.RegisterNode(RedPointPath.Main);
        // RedPointSystem.RegisterNode(RedPointPath.Main_Tasks, RedPointPath.Main);
        // RedPointSystem.RegisterNode(RedPointPath.Main_Tasks_Daily, RedPointPath.Main_Tasks);
        // RedPointSystem.RegisterNode(RedPointPath.Main_Tasks_Achie, RedPointPath.Main_Tasks);
        
    } 
 
    [Button("SetMain")]
    public void Test1()
    {
        RedPointSystem.SetNodeValue(RedPointPath.Main, 3, RedPointType.Exclamation);
    }
    
    [Button("SetMain_Tasks")]
    public void Test2()
    {
        RedPointSystem.SetNodeValue(RedPointPath.Main_Tasks, 6, RedPointType.Number);
    }
     
    [Button("SetMain_Tasks_Daily")]
    public void Test3()
    {
        RedPointSystem.SetNodeValue(RedPointPath.Main_Tasks_Daily, 11, RedPointType.Number);
        RedPointSystem.SetNodeValue(RedPointPath.Main_Tasks_Achie, 22, RedPointType.Exclamation);
    }
}