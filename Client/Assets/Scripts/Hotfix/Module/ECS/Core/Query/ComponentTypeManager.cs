using System;
using System.Collections.Generic;

namespace ECS
{
    /* 思路： 这是最接近真正ECS框架的做法。为每种组件类型分配一个唯一的位标识 (bit flag)。
     * 每个实体的组件签名 (Signature)是一个位掩码 (bitmask)，其中对应它拥有的组件类型的位被置为1。
     * 注册表维护一个 Dictionary<Bitmask, List<GameObject>>（或 SortedList）。
     * 查询就是查找与目标组合掩码完全匹配（精确匹配）或包含（子集匹配）的实体列表。
     *
     *
     * 1.ulong类型支持64位无符号整数
     * 2.位掩码bitmask：位掩码是一种用于表示多个位的数字。用二进制位开关表示状态。
     */
    //负责为每个注册的组件类型分配一个唯一的 int ID (通常连续且从0开始) 和对应的 Bitmask (1 << id)。
    public static class ComponentTypeManager
    {
        //组件类型---映射---唯一ID
        private static Dictionary<System.Type, int> _componentTypeToId = new();
        //组件类型---映射---位掩码
        private static Dictionary<System.Type, ulong> _componentTypeToBitmask = new();
        
        //下一个可用的ID
        private static int _nextId = 0;
        //锁对象
        private static readonly object _lock = new();

        // 获取组件类型的位掩码
        public static ulong GetBitmaskForType(System.Type componentType)
        {
            lock (_lock)
            {
                // 如果已经注册过，直接返回
                if (_componentTypeToBitmask.TryGetValue(componentType, out ulong bitmask))
                {
                    return bitmask;
                }

                // 检查是否超过64种类型
                if (_nextId >= 64)
                {
                    throw new Exception("Exceeded maximum number of component types (64).");
                }

                // 分配新ID
                int id = _nextId++;
                bitmask = 1UL << id; // 将1左移id位，得到位掩码

                // 存储映射
                _componentTypeToId.Add(componentType, id);
                _componentTypeToBitmask.Add(componentType, bitmask);

                return bitmask;
            }
        }

        public static int GetIdForType(Type componentType)
        {
            lock (_lock)
            {
                return _componentTypeToId.GetValueOrDefault(componentType, -1); //未找到。
            }
        }
    }
}