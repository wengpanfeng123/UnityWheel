using System;
using MessagePack;
using UnityEngine;

/*
 * 1.你应该使用索引键（整数）还是字符串键？
 * 我们建议使用索引键以实现更快的序列化和更紧凑的二进制表示，这比字符串键更有优势。
 * 然而，在调试时，字符串键中的额外信息可能会非常有用。
 
 * 2.如果后期版本停止使用某些成员(属性或字段)，你应该保留这些过时的成员（C# 提供了 Obsolete 属性来注解这些成员），
 * 直到所有其他客户端有机会更新并移除对这些成员的使用。
 * 
 */



[MessagePackObject] //添加序列化标签
public class PlayerData
{
    [Key(0)] //序列化字段或者属性
    public int Age;

    [Key(1)][Obsolete] //如果某个字段或者属性被弃用，请使用Obsolete属性来注解它。
    public float Height { get; set; }

    [Key(2)]
    public string Name { get; set; }

  
    [IgnoreMember] //忽略序列化字段或者属性
    public string Description;
}

//keyAsPropertyName: true  ，这样属性和字段就不用显示使用key标签了,但会使用字符串键。
[MessagePackObject(keyAsPropertyName: true)]
public class StudentData
{
    public int Name; //必须使用公开的字段或者属性，否则无法序列化。
    public string ClassId;
}


/*
 * 索引间隙会导致 MessagePack 在序列化时插入占位符。key(0) ,key(3) 中间就是存在1、2的索引间隙。
   反序列化时，占位符会被转换为对应类型的默认值。
   在设计数据结构时，尽量避免不必要的索引间隙，以减少序列化后的数据大小和处理开销。
 */

//IMessagePackSerializationCallbackReceiver序列化回调。
[MessagePackObject]
public class SampleCallback : IMessagePackSerializationCallbackReceiver
{
    [Key(0)] public int Key { get; set; }

    public void OnBeforeSerialize()
    {
        Debug.Log("OnBefore");
    }

    public void OnAfterDeserialize()
    {
        Debug.Log("OnAfter");
    }
}