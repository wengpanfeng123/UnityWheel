using System.Collections.Generic;
using UnityEngine;

public static class JsonListHelper
{
    // 序列化
    public static string Serialize<T>(List<T> list, bool prettyPrint = false)
    {
        JsonListWrapper<T> wrapper = new JsonListWrapper<T>
        {
            items = list
        };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    // 反序列化
    public static List<T> Deserialize<T>(string json)
    {
        JsonListWrapper<T> wrapper = JsonUtility.FromJson<JsonListWrapper<T>>(json);
        return wrapper.items;
    }
}