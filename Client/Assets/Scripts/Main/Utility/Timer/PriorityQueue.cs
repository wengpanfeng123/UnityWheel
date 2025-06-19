using System;
using System.Collections.Generic;

/// <summary>
/// 优先队列（最小堆实现）
/// </summary>
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> _heap;
    private Dictionary<T, int> _indexMap; //记录元素在堆中的索引

    public int Count => _heap.Count;
    public bool IsEmpty => _heap.Count == 0;

    public PriorityQueue()
    {
        _heap = new List<T>();
        _indexMap = new Dictionary<T, int>();
    }

    public PriorityQueue(int capacity)
    {
        _heap = new List<T>(capacity);
        _indexMap = new Dictionary<T, int>(capacity);
    }

    /// <summary>
    /// 添加元素到队列
    /// </summary>
    public void Enqueue(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");

        _heap.Add(item);
        _indexMap[item] = _heap.Count - 1; // 更新索引映射
        int childIndex = _heap.Count - 1;

        // 向上调整堆
        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (_heap[childIndex].CompareTo(_heap[parentIndex]) >= 0)
                break; // 已满足堆性质

            // 交换父子节点
            Swap(childIndex, parentIndex);

            childIndex = parentIndex;
        }
    }

    /// <summary>
    /// 获取并移除队首元素（最小值）
    /// </summary>
    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Priority queue is empty");

        int lastIndex = _heap.Count - 1;
        T frontItem = _heap[0];  // 保存队首元素
        Swap(0, lastIndex);  // 将最后一个元素移到队首
        _heap.RemoveAt(lastIndex);  // 移除最后一个元素
        _indexMap.Remove(frontItem); // 移除被删除元素的索引映射

        lastIndex--;  // 堆大小减1
        int parentIndex = 0;  // 从队首开始向下调整

        // 向下调整堆
        while (true)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > lastIndex)
                break;  // 没有左子节点

            int rightChildIndex = leftChildIndex + 1;
            int minChildIndex = leftChildIndex;  // 默认左子节点较小

            // 如果有右子节点且右子节点更小
            if (rightChildIndex <= lastIndex && _heap[rightChildIndex].CompareTo(_heap[leftChildIndex]) < 0)
                minChildIndex = rightChildIndex;

            // 如果父节点已经小于等于最小子节点，调整结束
            if (_heap[parentIndex].CompareTo(_heap[minChildIndex]) <= 0)
                break;

            // 交换父节点和最小子节点
            Swap(parentIndex, minChildIndex);

            parentIndex = minChildIndex;
        }

        return frontItem;
    }

    /// <summary>
    /// 获取队首元素（最小值）但不移除
    /// </summary>
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Priority queue is empty");

        return _heap[0];
    }

    /// <summary>
    /// 移除指定元素
    /// 时间复杂度：O(1)查找 + O(log n)调整
    /// </summary>
    public bool Remove(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");

        if (!_indexMap.TryGetValue(item, out int index))
            return false;  // 元素不存在

        int lastIndex = _heap.Count - 1;
        Swap(index, lastIndex);  // 将最后一个元素移到被移除位置
        _heap.RemoveAt(lastIndex);  // 移除最后一个元素
        _indexMap.Remove(item); // 移除被删除元素的索引映射
        lastIndex--;  // 堆大小减1

        // 如果移除的是最后一个元素，不需要调整
        if (index == lastIndex + 1)
            return true;

        // 尝试向上调整
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (_heap[index].CompareTo(_heap[parentIndex]) >= 0)
                break;  // 已满足堆性质

            // 交换父子节点
            Swap(index, parentIndex);

            index = parentIndex;
        }

        // 如果没有向上调整，尝试向下调整
        if (index == 0 || _heap[index].CompareTo(_heap[(index - 1) / 2]) >= 0)
        {
            // 向下调整堆
            while (true)
            {
                int leftChildIndex = index * 2 + 1;
                if (leftChildIndex > lastIndex)
                    break;  // 没有左子节点

                int rightChildIndex = leftChildIndex + 1;
                int minChildIndex = leftChildIndex;  // 默认左子节点较小

                // 如果有右子节点且右子节点更小
                if (rightChildIndex <= lastIndex && _heap[rightChildIndex].CompareTo(_heap[leftChildIndex]) < 0)
                    minChildIndex = rightChildIndex;

                // 如果父节点已经小于等于最小子节点，调整结束
                if (_heap[index].CompareTo(_heap[minChildIndex]) <= 0)
                    break;

                // 交换父节点和最小子节点
                Swap(index, minChildIndex);

                index = minChildIndex;
            }
        }

        return true;
    }

    /// <summary>
    /// 清空队列
    /// </summary>
    public void Clear()
    {
        _heap.Clear();
        _indexMap.Clear();
    }

    /// <summary>
    /// 检查队列是否包含指定元素
    /// </summary>
    public bool Contains(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");

        return _indexMap.ContainsKey(item);
    }

    /// <summary>
    /// 转换为数组（不保证顺序）
    /// </summary>
    public T[] ToArray()
    {
        return _heap.ToArray();
    }

    private void Swap(int index1, int index2)
    {
        (_heap[index1], _heap[index2]) = (_heap[index2], _heap[index1]);

        _indexMap[_heap[index1]] = index1;
        _indexMap[_heap[index2]] = index2;
    }
}