using System;
using System.Collections.Generic;
using UnityEngine;
using xicheng.common;

namespace ECS
{ 
    [DefaultExecutionOrder(-100)] // 确保在其他脚本之前执行
    public class EntityRegistry : MonoSingleton<EntityRegistry>
    {
        //实体当前签名的映射：GameObject -> 签名（ulong）
        private Dictionary<GameObject, ulong> _entitySignatures = new();
        // 签名到实体列表的映射：签名（ulong）-> 实体列表
        private Dictionary<ulong, List<GameObject>> _signatureToEntities = new();
 
        // 等待初始化的实体队列（解决初始化顺序问题）
        private Queue<GameObject> _pendingInitialization = new();
        
        private void Update()
        {
            while (_pendingInitialization.Count>0)
            {
                GameObject entity = _pendingInitialization.Dequeue();
                if (entity != null)
                {
                    InitializeEntitySignature(entity);
                }
            }
        }

        // 注册实体（在实体创建时调用）
        public void RegisterEntity(GameObject entity)
        {
            if (entity == null) return;
        
            // 如果实体已经注册，跳过
            if (_entitySignatures.ContainsKey(entity))
                return;
        
            // 初始签名设为0
            _entitySignatures[entity] = 0;
        
            // 添加到待初始化队列
            _pendingInitialization.Enqueue(entity);
        }

        // 注销实体（在实体销毁时调用）
        public void UnregisterEntity(GameObject entity)
        {
            if (entity == null) return;
        
            if (_entitySignatures.TryGetValue(entity, out ulong currentSignature))
            {
                // 从签名映射中移除
                if (_signatureToEntities.TryGetValue(currentSignature, out var list))
                {
                    list.Remove(entity);
                    if (list.Count == 0)
                    {
                        _signatureToEntities.Remove(currentSignature);
                    }
                }
            
                // 从实体签名字典中移除
                _entitySignatures.Remove(entity);
            }
        }
        
        // 初始化实体签名（处理所有组件）
        private void InitializeEntitySignature(GameObject entity)
        {
            ulong signature = 0;
        
            // 获取实体上的所有组件
            Component[] components = entity.GetComponents<Component>();
            foreach (Component comp in components)
            {
                // 跳过Transform和Unity内置组件
                if (comp is Transform || comp is MonoBehaviour || comp is Renderer || comp is Collider)
                    continue;
            
                Type compType = comp.GetType();
                ulong bitmask = ComponentTypeManager.GetBitmaskForType(compType);
                signature |= bitmask;
            }
        
            // 更新实体签名
            UpdateEntitySignature(entity, signature);
        }

        

        // 核心方法：查询拥有 EXACTLY 指定签名的实体 (精确匹配)
        public List<GameObject> GetEntitiesByExactSignature(ulong signature)
        {
            if (_signatureToEntities.TryGetValue(signature, out var list))
            {
                return new List<GameObject>(list); // 返回副本
            }

            return new List<GameObject>();
        }

        // 核心方法：查询拥有 AT LEAST 指定组件组合的实体 (子集匹配) - 更常用!!!
        public List<GameObject> GetEntitiesBySubsetSignature(ulong requiredSignature)
        {
            List<GameObject> result = new List<GameObject>();
            foreach (var kvp in _signatureToEntities)
            {
                ulong signature = kvp.Key;
                // 检查当前签名 (signature) 是否包含所需的签名 (requiredSignature) 的所有位
                if ((signature & requiredSignature) == requiredSignature)
                {
                    result.AddRange(kvp.Value); // 添加这个签名对应的所有实体
                }
            }

            return result;
        }

 

        // 当组件添加时调用
        public void OnComponentAdded(GameObject entity, Type componentType)
        {
            if (entity == null || componentType == null) return;
        
            // 获取组件位掩码
            ulong componentBitmask = ComponentTypeManager.GetBitmaskForType(componentType);
        
            // 获取当前签名
            _entitySignatures.TryGetValue(entity, out ulong currentSignature);
            ulong newSignature = currentSignature | componentBitmask;
        
            // 更新签名
            UpdateEntitySignature(entity, newSignature);
        }

        // 当组件移除时调用
        public void OnComponentRemoved(GameObject entity, Type componentType)
        {
            if (entity == null || componentType == null) return;
        
            // 获取组件位掩码
            ulong componentBitmask = ComponentTypeManager.GetBitmaskForType(componentType);
        
            if (_entitySignatures.TryGetValue(entity, out ulong currentSignature))
            {
                ulong newSignature = currentSignature & ~componentBitmask;
                UpdateEntitySignature(entity, newSignature);
            }
        }
        
        // 更新实体签名（内部方法）
        private void UpdateEntitySignature(GameObject entity, ulong newSignature)
        {
            if (!_entitySignatures.TryGetValue(entity, out ulong oldSignature))
            {
                // 如果实体未注册，先注册
                RegisterEntity(entity);
                return;
            }
        
            // 如果签名没有变化，直接返回
            if (oldSignature == newSignature) 
                return;
        
            // 从旧签名列表中移除实体
            if (_signatureToEntities.TryGetValue(oldSignature, out var oldList))
            {
                oldList.Remove(entity);
                if (oldList.Count == 0)
                {
                    _signatureToEntities.Remove(oldSignature);
                }
            }
        
            // 将实体添加到新签名列表
            if (!_signatureToEntities.TryGetValue(newSignature, out var newList))
            {
                newList = new List<GameObject>();
                _signatureToEntities[newSignature] = newList;
            }
        
            if (!newList.Contains(entity))
            {
                newList.Add(entity);
            }
        
            // 更新实体当前签名
            _entitySignatures[entity] = newSignature;
        }
        
        // 查询：获取拥有至少指定组件组合（requiredSignature）的实体列表
        public List<GameObject> GetEntitiesWithSignature(ulong requiredSignature)
        {
            List<GameObject> result = new List<GameObject>();
        
            // 遍历所有签名
            foreach (var kvp in _signatureToEntities)
            {
                ulong signature = kvp.Key;
            
                // 检查当前签名是否包含所需的所有位
                if ((signature & requiredSignature) == requiredSignature)
                {
                    result.AddRange(kvp.Value);
                }
            }
            return result;
        }

        // 提供一个更友好的泛型方法，用于查询多个组件类型
        public List<GameObject> GetEntitiesWithComponents(params Type[] componentTypes)
        {
            ulong requiredSignature = 0;
            foreach (Type type in componentTypes)
            {
                ulong bitmask = ComponentTypeManager.GetBitmaskForType(type);
                requiredSignature |= bitmask;
            }
            return GetEntitiesWithSignature(requiredSignature);
        }
        
    }
}