using ECS;

namespace Hotfix.ECS.Test
{
    using System.Collections.Generic;
    using UnityEngine;

    public class HealthSystem : MonoBehaviour
    {
        // 缓存需要的组件位掩码
        private ulong _requiredSignature;
    
        // 用于缓存组件引用，减少GetComponent调用
        private Dictionary<GameObject, HealthData> _healthDataCache = new Dictionary<GameObject, HealthData>();

        private void Start()
        {
            // 计算需要的签名：HealthData组件
            _requiredSignature = ComponentTypeManager.GetBitmaskForType(typeof(HealthData));
        }

        private void Update()
        {
            // 获取所有拥有HealthData的实体
            List<GameObject> entities = EntityRegistry.Inst.GetEntitiesWithSignature(_requiredSignature);
        
            foreach (GameObject entity in entities)
            {
                // 获取HealthData组件（使用缓存减少GetComponent调用）
                if (!_healthDataCache.TryGetValue(entity, out HealthData health))
                {
                    health = entity.GetComponent<HealthData>();
                    _healthDataCache[entity] = health;
                }
            
                // 跳过无效或已销毁的实体
                if (health == null) 
                    continue;
            
                // 处理死亡状态
                if (health.currentHealth <= 0 && !health.isDead)
                {
                    health.isDead = true;
                    HandleDeath(entity);
                }
            }
        }
    
        // 应用伤害（可由其他系统调用）
        public void ApplyDamage(GameObject target, float damage)
        {
            if (target == null) return;
        
            HealthData health = target.GetComponent<HealthData>();
            if (health != null && !health.isInvulnerable && !health.isDead)
            {
                health.currentHealth -= damage;
            
                // 更新缓存
                if (_healthDataCache.ContainsKey(target))
                {
                    _healthDataCache[target] = health;
                }
            }
        }
    
        private void HandleDeath(GameObject entity)
        {
            Debug.Log($"{entity.name} has died!");
            // 这里可以触发死亡事件、播放动画等
            // 实际项目中，我们可能不会立即销毁实体，而是触发一个死亡状态
        }
    }
}