using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main.Utility.Pool.GoPools
{
    /// <summary>
    /// 高性能GameObject对象池
    /// </summary>
    /// <typeparam name="T">组件类型，需继承自MonoBehaviour</typeparam>
    public class GoPool<T> : IDisposable where T : MonoBehaviour
    {
        // 对象池核心存储
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onCreate;

        // 统计信息
        public int ActiveCount => TotalCount - InactiveCount;
        public int InactiveCount => _pool.Count;
        public int TotalCount { get; private set; }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="initialSize">初始大小</param>
        /// <param name="parent">父对象</param>
        /// <param name="onCreate">创建时回调</param>
        /// <param name="onGet">获取时回调</param>
        /// <param name="onRelease">释放时回调</param>
        public GoPool(GameObject prefab, int initialSize = 0, Transform parent = null,
            Action<T> onCreate = null, Action<T> onGet = null, Action<T> onRelease = null)
        {
            _prefab = prefab;
            _parent = parent ? parent : new GameObject($"{typeof(T).Name}Pool").transform;
            _onCreate = onCreate;
            _onGet = onGet;
            _onRelease = onRelease;

            // 预加载对象
            if (initialSize > 0)
                Preload(initialSize);
        }

        /// <summary>
        /// 预加载对象
        /// </summary>
        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = CreateInstance();
                Release(obj);
            }
        }

        /// <summary>
        /// 异步预加载对象
        /// </summary>
        public IEnumerator PreloadAsync(int count, int batchSize = 100, float delay = 0.1f)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = CreateInstance();
                Release(obj);

                // 分批处理，避免卡顿
                if (i % batchSize == 0)
                    yield return new WaitForSeconds(delay);
            }
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Pop();
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj = CreateInstance();
            }

            _onGet?.Invoke(obj);
            return obj;
        }

        /// <summary>
        /// 释放对象回池中
        /// </summary>
        public void Release(T obj)
        {
            if (!obj) return;

            _onRelease?.Invoke(obj);
            obj.gameObject.SetActive(false);

            // 检查是否已在池中
            if (!_pool.Contains(obj))
                _pool.Push(obj);
        }

        /// <summary>
        /// 释放对象回池中（通过GameObject）
        /// </summary>
        public void Release(GameObject obj)
        {
            if (!obj) return;

            var component = obj.GetComponent<T>();
            if (component)
                Release(component);
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void Clear()
        {
            foreach (var item in _pool)
            {
                if (item)
                    UnityEngine.Object.Destroy(item.gameObject);
            }

            _pool.Clear();
            TotalCount = 0;
        }

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            Clear();
            if (_parent && _parent.parent == null)
                UnityEngine.Object.Destroy(_parent.gameObject);
        }

        // 创建新实例
        private T CreateInstance()
        {
            var obj = UnityEngine.Object.Instantiate(_prefab, _parent);
            obj.SetActive(false);
            var component = obj.GetComponent<T>();

            if (!component)
                throw new InvalidOperationException($"预制体 {_prefab.name} 不包含 {typeof(T)} 组件");

            _onCreate?.Invoke(component);
            TotalCount++;
            return component;
        }
    }
}