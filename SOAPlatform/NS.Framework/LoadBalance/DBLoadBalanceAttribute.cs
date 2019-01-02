using NS.Framework.LoadBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class |
         AttributeTargets.Method,
         Inherited = true)]
    public class DBLoadBalanceAttribute : Attribute
    {
        /// <summary>
        /// 默认
        /// </summary>
        public DBLoadBalanceAttribute()
        {
            LoadBalanceDbType = LoadBalanceDbType.Best;
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        public DBLoadBalanceAttribute(LoadBalanceDbType dbType)
        {
            LoadBalanceDbType = dbType;
        }

        /// <summary>
        ///  数据库类型
        /// </summary>
        public LoadBalanceDbType LoadBalanceDbType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否运用负载均衡
        /// </summary>
        public bool IsDBLoadBalance
        {
            get
            {
                try
                {
                    return Convert.ToBoolean(Config.PlatformConfig.ServerConfig.KeyValueSettings["DBLoadBalance"].Value);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
