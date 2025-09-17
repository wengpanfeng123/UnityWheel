using System.Collections.Generic;
using UnityEngine;

public static class JsonDictHelper
{
    // 将字典转换为可序列化包装类
    public static JsonDictWrapper<TKey, TValue> ToSerializable<TKey, TValue>(
        Dictionary<TKey, TValue> dict)
    {
        var wrapper = new JsonDictWrapper<TKey, TValue>();
        foreach (var kvp in dict)
        {
            wrapper.items.Add(new JsonDictWrapper<TKey, TValue>.KeyValuePair
            {
                key = kvp.Key,
                value = kvp.Value
            });
        }
        return wrapper;
    }

    // 将包装类转换为字典
    public static Dictionary<TKey, TValue> FromSerializable<TKey, TValue>(
        JsonDictWrapper<TKey, TValue> wrapper)
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var item in wrapper.items)
        {
            dict[item.key] = item.value;
        }
        return dict;
    }

    // 序列化为JSON字符串
    public static string Serialize<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        var wrapper = ToSerializable(dict);
        return JsonUtility.ToJson(wrapper, true);
    }

    // 从JSON反序列化
    public static Dictionary<TKey, TValue> Deserialize<TKey, TValue>(string json)
    {
        var wrapper = JsonUtility.FromJson<JsonDictWrapper<TKey, TValue>>(json);
        return FromSerializable(wrapper);
    }
}