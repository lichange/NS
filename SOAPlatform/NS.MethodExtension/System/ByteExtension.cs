using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class ByteExtension
    {
        #region 转换为16进制字符串
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        public static string ToHex(this IEnumerable<byte> bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
        #endregion

        #region ToBase64String
        public static string ToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region ToInt
        public static int ToInt(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt32(value, startIndex);
        }
        public static long ToInt64(this byte[] value, int startIndex)
        {
            return BitConverter.ToInt64(value, startIndex);
        }
        #endregion

        #region Decode
        public static string Decode(this byte[] data, Encoding encoding)
        {
            return encoding.GetString(data);
        }
        #endregion

        #region Hash
        //使用指定算法Hash
        public static byte[] Hash(this byte[] data, string hashName)
        {
            HashAlgorithm algorithm;
            if (string.IsNullOrEmpty(hashName))
                algorithm = HashAlgorithm.Create();
            else
                algorithm = HashAlgorithm.Create(hashName);
            return algorithm.ComputeHash(data);
        }
        //使用默认算法Hash
        public static byte[] Hash(this byte[] data)
        {
            return Hash(data, null);
        }
        #endregion

        #region 位运算
        //index从0开始
        //获取取第index是否为1
        public static bool GetBit(this byte b, int index)
        {
            return (b & (1 << index)) > 0;
        }
        //将第index位设为1
        public static byte SetBit(this byte b, int index)
        {
            b |= (byte)(1 << index);
            return b;
        }
        //将第index位设为0
        public static byte ClearBit(this byte b, int index)
        {
            b &= (byte)((1 << 8) - 1 - (1 << index));
            return b;
        }
        //将第index位取反
        public static byte ReverseBit(this byte b, int index)
        {
            b ^= (byte)(1 << index);
            return b;
        }

        #endregion

        #region To内存流
        public static MemoryStream ToMemoryStream(this byte[] data)
        {
            return new MemoryStream(data);
        }
        #endregion
    }
}
