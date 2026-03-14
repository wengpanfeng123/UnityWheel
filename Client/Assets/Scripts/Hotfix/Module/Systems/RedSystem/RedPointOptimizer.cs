using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//性能优化扩展
// RedPointOptimizer.cs
public class RedPointOptimizer : MonoBehaviour
{
    [SerializeField] private float hiddenUpdateInterval = 2.0f;
    [SerializeField] private float visibleUpdateInterval = 0.2f;
    
    private Dictionary<RedPointNode, float> updateTimers = new Dictionary<RedPointNode, float>();

    private void Update()
    {
        foreach (var node in RedPointSystem.Inst.nodes.Values)
        {
            updateTimers.TryAdd(node, 0);

            updateTimers[node] += Time.deltaTime;
            
            var interval = IsNodeVisible(node) ? 
                visibleUpdateInterval : 
                hiddenUpdateInterval;

            if (updateTimers[node] >= interval)
            {
                updateTimers[node] = 0;
                node.MarkDirty();
            }
        }
    }

    private bool IsNodeVisible(RedPointNode node)
    {
        // 实现具体的可见性判断逻辑
        return true;
    }
}