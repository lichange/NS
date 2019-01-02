// All Rights Reserved , Copyright © Learun 2013
//=====================================================================================
using NS.Framework.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace NS.Component.Utility
{
    /// <summary>
    /// Cookie帮助类
    /// </summary>
    public partial class CookieHelper
    {
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            NSHttpContext.Current.Response.Cookies.Append(strName, strValue);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            NSHttpContext.Current.Response.Cookies.Append(strName, strValue, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expires)
            });
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            return NSHttpContext.Current.Request.Cookies[strName];
        }
        /// <summary>
        /// 删除Cookie对象
        /// </summary>
        /// <param name="CookiesName">Cookie对象名称</param>
        public static void DelCookie(string CookiesName)
        {
            NSHttpContext.Current.Response.Cookies.Delete(CookiesName);
        }
    }
}
