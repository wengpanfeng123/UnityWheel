using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RedPointSystemExtensions
{
    public static void QueueDirtyNode(this RedPointSystem system, RedPointNode node)
    {
        // if (!system.dirtyNodes.Contains(node))
        // {
        //     system.dirtyNodes.Add(node);
        // }
    }
}
