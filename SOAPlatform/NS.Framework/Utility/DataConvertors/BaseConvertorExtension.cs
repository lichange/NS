using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Utility.DataConvertors
{
    /// <summary>
    /// 基础的数据类型-转换-扩展方法
    /// </summary>
    public static class BaseConvertorExtension
    {
        /// <summary>
        /// 16进制->整型
        /// </summary>
        /// <param name="HexChar"></param>
        /// <returns></returns>
        private static int HexCharToInt(this char HexChar)
        {
            if (HexChar == '0')
            {
                return 0;
            }
            else if (HexChar == '1')
            {
                return 1;
            }
            else if (HexChar == '2')
            {
                return 2;
            }
            else if (HexChar == '3')
            {
                return 3;
            }
            else if (HexChar == '4')
            {
                return 4;
            }
            else if (HexChar == '5')
            {
                return 5;
            }
            else if (HexChar == '6')
            {
                return 6;
            }
            else if (HexChar == '7')
            {
                return 7;
            }
            else if (HexChar == '8')
            {
                return 8;
            }
            else if (HexChar == '9')
            {
                return 9;
            }
            else if (HexChar == 'a' || HexChar == 'A')
            {
                return 10;
            }
            else if (HexChar == 'b' || HexChar == 'B')
            {
                return 11;
            }
            else if (HexChar == 'c' || HexChar == 'C')
            {
                return 12;
            }
            else if (HexChar == 'd' || HexChar == 'D')
            {
                return 13;
            }
            else if (HexChar == 'e' || HexChar == 'E')
            {
                return 14;
            }
            else if (HexChar == 'f' || HexChar == 'F')
            {
                return 15;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 16进制->长整型
        /// </summary>
        /// <param name="HexChar"></param>
        /// <returns></returns>
        public static long HexStringToLong(this string HexString)
        {
            if (HexString == null || HexString.Length == 0)
            {
                return 0;
            }
            else
            {
                long v = 0;
                for (int count = 0; count < HexString.Length; count++)
                {
                    char c = HexString[count];
                    v = v * 16 + HexCharToInt(c);
                }
                return v;
            }
        }

        /// <summary>
        /// 将源类型转换为目标类型
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ConversionType"></param>
        /// <returns></returns>
        public static object Convert(this object Source, System.Type ConversionType)
        {
            return Convert(Source, ConversionType, true);
        }

        /// <summary>
        /// 将源类型转换为目标类型
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="ConversionType"></param>
        /// <returns></returns>
        public static object Convert(this object Source, System.Type ConversionType, bool ThrowException)
        {
            if (Source != null && Source.GetType() == ConversionType)
            {
                return Source;
            }
            else if (Source == null)
            {
                return GetDefaultValue(ConversionType);
            }
            try
            {
                if (ConversionType == typeof(System.DateTime))
                {
                    return System.DateTime.Parse(Source.ToString());
                }
                else if (ConversionType == typeof(System.TimeSpan))
                {
                    return System.TimeSpan.Parse(Source.ToString());
                }
                else if (ConversionType.IsArray && (Source is string && (string)Source == ""))
                {
                    return null;
                }
                else
                {
                    return System.Convert.ChangeType(Source, ConversionType);
                }
            }
            catch (Exception ex)
            {
                if (ThrowException)
                {
                    throw ex;
                }
                else
                {
                    return GetDefaultValue(ConversionType);
                }
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(this System.Type Type)
        {
            if (Type == typeof(string))
            {
                return null;
            }
            else if (Type == typeof(System.DateTime))
            {
                return System.DateTime.MinValue;
            }
            else if (Type == typeof(System.Boolean))
            {
                return false;
            }
            else if (Type == typeof(System.Double))
            {
                return 0.0;
            }
            else if (Type == typeof(System.Int32))
            {
                return 0;
            }
            else if (Type == typeof(System.Int64))
            {
                return 0;
            }
            else if (Type == typeof(System.TimeSpan))
            {
                return new System.TimeSpan(0);
            }
            else if (Type == typeof(System.Data.DataTable))
            {
                return null;
            }
            else if (Type == typeof(System.Data.SqlTypes.SqlDateTime))
            {
                return System.Data.SqlTypes.SqlDateTime.MinValue;
            }
            else
            {
                return null;
            }
        }

        public static string ToShortGuid(Guid Guid)
        {
            byte[] array = Guid.ToByteArray();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            long l = 0;
            string s;
            for (int count = 0; count < 15; count = count + 5)
            {
                l = array[count] * 256 * 16 + array[count + 1] * 16 + (array[count + 2] / 16);
                s = ToBase32String(l, 4);
                builder.Append(s);
                l = (array[count + 2] % 16) * 256 * 256 + array[count + 3] * 256 + array[count + 4];
                s = ToBase32String(l, 4);
                builder.Append(s);
            }
            s = ToBase32String((long)array[15], 2);
            builder.Append(s);
            return builder.ToString();
        }

        private static string ToBase32String(long Value, int Length)
        {
            string s = ToBase32String(Value);
            int zeroCount = Length - s.Length;
            for (int count = 0; count < zeroCount; count++)
            {
                s = s.Insert(0, "0");
            }
            return s;
        }

        public static string ToBase32String(byte Value)
        {
            return ToBase32String((long)Value);
        }

        public static string ToBase32String(long Value)
        {
            if (Value < 0)
            {
                return "-" + ToUnsignedBase32String(0 - Value);
            }
            else if (Value == 0)
            {
                return "0";
            }
            else
            {
                return ToUnsignedBase32String(Value);
            }
        }

        private static string ToUnsignedBase32String(long Value)
        {
            if (Value < 0)
            {
                throw new NotImplementedException();
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            long last = Value;
            while (last > 0)
            {
                int i = (int)(last % 32);
                char c = ToBase32Char(i);
                builder.Insert(0, c);

                last = (long)(last / 32);
            }

            if (builder.Length == 0)
            {
                return "0";
            }
            else
            {
                return builder.ToString();
            }
        }

        private static char ToBase32Char(int Value)
        {
            if (Value < 10)
            {
                char baseChar = '0';
                int baseInt = (int)baseChar;
                int charValue = Value + baseInt;
                char c = (char)charValue;
                return c;
            }
            else if (Value < 32)
            {
                char baseChar = 'a';
                int baseInt = (int)baseChar;
                int charValue = Value + baseInt - 10;
                char c = (char)charValue;
                return c;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
