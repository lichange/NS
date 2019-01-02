using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.LoadBalance
{
    public interface IDbLoadBalanceRouter
    {
        /// <summary>
        /// 根据配置的关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回对应的具体的字符串连接对象</returns>
        string GetDbConnectionString(LoadBalanceDbType dbType, string key);
    }
}
