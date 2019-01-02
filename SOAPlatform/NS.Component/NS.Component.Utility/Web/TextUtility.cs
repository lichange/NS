using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NS.Component.Utility
{
    /// <summary>
    /// 文本基本操作类
    /// </summary>
    public static class TextUtility
    {
        /// <summary>
        /// 判断b数组中是否包含a字符串(不区分大小写)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string a, params string[] b)
        {
            if (b == null || b.Length == 0)
            {
                return false;
            }
            foreach (string s in b)
            {
                if (string.Equals(a, s, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 按位生随机码
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string CreateRandomCode(int codeCount = 4)
        {
            int number;
            string randomCode = string.Empty;
            Random random = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                number = random.Next(100);
                switch (number % 3)
                {
                    case 0:
                        randomCode += ((char)('0' + (char)(number % 10))).ToString();
                        break;
                    case 1:
                        randomCode += ((char)('a' + (char)(number % 26))).ToString();
                        break;
                    case 2:
                        randomCode += ((char)('A' + (char)(number % 26))).ToString();
                        break;
                    default:
                        break;
                }
            }
            return randomCode;
        }

        /// <summary>
        /// 判断字符串source是否为空
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string source)
        {
            return source == null || source.Trim().Length < 1;
        }

        /// <summary>
        /// EDC加密过程
        /// </summary>
        /// <param name="pToDecrypt">被加密的字符串</param>
        /// <param name="sKey">密匙（只支持8个字节的密匙）</param>
        /// <returns></returns>
        public static string ToEncrypt(this string pToEncrypt, string sKey)
        {
            //访问数据加密标准(DES)算法的加密服务提供程序 (CSP) 版本的包装对象
            DESCryptoServiceProvider Des = new DESCryptoServiceProvider();
            //建立加密对象的密钥和偏移量
            Des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法
            Des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            //把字符串放到byte数组中
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            MemoryStream ms = new MemoryStream();//创建其支持存储区为内存的流　
            //定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, Des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //上面已经完成了把加密后的结果放到内存中去
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DEC 解密过程
        /// </summary>
        /// <param name="pToDecrypt">被解密的字符串</param>
        /// <param name="sKey">密钥（只支持8个字节的密钥，同前面的加密密钥相同）</param>
        /// <returns>返回被解密的字符串</returns>
        public static string ToDecrypt(this string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider Des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            //建立加密对象的密钥和偏移量，此值重要，不能修改
            Des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            Des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, Des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="len">截取长度</param>
        /// <param name="isSuffix">是否添加后缀…</param>
        /// <returns></returns>
        public static string SubStr(this string source, int len, bool isSuffix = false)
        {
            if (source.IsEmpty())
            {
                return string.Empty;
            }
            else if (source.Length < len)
            {
                return source;
            }
            else
            {
                return source.Substring(0, len) + (isSuffix ? "..." : "");
            }
        }

        /// <summary>
        /// 提示信息条
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="alertEnum">信息类型</param>
        /// <param name="enableClose">是否显示关闭按钮</param>
        /// <returns>提示信息条</returns>
        public static string GetMessageBar(string message, AlertEnum alertEnum = AlertEnum.Default, Boolean enableClose = true)
        {
            StringBuilder sb = new StringBuilder(300);
            string type = alertEnum.ToString().EqualsIgnoreCase("default") ? "" : string.Format(" alert-{0}", alertEnum.ToString().ToLower());
            sb.AppendFormat("<div class=\"alert{0}\">", type);
            if (enableClose)
            {
                sb.Append("<button type=\"button\" class=\"close\" data-dismiss=\"alert\">&times;</button>");
            }
            sb.AppendFormat("<span class=\"message\">{0}</span></div>", message);
            return sb.ToString();
        }

        /// <summary>
        /// 1,2,4,8,16这类字符（余求和）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToOrSum(this string str)
        {
            if (str.IsEmpty())
            {
                return 0;
            }
            IList<int> list = str.Split(',').Select(p => Convert.ToInt32(p)).ToList();
            int count = 0;
            foreach (int i in list)
            {
                count = count | i;
            }
            return count;
        }
    }
}
