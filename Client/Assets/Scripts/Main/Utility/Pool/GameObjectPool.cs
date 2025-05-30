using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using xicheng.res;
using Object = UnityEngine.Object;

namespace xc.pool
{
    public class PoolObject
    {
        public DateTime SurvivalTime;
        public GameObject Go;
    }

    //对象池
    public class GoPool
    {
        private ClassObjectPool<PoolObject> _classPool;
        public string PoolName;
        private readonly Dictionary<string, List<PoolObject>> _cacheObj = new();
        //key:实例id --- value:path
        private readonly Dictionary<int, string> _keyTypeDict = new();
        private readonly GameObject _poolRoot;
        private readonly int _maxCount = 5;

        private float _survivalTime = 5; //在池中保留的时间。

        public GoPool(string poolName, int maxCount = 10)
        {
            PoolName = poolName;
            _maxCount = maxCount;
            _poolRoot = new GameObject($"_GameObjectPool_{poolName}_");
            _classPool = new ClassObjectPool<PoolObject>(10);
        }

        private void AddInPool(string key, GameObject go)
        {
            if (!_cacheObj.ContainsKey(key))
                _cacheObj.Add(key, new List<PoolObject>());
            var poolObject = _classPool.Spawn();
            poolObject.SurvivalTime = DateTime.Now.AddSeconds(_survivalTime);
            _cacheObj[key].Add(poolObject);
        }

        
        
        public void OnUpdate()
        {
            foreach (var objectList in _cacheObj.Values)
            {
                for (int i = objectList.Count-1; i >=0; i--)
                {
                    var poolObject = objectList[i];
                    if (poolObject.SurvivalTime > DateTime.Now)
                    {
                        Object.Destroy(poolObject.Go);
                        _classPool.Recycle(poolObject);
                        objectList.RemoveAt(i);
                    }
                }
            }
        }

        private PoolObject FindUsable(string keyPath)
        {
            if (!_cacheObj.TryGetValue(keyPath, out var objectList) ||objectList.Count <= 0)
                return null;
            var poolObject = objectList[^1];
            objectList.Remove(poolObject);
            return poolObject;
        }
        
        
        public GameObject Get(string assetPath,Transform parent= null, Vector3 position = default, Quaternion quaternion = default)
        {
            var poolObject = FindUsable(assetPath);
            if (poolObject != null && poolObject.Go!=null)
            {
                poolObject.Go.transform.position = position;
                poolObject.Go.transform.rotation = quaternion;
                poolObject.Go.SetActive(true);
                return poolObject.Go;
            }
            
            // var go = ResFrame.LoadAsset<GameObject>(assetPath);
            // if (go == null)
            // {
            //     Debug.LogError("xc.[GoPool]Load prefab is null .path :" + assetPath);
            //     return null;
            // }
            // var cloneObj = Object.Instantiate(go, position, quaternion);
            // _keyTypeDict.Add(cloneObj.GetInstanceID(),assetPath);
            // return cloneObj;
            return null;
        }

        public string GetInstAssetPath(int instanceId)
        {
            if (_keyTypeDict.TryGetValue(instanceId, out var assetPath))
            {
                return assetPath;
            }

            return string.Empty;
        }

        //回收
        public bool Recycle(GameObject obj,bool isDestroy =false)
        {
            if (obj == null)
            {
                Debug.LogError("回收对象失败，对象为空");
                return false;
            }

            if (isDestroy)
            { 
                return false;
            }

            if (_poolRoot.transform.childCount >= _maxCount)
            {
                return false;
            }
            
            if (!_keyTypeDict.TryGetValue(obj.GetInstanceID(), out var keyPath))
            {
                return false;
            }
            
            obj.transform.SetParent(_poolRoot.transform);
            obj.SetActive(false);
            AddInPool(keyPath, obj);
            return true;
        }
        
        //清空某一种类型对象
        public void Clear(string keyPath)
        {
            if (!_cacheObj.TryGetValue(keyPath, out var objs))
                return;
            foreach (var poolObject in objs)
            {
                // ResCtrl.Inst.DestroyGameObject(poolObject.Go);
                // _classPool.Recycle(poolObject);
            }

            _cacheObj.Remove(keyPath);
        }
        
        //清空对象池
        public void OnRelease()
        {
            foreach (var objs in _cacheObj.Values)
            {
                foreach (var obj in objs)
                {
                    // ResCtrl.Inst.DestroyGameObject(obj.Go);
                    // Object.DestroyImmediate(obj.Go);
                }
            }

            Object.Destroy(_poolRoot);
            _cacheObj.Clear();
            _classPool.OnRelease();
            _classPool = null;
        }
    }
}