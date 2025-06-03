using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace xicheng.ui
{
    public partial class UIManager
    {
        
        private int MAX_INSTANCES = 5;
        
        public void Recycle(UIBase instance)
        {
            var uiKey = instance.UIKey;
            if (!_instancesDict.TryGetValue(uiKey, out var stack)) 
            {
                stack = new Stack<UIBase>();
                _instancesDict[uiKey] = stack;
            }

            if (stack.Count >= MAX_INSTANCES)
            {
                instance.DestroyUI();
                return;
            }

            instance.OnRecycle();
            stack.Push(instance); // 回收实例到缓存
        }
        
        //TODO:根据LRU算法缓存UI实例
 
        //添加 LRU 清理方法
        private void CleanUpLRUCache()
        {
            foreach (var pair in _instancesDict)
            {
                // var stack = pair.Value;
                // if (stack.Count <= 0) continue;
                //
                // // 转为列表并按时间戳排序（升序：越早的时间排在前面）
                // var list = new List<UIInstanceInfo>(stack);
                // list.Sort((a, b) => a.LastUsedTime.CompareTo(b.LastUsedTime));
                //
                // // 保留最近使用的 MAX_INSTANCES 个实例，其余销毁
                // while (list.Count > MAX_INSTANCES)
                // {
                //     var oldest = list[0];
                //     oldest.Instance.DestroyUI();
                //     list.RemoveAt(0);
                // }
                //
                // // 重新构建栈
                // stack.Clear();
                // foreach (var info in list)
                // {
                //     stack.Push(info);
                // }
            }
        }
        
        private async void StartCacheCleanupCoroutine()
        {
            while (true)
            {
                await Task.Delay(60000); // 每分钟清理一次
                CleanUpLRUCache();
            }
        }
    }
}