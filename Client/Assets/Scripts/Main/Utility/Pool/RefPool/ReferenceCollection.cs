using System;
using System.Collections.Generic;

namespace Xicheng.ReferencePool
{
    /*
     * Add() 向引用池中追加指定数量的引用。追加的引用标记为Unused。引用计数+1.
     * Acquire()从引用池中获取引用。若池中空的，则创建引用。若池中存在，则取出，取出后池子则不再保留该对象。获取的引用被标记为using,计数+1.
     * Release(type) 把引用归还池子中。 
     * Remove(type)把引用从池子中移除。 
     * 
     */
    //一个类型对应一个引用集合，每次请求从引用集合的队列中获取
    public sealed class ReferenceCollection
    {
        private readonly Queue<IReference> m_References;
        private Type m_ReferenceType;
        private int m_UsingReferenceCount;  //使用引用的次数。
        private int m_AcquireReferenceCount; //获取引用的次数。
        private int m_ReleaseReferenceCount; //释放引用的次数
        private int m_AddReferenceCount;    //添加到池子的引用数量。
        private int m_RemoveReferenceCount;//移除引用的总数量
        public int UnusedReferenceCount => m_References.Count;
        public int UsingReferenceCount => m_UsingReferenceCount;
        public int AcquireReferenceCount => m_AcquireReferenceCount;
        public int ReleaseReferenceCount => m_ReleaseReferenceCount;
        public int AddReferenceCount => m_AddReferenceCount;
        public int RemoveReferenceCount => m_RemoveReferenceCount;


        public ReferenceCollection(Type refType)
        {
            m_References = new Queue<IReference>();
            m_ReferenceType = refType;
            m_UsingReferenceCount = 0;
            m_AcquireReferenceCount = 0;
            m_ReleaseReferenceCount = 0;
            m_AddReferenceCount = 0;
            m_RemoveReferenceCount = 0;
        }

        public T Acquire<T>() where T : class, IReference, new()
        {
            if (typeof(T) != m_ReferenceType)
            {
                throw new Exception("类型不相同无法请求!!!");
            }
            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;
            lock (m_References)
            {
                if (m_References.Count > 0)
                {
                    return (T)m_References.Dequeue();
                }
            }
            m_AddReferenceCount++;
            return new T();
        }

        public IReference Acquire()
        {
            m_UsingReferenceCount++;
            m_AcquireReferenceCount++;
            lock (m_References)
            {
                if (m_References.Count > 0)
                {
                    return m_References.Dequeue();
                }
            }
            m_AddReferenceCount++;
            return (IReference)Activator.CreateInstance(m_ReferenceType);
        }

        
        //回收引用
        public void Release(IReference reference)
        {
            reference.Clear();
            lock (m_References)
            {
                if (m_References.Contains(reference))
                {
                    throw new Exception("引用已经被释放，请勿重新释放!!!");
                }

                m_References.Enqueue(reference);
            }

            m_ReleaseReferenceCount++;
            m_UsingReferenceCount--;
        }

        //添加引用
        public void Add<T>(int count) where T : class, IReference, new()
        {
            if (typeof(T) != m_ReferenceType)
            {
                throw new Exception($"类型无效无法添加!!!{typeof(T)} {m_ReferenceType}");
            }
            lock (m_References)
            {
                m_AddReferenceCount += count;
                while (count-- > 0)
                {
                    m_References.Enqueue(new T());
                }
            }
        }
        
        public void Add(int count)
        {
            lock (m_References)
            {
                m_AddReferenceCount += count;
                while (count-- > 0)
                {
                    m_References.Enqueue((IReference)Activator.CreateInstance(m_ReferenceType));
                }
            }
        }

        //删除指定数量的引用。
        public void Remove(int count)
        {
            lock (m_References)
            {
                if(count > m_References.Count)
                {
                    count = m_References.Count;
                }
                m_RemoveReferenceCount += count;
                while (count-- > 0)
                {
                    m_References.Dequeue();
                }
            }
        }

        //清空引用池。
        public void RemoveAll()
        {
            lock (m_References)
            {
                m_RemoveReferenceCount += m_References.Count;
                m_References.Clear();
            }
        }
    }
}