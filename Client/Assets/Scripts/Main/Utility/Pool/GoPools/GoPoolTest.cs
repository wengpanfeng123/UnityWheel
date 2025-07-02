using System.Collections;

namespace Main.Utility.Pool.GoPools
{
    using UnityEngine;

    public class Bullet : MonoBehaviour
    {
        // 重置子弹状态
        public void Reset()
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            // 其他重置逻辑
        }
    }

    public class ShootingSystem : MonoBehaviour
    {
        private GoPool<Bullet> _bulletPool;
    
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private int _poolSize = 100;

        private void Start()
        {
            // 创建对象池，设置初始化大小和回调
            _bulletPool = new GoPool<Bullet>(
                _bulletPrefab, 
                _poolSize,
                transform,
                onCreate: bullet => { /* 对象创建时调用 */ },
                onGet: bullet => { /* 对象从池中取出时调用 */ },
                onRelease: bullet => { bullet.Reset(); } // 对象放回池中时调用
            );
        }

        public void Fire()
        {
            // 从池中获取子弹
            var bullet = _bulletPool.Get();
            //bullet.transform.position = firePoint.position;
        
            // 5秒后自动回收
            StartCoroutine(ReleaseAfterDelay(bullet.gameObject, 5f));
        }

        private IEnumerator ReleaseAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            _bulletPool.Release(obj);
        }

        private void OnDestroy()
        {
            // 释放资源
            _bulletPool.Dispose();
        }
    }
}