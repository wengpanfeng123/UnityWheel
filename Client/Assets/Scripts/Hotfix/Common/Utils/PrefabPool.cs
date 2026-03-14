using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HsJam
{
    public class PrefabPool : MonoBehaviour
    {
        public GameObject templateGo;
        public bool useTemplate = true;
        [SerializeField] [ReadOnly] private bool isPrefab = false;

        private List<GameObject> _spawnObjects;

        /// <summary>
        /// 获取一定数量的类型
        /// </summary>
        /// <typeparam name="T">必须是组件</typeparam>
        /// <param name="count">要获取的数量</param>
        /// <param name="spawnList"></param>
        public void GetSpawnCount<T>(int count, ref List<T> spawnList) where T : Component
        {
            spawnList ??= new List<T>();
            if (count == spawnList.Count)
            {
                return;
            }

            if (count < spawnList.Count)
            {
                for (int i = spawnList.Count - 1; i >= count; --i)
                {
                    spawnList[i].gameObject.SetActive(false);
                }

                spawnList.RemoveRange(count, spawnList.Count - count);
                return;
            }

            _spawnObjects ??= new List<GameObject>();
            if (!isPrefab)
            {
                if (useTemplate)
                {
                    if (0 == _spawnObjects.Count)
                    {
                        _spawnObjects.Add(templateGo);
                    }
                }
                else
                {
                    templateGo.SetActive(false);
                }
            }

            int haveItemCount = _spawnObjects.Count;
            int subNum = haveItemCount - count; //spawnObjects中已有的个数到要获取的数量还差多少个
            for (int i = 0; i < -subNum; i++)
            {
                _spawnObjects.Add(SpawnOneObject(haveItemCount + i));
            }

            for (int i = spawnList.Count; i < count; i++)
            {
                GameObject spawnGo = _spawnObjects[i];
                spawnGo.SetActive(true);
                spawnList.Add(spawnGo.GetComponent<T>());
            }
        }

        GameObject SpawnOneObject(int index)
        {
            GameObject go = Instantiate(templateGo, transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

#if UNITY_EDITOR
            go.name = index.ToString();
#endif
            return go;
        }

        /// <summary>
        /// 获得当前实例过的所有对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allSpawnList"></param>
        public void GetAllSpawnList<T>(ref List<T> allSpawnList) where T : Component
        {
            if (null == _spawnObjects) return;
            if (null == allSpawnList)
                allSpawnList = new List<T>();
            int count = _spawnObjects.Count;
            if (count == allSpawnList.Count) return;
            allSpawnList.Clear();
            for (int i = 0; i < count; i++)
            {
                allSpawnList.Add(_spawnObjects[i].GetComponent<T>());
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (templateGo)
            {
                //isPrefab = PrefabUtility.IsPartOfPrefabAsset(templateGo);

                // PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(templateGo);
                // PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(templateGo);
                //
                // //是否是 prefab asset（Project 中的 prefab 文件）
                // isPrefab = PrefabAssetType.NotAPrefab != prefabAssetType &&
                //            PrefabInstanceStatus.NotAPrefab == prefabInstanceStatus;

                isPrefab = !templateGo.transform.parent;

                Debug.Log(isPrefab
                    ? "templateGo是一个Project中的 Prefab 资源！"
                    : "templateGo不是一个Project中的 Prefab 资源，可能是场景中的实例！");
            }
        }

        /// <summary>
        /// 加载一个Prefab，并且break之前的Prefab
        /// </summary>
        /// <returns></returns>
        [ContextMenu("Load To Edit This Prefab")]
        private GameObject LoadPrefab()
        {
            GameObject go = PrefabUtility.InstantiatePrefab(templateGo) as GameObject;
            if (null != go)
            {
                Transform t = go.transform;
                t.parent = transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                //go.layer = gameObject.layer;
            }

            return go;
        }
#endif
    }
}
