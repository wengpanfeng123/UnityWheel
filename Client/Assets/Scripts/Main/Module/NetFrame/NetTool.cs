using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;


public class NetTool
{
    //转大端序
    public static byte[] ToBigEndian(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return bytes;
    }


    // 新增压缩方法（使用Brotli算法）
    public static byte[] CompressData(byte[] data)
    {
        using var output = new MemoryStream();
        using (var compressor = new BrotliStream(output, CompressionLevel.Optimal))
        {
            compressor.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    // 新增加密方法（使用AES）
    public static byte[] EncryptData(byte[] data, byte[] key)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        using MemoryStream ms = new();
        ms.Write(aes.IV, 0, aes.IV.Length);
        using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        cs.Write(data, 0, data.Length);
        cs.FlushFinalBlock();
        return ms.ToArray();
    }
}