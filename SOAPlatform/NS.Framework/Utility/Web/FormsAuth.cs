//using Newtonsoft.Json;
//using NS.Framework.Config;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
////using System.Web;
////using System.Web.Security;

namespace NS.Framework.Utility
{
    //    public static class FormsAuth
    //    {
    //        public static void SignIn(string loginName, object userData)
    //        {
    //            var tempKeyValuePair = PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key.ToLower() == "LoginEffectiveHours").FirstOrDefault();

    //            SignIn(loginName, userData, (tempKeyValuePair==null ||tempKeyValuePair.Value == "") ? 8 : int.Parse(tempKeyValuePair.Value));
    //        }

    //        public static void SignIn(string loginName, object userData, int expireMin)
    //        {
    //            var data = JsonConvert.SerializeObject(userData);

    //            //创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
    //            var ticket = new FormsAuthenticationTicket(2,
    //                loginName, DateTime.Now, DateTime.Now.AddDays(1), true, data);

    //            //加密Ticket，变成一个加密的字符串。
    //            var cookieValue = FormsAuthentication.Encrypt(ticket);

    //            //根据加密结果创建登录Cookie
    //            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
    //            {
    //                HttpOnly = true,
    //                Secure = FormsAuthentication.RequireSSL,
    //                Domain = FormsAuthentication.CookieDomain,
    //                Path = FormsAuthentication.FormsCookiePath
    //            };
    //            if (expireMin > 0)
    //                cookie.Expires = DateTime.Now.AddMinutes(expireMin);

    //            var context = HttpContext.Current;
    //            if (context == null)
    //                throw new InvalidOperationException();

    //            //写登录Cookie
    //            context.Response.Cookies.Remove(cookie.Name);
    //            context.Response.Cookies.Add(cookie);
    //        }

    //        public static void SingOut()
    //        {
    //            FormsAuthentication.SignOut();
    //        }

    //        public static CurrentUser GetUserData()
    //        {
    //            return GetUserData<CurrentUser>();
    //        }

    //        public static T GetUserData<T>() where T : class, new()
    //        {
    //            var UserData = new T();
    //            try
    //            {
    //                var context = HttpContext.Current;
    //                var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
    //                var ticket = FormsAuthentication.Decrypt(cookie.Value);
    //                UserData = JsonConvert.DeserializeObject<T>(ticket.UserData);
    //            }
    //            catch
    //            {
    //            }

    //            return UserData;
    //        }
    //    }

    public class CurrentUser
{
    /// <summary>
    /// UserName
    /// </summary>
    public string UserName
    {
        get; set;
    }
    /// <summary>
    /// UserCode
    /// </summary>
    public string UserCode
    {
        get; set;
    }
}
}
