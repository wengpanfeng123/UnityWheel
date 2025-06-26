using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xicheng.utility
{
    public class TestCoroutine:MonoBehaviour
    {
        //使用案例：
        private void Start()
        {
             CoroutineRunner.Inst.Run("key-WaitTaskEnd",WaitTaskEnd());
        }

    
        private IEnumerator WithCacheTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                yield return WaitCache.Seconds(0.01f); //使用缓存。
            }
        }
        private IEnumerator WaitTaskEnd()
        {
            yield return WaitCache.EndOfFrame;
            Debug.Log("----WaitCache.EndOfFrame----");
            yield return WaitCache.Seconds(1);
            Debug.Log("----WaitCache.Seconds(1)----");
        }
        

        private bool OnWait()
        {
            return true;
        }
    }
}