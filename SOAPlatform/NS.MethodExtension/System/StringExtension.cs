using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace System
{
    /// <summary>
    /// �ַ�����չ��
    /// </summary>
    public static class StringExtension
    {
        #region Split�ָ�
        /// <summary>
        /// ʹ��ǰ��ָ���������÷ָ������е��ַ���
        /// </summary>
        /// <param name="s">��Ҫ���зָ����ַ���</param>
        /// <param name="frontSeperator">ǰ�ָ���</param>
        /// <param name="backSeperator">��ָ���</param>
        /// <returns>�ָ����</returns>
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
        /// �ָ��ַ���
        /// </summary>
        /// <param name="s">��Ҫ�ָ���ַ���</param>
        /// <param name="seperator">�ָ����</param>
        /// <returns>���طָ�Ľ��(�������ָ��ַ���)��������Ϊ�գ�����0��������</returns>
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
        /// �ָ��ַ���
        /// </summary>
        /// <param name="target">��Ҫ�ָ���ַ���</param>
        /// <param name="seperators">�ָ��������</param>
        /// <param name="length">��ɷָ���ַ�������</param>
        /// <returns>���طָ�Ľ��(�������ָ��ַ���)��������Ϊ�գ�����0��������</returns>
        public static string[] Split(this string target, string[] seperators, out int length)
        {
            List<string> result = new List<string>();
            int startIndex = 0; //ÿ�β��ҵ���ʼλ��
            
            while (true)
            {
                int itemIndex =-1;
                int minItemIndex = int.MaxValue;  //����ķָ���ŵ�λ��
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
                    //�κεķָ���Ŷ�û���ҵ�
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

        #region To16����
        /// <summary>
        /// ��16�����ַ���תΪ����
        /// </summary>
        /// <param name="valueString">Դ�ַ���</param>
        /// <returns>16��������</returns>
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

                throw new Exception("�Ƿ����ַ�");
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

        #region ������չ
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

        #region  Format����
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

        #region ���RegularExpression֧��

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
