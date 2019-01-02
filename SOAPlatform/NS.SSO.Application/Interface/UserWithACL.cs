using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.SSO.Application
{
    /// <summary>
    ///  用户视图模型
    /// <para>包括用户及用户可访问的机构/资源/模块</para>
    /// <para>hegezhou修改于2016-07-19 10:57:31</para>
    /// </summary>
    public class UserWithACL
    {
        public User User
        {
            get; set;
        }
        /// <summary>
        /// 用户可以访问到的模块（包括所属角色与自己的所有模块）
        /// </summary>
        public object Modules
        {
            get; set;
        }

        //用户可以访问的资源
        public object Resources
        {
            get; set;
        }

        /// <summary>
        ///  用户所属机构
        /// </summary>
        public object Orgs
        {
            get; set;
        }

        /// <summary>
        /// 用户角色
        /// </summary>
        public object Roles
        {
            get; set;
        }
    }
}
