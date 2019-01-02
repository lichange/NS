using System;
using System.Collections.Generic;
using System.Reflection;

namespace NS.Component.Utility
{
    public static class ConvertUtility
    {
        #region 基本值类型转换
        /// <summary>
        /// 字符串转换成byte类型（转换失败返回0）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成byte类型</returns>
        public static byte ToByte(this string input)
        {
            return input.ToByte(0);
        }
        /// <summary>
        /// 字符串转换成byte类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成byte类型</returns>
        public static byte ToByte(this string input, byte defaultValue)
        {
            byte iReturn;
            if (byte.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成short类型（转换失败返回0）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成short类型</returns>
        public static Int16 ToInt16(this string input)
        {
            return input.ToInt16(0);
        }
        /// <summary>
        /// 字符串转换成short类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成short类型</returns>
        public static Int16 ToInt16(this string input, Int16 defaultValue)
        {
            Int16 iReturn;
            if (Int16.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成int类型（转换失败返回0）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成int类型</returns>
        public static Int32 ToInt32(this string input)
        {
            return input.ToInt32(0);
        }
        /// <summary>
        /// 字符串转换成int类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成int类型</returns>
        public static Int32 ToInt32(this string input, Int32 defaultValue)
        {
            Int32 iReturn;
            if (Int32.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成Int64类型（转换失败返回0）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成Int64类型</returns>
        public static Int64 ToInt64(this string input)
        {
            return input.ToInt64(0);
        }
        /// <summary>
        /// 字符串转换成Int64类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成Int64类型</returns>
        public static Int64 ToInt64(this string input, Int64 defaultValue)
        {
            Int64 iReturn;
            if (Int64.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成Decimal类型（转换失败返回0m）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成Decimal类型</returns>
        public static Decimal ToDecimal(this string input)
        {
            return input.ToDecimal(0m);
        }
        /// <summary>
        /// 字符串转换成Decimal类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成Decimal类型</returns>
        public static Decimal ToDecimal(this string input, Decimal defaultValue)
        {
            Decimal iReturn;
            if (Decimal.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成Single类型（转换失败返回0f）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成Single类型</returns>
        public static Single ToSingle(this string input)
        {
            return input.ToSingle(0f);
        }
        /// <summary>
        /// 字符串转换成Single类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成Single类型</returns>
        public static Single ToSingle(this string input, Single defaultValue)
        {
            Single iReturn;
            if (Single.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 字符串转换成Boolean类型（转换失败返回false）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns>字符串转换成Boolean类型</returns>
        public static Boolean ToBoolean(this string input)
        {
            return input.ToBoolean(false);
        }
        /// <summary>
        /// 字符串转换成Boolean类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成Boolean类型</returns>
        public static Boolean ToBoolean(this string input, Boolean defaultValue)
        {
            Boolean iReturn;
            if (Boolean.TryParse(input, out iReturn))
                return iReturn;
            return false;
        }

        /// <summary>
        /// 字符串转换成DateTime类型（转换失败返回1900-1-1）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成DateTime类型</returns>
        public static DateTime ToDateTime(this string input)
        {
            return input.ToDateTime(new DateTime(1900, 1, 1));
        }
        /// <summary>
        /// 字符串转换成DateTime类型（转换失败返回默认值）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="defaultValue">转换失败时默认值</param>
        /// <returns>字符串转换成DateTime类型</returns>
        public static DateTime ToDateTime(this string input, DateTime defaultValue)
        {
            DateTime iReturn;
            if (DateTime.TryParse(input, out iReturn))
                return iReturn;
            return defaultValue;
        }

        /// <summary>
        /// 把可空时间类型转换为字符
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime? dt)
        {
            if (dt == null)
            {
                return string.Empty;
            }
            else
            {
                return dt.Value.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 把字符串数组转化为整形数组
        /// </summary>
        /// <param name="arrstr"></param>
        /// <returns></returns>
        public static int[] StrArrayConvertIntArray(string[] arrstr)
        {
            if (arrstr == null || arrstr.Length == 0)
            {
                return new int[0];
            }
            int[] ia = new int[arrstr.Length];
            for (int i = 0; i < arrstr.Length; i++)
            {
                ia[i] = int.Parse(arrstr[i]);
            }
            return ia;
        }
        #endregion

        /// <summary>
        /// 获取属性key value 对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> GetPropertiesValueDict<T>(T t)
        {
            IDictionary<String, Object> dict = new Dictionary<String, Object>();
            if (t == null)
            {
                return dict;
            }
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return dict;
            }
            foreach (PropertyInfo item in properties)
            {
                dict.Add(item.Name, item.GetValue(t, null));
            }
            return dict;
        }

        /// <summary>
        /// 获取表单数据转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="form"></param>
        /// <returns></returns>
        public static T ToModel<T>(this System.Collections.Specialized.NameValueCollection form, params string[] notcontainerfields) where T : new()
        {
            T model = new T();
            Type type = typeof(T);
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo p in ps)
            {
                if (!form[p.Name].IsEmpty()&&!p.Name.EqualsIgnoreCase(notcontainerfields))
                {
                    type.GetProperty(p.Name).SetValue(model, Convert.ChangeType(form[p.Name], p.PropertyType), null);
                }

                //if (p.Name == type.Name + "Id")
                //{
                //    type.GetProperty(p.Name).SetValue(model, Convert.ChangeType(form["ID"], p.PropertyType), null);
                //}
            }
            return model;
        }
    }
}
