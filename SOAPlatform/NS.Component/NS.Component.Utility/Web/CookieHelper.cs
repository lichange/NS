using Microsoft.AspNetCore.Http;
using NS.Framework.Global;
using System;
using System.Web;

namespace NS.Component.Utility
{
    public partial class CookieHelper
    {
        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookieName">cookieName</param>
        public static void ClearCookie(String cookieName)
        {
            NSHttpContext.Current.Response.Cookies.Delete(cookieName);
        }

        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static String GetCookieValue(String cookieName)
        {
            return NSHttpContext.Current.Request.Cookies[cookieName];
        }

        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookieName">cookie名</param>
        /// <param name="cookieValue">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        public static void SetCookie(String cookieName, String cookieValue, DateTime expires)
        {
            NSHttpContext.Current.Response.Cookies.Append(cookieName, cookieValue, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes((expires - DateTime.Now).TotalMinutes)
            });
        }
    }
}
