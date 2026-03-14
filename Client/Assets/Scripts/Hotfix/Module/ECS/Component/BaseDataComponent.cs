using UnityEngine;

namespace ECS
{
    public abstract class BaseDataComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            // 确保实体已注册
            EntityRegistry.Inst.RegisterEntity(gameObject);
        }

        protected virtual void OnEnable()
        {
            // 通知EntityRegistry：这个组件被启用了
            EntityRegistry.Inst.OnComponentAdded(gameObject, GetType());
        }

        protected virtual void OnDisable()
        {
            // 通知EntityRegistry：这个组件被禁用了
            EntityRegistry.Inst.OnComponentRemoved(gameObject, GetType());
        }

        protected virtual void OnDestroy()
        {
            // 通知EntityRegistry：实体被销毁
            EntityRegistry.Inst.UnregisterEntity(gameObject);
        }
    }
}