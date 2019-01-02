using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.SSO.Application
{
    /// <summary>
    /// sso账户管理-可自定义实现
    /// </summary>
    public interface IUserManagementApplication
    {
        User GetUser(string account);
    }
}
