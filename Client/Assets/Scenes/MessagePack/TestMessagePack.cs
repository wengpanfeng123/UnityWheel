using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MessagePack;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
namespace Scenes.MessagePack
{
    public class TestMessagePack:MonoBehaviour
    {
        private Stopwatch watch = new();
        private byte[] messagePackData;
        private string jsonDataString;
        
        void Start()
        { }

        [Button("GenerateLargetJson")]
        void GenerateLargetJson()
        {
            watch.Restart();
            var data = new Dictionary<string, List<dynamic>>
            {
                ["records"] = new List<dynamic>()
            };

            for (int i = 0; i < 100000; i++)
            {
                data["records"].Add(new
                {
                    id = i,
                    name = $"Item_{i}",
                    value = Random.Range(1,1000),
                    nested = new { a = Random.Range(0, 100), b = "test" }
                });
            }

            File.WriteAllText("large.json", JsonConvert.SerializeObject(data));
            watch.Stop();
            Debug.Log($"生成large.json耗时: {watch.ElapsedMilliseconds}ms");
        }
        

        [Button("ParseJson")]
        void ParseJson()
        {
            // 加载大JSON文件
            watch.Restart();
            jsonDataString = File.ReadAllText("large.json");
            watch.Stop();
            Debug.Log($"读取large.json耗时: {watch.ElapsedMilliseconds}ms");
            
            
            // 序列化测试
            watch.Restart();
            messagePackData = MessagePackSerializer.Serialize(jsonDataString); // MessagePack
            watch.Stop();
            Debug.Log($"MessagePack Serialize: {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            string compactJson = JsonUtility.ToJson(JsonUtility.FromJson<object>(jsonDataString)); // JsonUtility
            watch.Stop();
            Debug.Log($"JsonUtility Serialize: {watch.ElapsedMilliseconds}ms");
            
            watch.Restart();
            JsonConvert.SerializeObject(jsonDataString);
            Debug.Log($"Newtonsoft.Json Serialize: {watch.ElapsedMilliseconds}ms");
            watch.Stop();
            

            // 反序列化测试
            watch.Restart();
            string unpacked = MessagePackSerializer.Deserialize<string>(messagePackData);
            watch.Stop();
            Debug.Log($"MessagePack Deserialize: {watch.ElapsedMilliseconds}ms");

            watch.Restart();
            object obj = JsonUtility.FromJson<object>(jsonDataString);
            watch.Stop();
            Debug.Log($"JsonUtility Deserialize: {watch.ElapsedMilliseconds}ms");
            
            watch.Restart();
            JsonConvert.DeserializeObject<object>(jsonDataString);
            Debug.Log($"Newtonsoft.Json Deserialize: {watch.ElapsedMilliseconds}ms");
            watch.Stop();
        }
    }
}