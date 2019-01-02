using NS.Framework.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.LoadBalance
{
    /// <summary>
    /// 派发器
    /// </summary>
    public static class DbLoadBalanceDispatcher
    {
        /// <summary>
        /// 获取负载均衡的数据库字符串
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public static string GetConnectionString(string key)
        {
            IDbLoadBalanceRouter tempRouter = ObjectContainer.CreateInstance<IDbLoadBalanceRouter>();

            return tempRouter.GetDbConnectionString(LoadBalanceDbType.Best, key);
        }
    }
}
