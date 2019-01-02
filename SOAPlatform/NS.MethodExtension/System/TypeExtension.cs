using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Type 类型的扩展方法
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 检查目标类型是否是制定的类型
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Check(this System.Type Type, object Value)
        {
            return Check(Type.FullName, Value);
        }

        public static bool Check(this string Type, object Value)
        {
            if (Type == null)
            {
                return false;
            }
            else if (
                ((Type == typeof(long).FullName) && Value == null) ||
                ((Type == typeof(bool).FullName) && Value == null) ||
                ((Type == typeof(char).FullName) && Value == null) ||
                ((Type == typeof(byte).FullName) && Value == null) ||
                ((Type == typeof(int).FullName) && Value == null) ||
                ((Type == typeof(double).FullName) && Value == null) ||
                ((Type == typeof(System.DateTime).FullName) && Value == null) ||
                ((Type == typeof(System.TimeSpan).FullName) && Value == null) ||
                (Value != null && Type != Value.GetType().FullName))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

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

        public static string GetDefaultStringValue(this System.Type Type)
        {
            object defaultValue;
            if (Type == typeof(bool))
            {
                return "false";
            }
            else if ((defaultValue = GetDefaultValue(Type)) == null)
            {
                return "";
            }
            else
            {
                return defaultValue.ToString();
            }
        }
    }
}
