using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Component.Utility.OAuth
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public interface IUserInterface
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        dynamic GetUserInfo();

        /// <summary>
        /// 关闭Session
        /// </summary>
        void EndSession();
    }
}
