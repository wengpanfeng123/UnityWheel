using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using MemoryPack;
using MemoryPack.Compression;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Example
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(TestDicValue))]
    public abstract partial class Data
    {
        [MemoryPackOrder(0)]
        public bool testValue = true;
    }
    [MemoryPackable]
    public partial class TableHeadData 
    {
        public int tableIndex;
        public int tableCount;
        public int tableBineOffsetByteOffset;
        public int tableRealDataByteOffset;
    }
    
    [MemoryPackable]
    public partial class TestDicValue: Data
    {
        [MemoryPackOrder(2)]
        public int intValue;
        [MemoryPackOrder(1)]
        public float floatValue;
    }
    public class UsageExample : MonoBehaviour
    {

        private Dictionary<int,TestDicValue> dic = new Dictionary<int, TestDicValue>();
        private void Awake()
        {
            TestListFunc();
            //TestDicFunc();
            //TestClassFunc();
        }
        private void TestListFunc()
        {
            for (int index = 0; index < 10; index++)
            {
                dic.Add(index, new TestDicValue { intValue = index ,floatValue = index * 10 });
            }
            using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);

            var msArrayBufferWriter = new ArrayBufferWriter<byte>();
            var ms = msArrayBufferWriter as IBufferWriter<byte>;
            var writer = new MemoryPackWriter(ref ms,state);
            //writer.WriteObjectHeader(1);
            writer.WriteCollectionHeader(dic.Count);
            List<byte[]> recordCount = new List<byte[]>(dic.Count);
            int totalByteLength = 0;
            foreach (var record in dic)
            {
                // writer.WriteUnionHeader((ushort) 3);
                writer.WriteObjectHeader((byte)3); // ==> 对象数量
                writer.WriteUnmanaged(record.Value.testValue);
                writer.WriteUnmanaged(record.Value.floatValue);
                writer.WriteUnmanaged(record.Value.intValue);
                
                //MemoryPackSerializer.Serialize(ref writer, record.Value);

                // msArrayBufferWriter.Clear();
                // totalByteLength += byteArray.Length;
                // recordCount.Add(byteArray);
            }
            writer.Flush();
            var byteArray = msArrayBufferWriter.WrittenSpan.ToArray();
            
            using var compressor = new BrotliCompressor(CompressionLevel.Fastest);
            compressor.CopyTo(ref writer);
            var bin = compressor.ToArray();
            using var dcp = new BrotliDecompressor();
            var buffer = dcp.Decompress(bin);
            
            var data = MemoryPackSerializer.Deserialize<List<TestDicValue>>(byteArray);

            foreach (var value in data)
            {
                Debug.Log(" "+value.intValue+" "+value.floatValue);
            }
        }
        private void TestDicFunc()
        {
            for (int index = 0; index < 10; index++)
            {
                dic.Add(index, new TestDicValue { intValue = index ,floatValue = index * 10 });
            }
            using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);

            var msArrayBufferWriter = new ArrayBufferWriter<byte>();
            var ms = msArrayBufferWriter as IBufferWriter<byte>;
            var writer = new MemoryPackWriter(ref ms,state);
            //writer.WriteObjectHeader(1);
            writer.WriteCollectionHeader(dic.Count);
            List<byte[]> recordCount = new List<byte[]>(dic.Count);
            int totalByteLength = 0;
            foreach (var record in dic)
            {
                writer.WriteUnmanaged(record.Key);
                // writer.WriteUnionHeader((ushort) 3);
                 writer.WriteObjectHeader((byte)3); // ==> 对象数量
                writer.WriteUnmanaged(record.Value.testValue);
                writer.WriteUnmanaged(record.Value.floatValue);
                writer.WriteUnmanaged(record.Value.intValue);
                
                //MemoryPackSerializer.Serialize(ref writer, record.Value);

                // msArrayBufferWriter.Clear();
                // totalByteLength += byteArray.Length;
                // recordCount.Add(byteArray);
            }
            writer.Flush();
            var byteArray = msArrayBufferWriter.WrittenSpan.ToArray();
            
            var data = MemoryPackSerializer.Deserialize<Dictionary<int,TestDicValue>>(byteArray);

            foreach (var value in data)
            {
                Debug.Log(value.Key+" "+value.Value.intValue+" "+value.Value.floatValue);
            }
        }
        private void TestClassFunc()
        {
            for (int index = 0; index < 10; index++)
            {
                dic.Add(index, new TestDicValue { intValue = index ,floatValue = index * 10 });
            }
            using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);

            var msArrayBufferWriter = new ArrayBufferWriter<byte>();
            var ms = msArrayBufferWriter as IBufferWriter<byte>;
            var writer = new MemoryPackWriter(ref ms,state);
            //writer.WriteObjectHeader(1);
            //writer.WriteCollectionHeader(dic.Count);
            List<byte[]> recordCount = new List<byte[]>(dic.Count);
            int totalByteLength = 0;
            foreach (var record in dic)
            {
                //writer.WriteUnmanaged(record.Key);
                writer.WriteUnionHeader((ushort) 0);
                writer.WriteObjectHeader((byte)3); // ==> 对象数量
                writer.WriteUnmanaged(record.Value.testValue);
                writer.WriteUnmanaged(record.Value.floatValue);
                writer.WriteUnmanaged(record.Value.intValue);
                
                //MemoryPackSerializer.Serialize(ref writer, record.Value);
                writer.Flush();
                var byteArray = msArrayBufferWriter.WrittenSpan.ToArray();
                msArrayBufferWriter.Clear();
                totalByteLength += byteArray.Length;
                recordCount.Add(byteArray);
            }
            byte[] totalByteArray;
            int beginBinaryOffset = 0;
            List<Vector2Int> binOffsets = new List<Vector2Int>(recordCount.Count);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    foreach (var record in recordCount)
                    {
                        Vector2Int offset = Vector2Int.zero;
                        offset.x = beginBinaryOffset;
                        binaryWriter.Write(record);
                        beginBinaryOffset += record.Length;
                        offset.y = record.Length;
                        binOffsets.Add(offset);
                    }
                }
                totalByteArray = memoryStream.ToArray();
            }
            //writer.Flush();
            // var array = msArrayBufferWriter.WrittenSpan.ToArray();
            //
            // var data = MemoryPackSerializer.Deserialize<Dictionary<int,TestDicValue>>(array);

            foreach (var binOffsetData in binOffsets)
            {
                
                byte[] deserializeData = new byte[binOffsetData.y];
                for (int index = 0; index < binOffsetData.y; index++)
                {
                    deserializeData[index] = totalByteArray[binOffsetData.x + index];
                }
                var data = MemoryPackSerializer.Deserialize<Data>(deserializeData) as TestDicValue;
                Debug.Log("binOffset "+binOffsetData.x+" "+binOffsetData.y+" "+data.testValue+"  "+data.intValue+" "+data.floatValue);//data.intValue+" "+data.floatValue
            }
            
        }
    }
}