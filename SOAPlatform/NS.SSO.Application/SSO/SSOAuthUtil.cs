﻿using NS.Framework.IOC;
using NS.SSO.Application.Cache;

using System;

namespace NS.SSO.Application
{
    public class SSOAuthUtil
    {
        public static LoginResult Parse(PassportLoginRequest model)
        {
            var result = new LoginResult();
            try
            {
                model.Trim();
                //获取应用信息
                var appInfo = new AppInfoService().Get(model.AppKey);
                if (appInfo == null)
                {
                    throw new Exception("应用不存在");
                }
                //获取用户信息

                User userInfo = null;
                if (model.Account == "System")
                {
                    userInfo = new User
                    {
                        Id = Guid.Empty.ToString(),  //TODO:可以根据需要调整
                        Account = "System",
                        Name = "超级管理员",
                        Password = "123456"
                    };
                }
                else
                {
                    var usermanager = ObjectContainer.CreateInstance<IUserManagementApplication>();
                    userInfo = usermanager.GetUser(model.Account);
                }

                if (userInfo == null)
                {
                    throw new Exception("用户不存在");
                }
                if (userInfo.Password != model.Password)
                {
                    throw new Exception("密码错误");
                }

                var currentSession = new UserAuthSession
                {
                    Account = model.Account,
                    Name = userInfo.Name,
                    Token = Guid.NewGuid().ToString().GetHashCode().ToString("x"),
                    AppKey = model.AppKey,
                    CreateTime = DateTime.Now,
                    IpAddress = HttpContext.Current.Request.UserHostAddress
                };

                //创建Session
                //new ObjCacheProvider<UserAuthSession>().Create(currentSession.Token, currentSession, DateTime.Now.AddDays(10));
                //创建Session
                UserSessionCacheProvider.Instance.CreateNewUserSesssion(currentSession.Token, currentSession, 10);

                result.Code = 200;
                result.ReturnUrl = appInfo.ReturnUrl;
                result.Token = currentSession.Token;

            }
            catch (Exception ex)
            {
                result.Code = 500;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}