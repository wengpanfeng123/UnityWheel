/*************************************************************************
* Copyright  xicheng. All rights reserved.
*------------------------------------------------------------------------
* File     : EncryptUtil.cs
* Author   : xicheng
* Date     : 2025-09-03 14:58
* Tips     : xicheng知识库
* Description :加密工具类型
 * 1. AES对称加密
 * 2. RSA非对称加密
 * 3. XOR异或加密
*************************************************************************/

using System;
using System.Security.Cryptography;
using System.Text;

namespace HsJam
{
    public static class EncryptUtil
    {
        #region XOR(异或)
        
        private static byte[] Encrypt(byte[] data, byte[] key)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
            return result;
        }

        //加密接口
        public static string EncryptToBase64(string text, string key)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(text);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] encrypted = Encrypt(dataBytes, keyBytes);
            return Convert.ToBase64String(encrypted);
        }

        //解密接口
        public static string DecryptFromBase64(string base64Text, string key)
        {
            byte[] encryptedBytes = Convert.FromBase64String(base64Text);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] decrypted = Encrypt(encryptedBytes, keyBytes);
            return Encoding.UTF8.GetString(decrypted);
        }

        #endregion
        
        #region AES
        //AES-128 CBC 模式
        //key 长度必须是 16 字节，iv 也一样
        private static string iv =  "1234567890123456"; // 16位 IV
        
        public static string AesEncrypt(string plainText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] result = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(result);
        }

        public static string AesDecrypt(string cipherText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform decrypt = aes.CreateDecryptor();
            byte[] result = decrypt.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return  Encoding.UTF8.GetString(result);
        }
        #endregion

        #region RSA
        //公钥加密，私钥解密。

        // public static void InitRsa()
        // {
        //     string original = "Unity机密数据";
        // }

        // 生成RSA密钥对
        public static (string publicKey, string privateKey) GenerateKeyPair()
        {
            using RSA rsa = RSA.Create(2048); // 推荐2048位密钥，最多加密 245字节数据
            return (
                rsa.ToXmlString(false),  // 公钥
                rsa.ToXmlString(true)    // 私钥
            );
        }

        public static string RsaEncrypt(string publicKeyXml, string plainText)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(publicKeyXml);
        
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        
            return Convert.ToBase64String(encrypted);
        }
        
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privateKeyXml">私钥</param>
        /// <param name="encryptedBase64">加密内容</param>
        /// <returns></returns>
        public static string RsaDecrypt(string privateKeyXml, string encryptedBase64)
        {
            using RSA rsa = RSA.Create();
            rsa.FromXmlString(privateKeyXml);
        
            byte[] data = Convert.FromBase64String(encryptedBase64);
            byte[] decrypted = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        
            return Encoding.UTF8.GetString(decrypted);
        }

        #endregion
        
        #region 混合方案（AES+RSA）
        public static string HybridEncrypt(string publicKey, byte[] largeData)
        { 
            // 1. 生成随机AES密钥
            using Aes aes = Aes.Create();
            aes.GenerateKey();
            // 2. 用AES加密数据
            //byte[] encryptedData = aes.en(largeData, aes.IV);
            // 3. 用RSA加密AES密钥
            //byte[] encryptedKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);
            return string.Empty;
        }
        #endregion
    }
}