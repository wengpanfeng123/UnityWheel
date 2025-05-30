/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: 类对象池

------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace xicheng.res
{
    public class ClassObjectPool<T> where T : class, new()
    {
        /// <summary>
        /// 存放类的一个对象池，偏底层的东西 尽量别使用list 
        /// </summary>
        private readonly Stack<T> _poolStack = new();
        /// <summary>
        /// 最大的缓存对象个数 小于等于0表示不限个数
        /// </summary>
        protected int MaxCount = 0;

        public int PoolCount => _poolStack.Count;

        
        public ClassObjectPool(int maxCount)
        {
            MaxCount = maxCount;
            for (int i = 0; i < maxCount; i++)
            {
                _poolStack.Push(new T());
            }
        }
        /// <summary>
        /// 取出对象
        /// </summary>
        /// <returns></returns>
        public T Spawn()
        {
            if (_poolStack.Count>0)
            {
                return _poolStack.Pop();
            }
            else
            {
                return new T();
            }
        }
        /// <summary>
        /// 回收类对象
        /// </summary>
        /// <param name="obj"></param>
        public void Recycle(T obj)
        {
            if (obj==null)
            {
                Debug.LogError("Recycl Obj failed,obj is null!");
                return;
            }

            if (MaxCount==0 ||  _poolStack.Count < MaxCount)
            {
                _poolStack.Push(obj);
            }
            else
            {
                obj = null;
            }
        }

        public void OnRelease()
        {
            _poolStack.Clear();
        }
    }
}