using NS.Framework.Cache;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.SSO.Application
{
    public class UserSessionCacheProvider
    {
        private static readonly object lock_flag = new object();
        private static UserSessionCacheProvider _factory = null;
        private ICacheProviderEx cacheProvider = null;

        public static UserSessionCacheProvider Instance
        {
            get
            {
                if (_factory == null)
                {
                    lock (lock_flag)
                    {
                        if (_factory == null)
                        {
                            _factory = new UserSessionCacheProvider();
                        }
                    }
                }
                return _factory;
            }
        }
        public void CreateNewUserSesssion(string token, UserAuthSession userAuthSession, int days)
        {
            cacheProvider.Add(token, token, userAuthSession,DateTime.Now.AddDays(days));
        }

        public UserAuthSession GetUserSession(string token)
        {
            return (UserAuthSession)cacheProvider.Get(token, token);
        }

        public void RemoveUserSession(string token)
        {
            cacheProvider.Remove(token);
        }
    }
}
