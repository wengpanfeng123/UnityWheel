using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xicheng.Utility
{
    public class CoroutineRunner:MonoSingleton<CoroutineRunner>
    {
        private readonly Dictionary<string, Coroutine> _runningCoroutines =new();
            
        public void Run(string key, IEnumerator routine)
        {
            Stop(key);

            //创建一个包装器协程，在原始协程完成后自动清理
            IEnumerator WrapperCoroutine()
            {
                yield return routine;
                _runningCoroutines.Remove(key); // 自动清理
                Debug.Log("自动移除key:" + key);
            }

            var coroutine = StartCoroutine(WrapperCoroutine());
            _runningCoroutines[key] = coroutine;
        }

        public void Stop(string key)
        {
            if(_runningCoroutines.TryGetValue(key,out var co))
            {
                StopCoroutine(co);
                _runningCoroutines.Remove(key);
            }
        }

        public void StopAll()
        {
            foreach(var co in _runningCoroutines.Values)
                StopCoroutine(co);
            _runningCoroutines.Clear();
        }

        private void OnDestroy()
        {
            WaitCache.Clear();
            StopAll();
            _runningCoroutines.Clear();
        }
    }
}