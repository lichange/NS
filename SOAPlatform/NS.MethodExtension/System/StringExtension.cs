using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class StringExtension
    {
        #region Split分隔
        /// <summary>
        /// 使用前后分隔符号来获得分隔符号中的字符串
        /// </summary>
        /// <param name="s">将要进行分隔的字符串</param>
        /// <param name="frontSeperator">前分隔符</param>
        /// <param name="backSeperator">后分隔符</param>
        /// <returns>分隔结果</returns>
        public static String[] Split(this string s, String frontSeperator, String backSeperator)
        {
            int startIndex = 0;
            ArrayList result = new ArrayList();

            while(true)
            {
                int frontIndex = s.IndexOf(frontSeperator, startIndex);

                if ( (frontIndex + 1 > s.Length) || frontIndex == -1)
                 {
                    break;
                }
                int backIndex = s.IndexOf(backSeperator, frontIndex + 1);
                if(backIndex== -1 )
                {
                    break;
                }

                result.Add(s.Substring(frontIndex + 1, backIndex - frontIndex - 1));
                startIndex = backIndex+1;
            }

            String[] stringArray = new String[result.Count];
            result.CopyTo(stringArray);
            return stringArray;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="s">将要分割的字符串</param>
        /// <param name="seperator">分割符号</param>
        /// <returns>返回分割的结果(不包含分割字符串)，如果结果为空，返回0长的数组</returns>
        public static string[] Split(this string s , string seperator)
        {
            List<string> result = new List<string>();
            int startIndex = 0;
            while(true)
            {
                int itemIndex = s.IndexOf(seperator, startIndex);
                if(itemIndex==-1)
                {
                    string last = s.Substring(startIndex);
                    if (!string.IsNullOrEmpty(last))
                        result.Add(last);

                    break;
                }
                string value = s.Substring(startIndex, itemIndex - startIndex);
                if(!string.IsNullOrEmpty(value))
                    result.Add(value);
                startIndex = itemIndex + seperator.Length;
            }

            return result.ToArray();
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="target">将要分割的字符串</param>
        /// <param name="seperators">分割符号数组</param>
        /// <param name="length">完成分割的字符串长度</param>
        /// <returns>返回分割的结果(不包含分割字符串)，如果结果为空，返回0长的数组</returns>
        public static string[] Split(this string target, string[] seperators, out int length)
        {
            List<string> result = new List<string>();
            int startIndex = 0; //每次查找的起始位置
            
            while (true)
            {
                int itemIndex =-1;
                int minItemIndex = int.MaxValue;  //最近的分割符号的位置
                string seperator=null;

                foreach (string item in seperators)
                {
                    itemIndex= target.IndexOf(item, startIndex);
                    if(itemIndex != -1 && itemIndex < minItemIndex)
                    {
                        minItemIndex = itemIndex;
                        seperator = item;
                    }
                }

                if (minItemIndex ==int.MaxValue)
                {
                    //任何的分割符号都没有找到
                    break;
                }
                else
                {
                    result.Add(target.Substring(startIndex, minItemIndex - startIndex));
                    startIndex = minItemIndex + seperator.Length;
                }
            }
           
            length = startIndex;
            return result.ToArray();
        }

        #endregion

        #region To16进制
        /// <summary>
        /// 把16进制字符串转为数字
        /// </summary>
        /// <param name="valueString">源字符串</param>
        /// <returns>16进制数字</returns>
        public static int ToXdigitValue(this string valueString)
        {
            int sum =0; 
            for (int i = 0; i < valueString.Length; ++i)
            {
                sum *=16;
                char c = valueString[i];
                c = char.ToUpper(c);

                if(Char.IsNumber(c))
                {
                    sum += c-'0';
                    continue;
                }

                if(c>='A' && c<'G')
                {
                    sum += c - 'A'+10;
                    continue;
                }

                throw new Exception("非法的字符");
            }

            return sum;
        }
        #endregion

        #region TrimEnd
        /// <summary>
        /// Trims the end of strings.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trimStr">The trim STR.</param>
        /// <returns></returns>
        public static string TrimEnd(this string target,string trimStr)
        {
            int index = target.LastIndexOf(trimStr);
            if (index != -1)
            {
                return target.Substring(0, index+1);
            }

            return target;
        }
        #endregion

        #region 常用扩展
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsInt(this string s)
        {
            int i;
            return int.TryParse(s, out i);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static bool IsDateTime(this string s)
        {
            DateTime i;
            return DateTime.TryParse(s, out i);
        }

        public static DateTime ToDateTime(this string s)
        {
            return DateTime.Parse(s);
        }

        public static bool IsDecimal(this string s)
        {
            decimal i;
            return Decimal.TryParse(s, out i);
        }

        public static decimal ToDecimal(this string s)
        {
            return Decimal.Parse(s);
        }
        #endregion

        #region  Format整合
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
        public static string Format(string format, object arg0)
        {
            return string.Format(format, arg0);
        }
        public static string Format(string format, object arg0, object arg1)
        {
            return string.Format(format, arg0,arg1);
        }
        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            return string.Format(format, arg0,arg1,arg2);
        }
        #endregion

        #region 添加RegularExpression支撑

        public static bool IsMatch(this string s, string pattern)
        {
            if (s == null)
                return false;
            else
                return Regex.IsMatch(s, pattern);
        }

        public static string Match(this string s, string pattern)
        {
            if (s == null)
                return "";
            return Regex.Match(s, pattern).Value;
        }

        #endregion
    }
}
