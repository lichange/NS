using Newtonsoft.Json;
using NS.Framework.Global;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace NS.Component.Utility
{
    /// <summary>
    /// 管理提供者
    /// </summary>
    public class ManageProvider : IManageProvider
    {
        private IManageUser currentUser;

        #region 静态实例
        /// <summary>当前提供者</summary>
        public static IManageProvider Provider
        {
            get
            {
                return new ManageProvider();
            }
        }
        #endregion

        /// <summary>
        /// 秘钥
        /// </summary>
        private string LoginUserKey = "LoginUserKey";
        /// <summary>
        /// 登陆提供者模式:Session、Cookie 
        /// </summary>
        private string LoginProvider = ConfigHelper.AppSettings("LoginProvider");
        /// <summary>
        /// 写入登录信息
        /// </summary>
        /// <param name="user">成员信息</param>
        public virtual void AddCurrent(IManageUser user)
        {
            try
            {
                this.currentUser = user;

                if (LoginProvider == "Cookie")
                {
                    CookieHelper.WriteCookie(LoginUserKey, JsonConvert.SerializeObject(user));
                }
                else
                {
                    SessionHelper.Add(LoginUserKey, JsonConvert.SerializeObject(user));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public virtual IManageUser Current()
        {
            try
            {
                IManageUser user = new IManageUser();
                if (LoginProvider == "Cookie")
                {
                    user = JsonConvert.DeserializeObject<IManageUser>(CookieHelper.GetCookie(LoginUserKey));
                }
                else
                {
                    var tempUser = SessionHelper.Get(LoginUserKey);
                    if (tempUser != null)
                        user = JsonConvert.DeserializeObject<IManageUser>(tempUser.ToString());
                }
                if (user == null)
                {
                    throw new Exception("登录信息超时，请重新登录。");
                }
                this.currentUser = user;
                return user;
            }
            catch
            {
                throw new Exception("登录信息超时，请重新登录。");
            }
        }
        /// <summary>
        /// 删除登录信息
        /// </summary>
        public virtual void EmptyCurrent()
        {
            if (LoginProvider == "Cookie")
            {
                NSHttpContext.Current.Response.Cookies.Append(LoginUserKey.Trim(), DateTime.Now.AddYears(-5).ToString());
            }
            else
            {
                SessionHelper.Remove(LoginUserKey.Trim());
            }
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOverdue()
        {
            object str = "";
            if (LoginProvider == "Cookie")
            {
                str = CookieHelper.GetCookie(LoginUserKey);
            }
            else
            {
                str = SessionHelper.Get(LoginUserKey);
            }
            if (str != null && str.ToString() != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前缓存的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetCurrentUser<T>() where T : class
        {
            var tempJson = this.Current().CurrentUser.ToString();
            T tempvalue = JsonConvert.DeserializeObject<T>(tempJson);

            return tempvalue;
        }
    }
}
