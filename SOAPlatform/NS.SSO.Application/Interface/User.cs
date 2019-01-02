using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.SSO.Application
{
    /// <summary>
    /// SSO 统一登录用户信息
    /// </summary>
    public class User
    {
        public User()
        {
            this.Account = string.Empty;
            this.Password = string.Empty;
            this.Name = string.Empty;
            this.Sex = 0;
            this.Status = 0;
            this.BizCode = string.Empty;
            this.CreateTime = DateTime.Now;
            this.CrateId = string.Empty;
            this.TypeName = string.Empty;
            this.TypeId = string.Empty;
        }

        /// <summary>
        /// 用户登录帐号
        /// </summary>
        public string Id
        {
            get; set;
        }

        /// <summary>
        /// 用户登录帐号
        /// </summary>
        public string Account
        {
            get; set;
        }
        /// <summary>
	    /// 密码
	    /// </summary>
        public string Password
        {
            get; set;
        }
        /// <summary>
	    /// 用户姓名
	    /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
	    /// 性别
	    /// </summary>
        public int Sex
        {
            get; set;
        }
        /// <summary>
	    /// 用户状态
	    /// </summary>
        public int Status
        {
            get; set;
        }
        /// <summary>
	    /// 业务对照码
	    /// </summary>
        public string BizCode
        {
            get; set;
        }
        /// <summary>
	    /// 经办时间
	    /// </summary>
        public System.DateTime CreateTime
        {
            get; set;
        }
        /// <summary>
	    /// 创建人
	    /// </summary>
        public string CrateId
        {
            get; set;
        }
        /// <summary>
	    /// 分类名称
	    /// </summary>
        public string TypeName
        {
            get; set;
        }
        /// <summary>
	    /// 分类ID
	    /// </summary>
        public string TypeId
        {
            get; set;
        }
    }
}
