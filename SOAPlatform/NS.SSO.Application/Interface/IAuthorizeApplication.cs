using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.SSO.Application
{
    /// <summary>
    /// sso授权服务接口-可自定义实现
    /// </summary>
    public interface IAuthorizeApplication
    {
        UserWithACL GetAccessedControls(string username);

    }
}
