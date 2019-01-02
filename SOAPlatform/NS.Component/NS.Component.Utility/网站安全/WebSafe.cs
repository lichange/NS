
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using NS.Framework.Global;

namespace NS.Component.Utility
{
    /// <summary>
    /// 防止跨站脚本攻击的代码
    /// </summary>
    public class WebSafe
    {
        private const string StrRegex = @"^\+/v(8|9)|\b(and|or)\b.{1,6}?(=|>|<|\bin\b|\blike\b)|/\*.+?\*/|<\s*script\b|<\s*img\b|\bEXEC\b|UNION.+?SELECT|UPDATE.+?SET|INSERT\s+INTO.+?VALUES|(SELECT|DELETE).+?FROM|(CREATE|ALTER|DROP|TRUNCATE)\s+(TABLE|DATABASE)";

        public static bool PostData()
        {
            bool result = false;
            foreach (var key in NSHttpContext.Current.Request.Form.Keys)
            {
                result = CheckData(NSHttpContext.Current.Request.Form[key].ToString());
                if (result)
                {
                    break;
                }
            }

            return result;
        }


        public static bool GetData()
        {
            bool result = false;

            foreach (var key in NSHttpContext.Current.Request.Query.Keys)
            {
                result = CheckData(NSHttpContext.Current.Request.Query[key].ToString());
                if (result)
                {
                    break;
                }
            }

            return result;
        }


        public static bool CookieData()
        {
            bool result = false;
            foreach (var key in NSHttpContext.Current.Request.Cookies.Keys)
            {
                result = CheckData(NSHttpContext.Current.Request.Cookies[key].ToString());
                if (result)
                {
                    break;
                }
            }

            return result;

        }
        public static bool referer()
        {
            bool result = false;
            return result = CheckData(Convert.ToString(NSHttpContext.Current.Request.Headers["Referer"]));
        }

        public static bool CheckData(string inputData)
        {
            if (Regex.IsMatch(inputData, StrRegex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
