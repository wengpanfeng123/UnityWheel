using UnityEngine;

namespace Hotfix.ECS
{
    //用于管理所有系统的执行顺序
    public class SystemsManager : MonoBehaviour
    {
        [Header("System Execution Order")]
        public MonoBehaviour[] systemsInOrder;

        private void Awake()
        {
            // 设置脚本执行顺序
            for (int i = 0; i < systemsInOrder.Length; i++)
            {
                if (systemsInOrder[i] != null)
                {
                    // 设置负值确保执行顺序
                    int executionOrder = -1000 + i * 10;
                    SetExecutionOrder(systemsInOrder[i].GetType(), executionOrder);
                }
            }
            DontDestroyOnLoad(this);
        }

        private void SetExecutionOrder(System.Type type, int order)
        {
#if UNITY_EDITOR
            string scriptName = type.Name;
            foreach (var script in UnityEditor.MonoImporter.GetAllRuntimeMonoScripts())
            {
                if (script.name == scriptName)
                {
                    UnityEditor.MonoImporter.SetExecutionOrder(script, order);
                    break;
                }
            }
#endif
        }
    }
}