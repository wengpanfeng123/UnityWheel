using System;
using System.Collections;
using System.Collections.Generic;
using MemoryPack;
using UnityEngine;


[MemoryPackable]
public partial class PlayData
{
    [MemoryPackOrder(0)]
    public string PName;
    [MemoryPackOrder(1)]
    public int Age;

    [MemoryPackInclude] //私有字段/属性通过[MemoryPackInclude]特性显式标记。
    private int _level; 
}

public class TestMemPack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayData data = new PlayData();
        data.PName = "xicheng";
        data.Age = 11;
        //序列化
        byte[] bytes = MemoryPackSerializer.Serialize(data);
        //反序列化
        PlayData data2 = MemoryPackSerializer.Deserialize<PlayData>(bytes);
        
        
        //高级用法
        //栈上分配1024缓冲，无GC
        Span<byte> stackBuffer = stackalloc byte[1024]; //只能方法内使用。
        //直接反序列化到缓冲，无需堆分配。
       
        //var byteWritten = MemoryPackSerializer.Serialize(stackBuffer, data);

        byte[] array = new byte[1024];
        Span<byte> span = array.AsSpan(); //指向现有数组，不产生新分配。


        /*应用场景：
         * 1.网络通信：
         *   a.消息传递 b.相比json，memorypack 可以显著减少网络延迟和带宽消耗
         * 2.存在与加载
         *   a.游戏进度保存到本地或者云存储
         *   b.二进制格式比json更小，读写速度更快。
         * 3.
         *
         */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
