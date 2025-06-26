using System;
using System.Collections.Generic;
using UnityEngine;

namespace xicheng.utility
{
    public static class WaitCache
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitSeconds = new();
        public static WaitForEndOfFrame EndOfFrame = new();
       
        public static WaitForSeconds Seconds(float t)
        {
            if (!WaitSeconds.TryGetValue(t, out var wait))
                WaitSeconds[t] = wait = new WaitForSeconds(t);
            return wait;
        }
        
        public static void Clear()
        {
            WaitSeconds.Clear();
            EndOfFrame = null;
        }
    }
}