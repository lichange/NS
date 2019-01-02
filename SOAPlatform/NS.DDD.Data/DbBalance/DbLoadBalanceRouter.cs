using NS.Framework.Attributes;
using NS.Framework.LoadBalance;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data
{
    /// <summary>
    /// 数据库负载均衡辅助类
    /// </summary>
    [Export(typeof(DbLoadBalanceRouter))]
    public class DbLoadBalanceRouter : IDbLoadBalanceRouter
    {
        private readonly string defaultConnectionStringKey="DefaultConnectionString";

        /// <summary>
        /// 根据配置的关键字
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回对应的具体的字符串连接对象</returns>
        public string GetDbConnectionString(LoadBalanceDbType dbType,string key)
        {
            string connString = this.GetDefaultConnectionString();
            switch (dbType)
            {
                case LoadBalanceDbType.Main:
                    connString = this.GetMainConnectionString(key);
                    break;
                default:
                    connString = this.GetBestConnectionString(key);
                    break;
            }
            return connString;
        }

        /// <summary>
        /// 获取最优选择数据库连接字符串
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回对应的字符串</returns>
        private string GetBestConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return GetDefaultConnectionString();

            var bestConnectionString = NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConnectionStrings.Where(pre => pre.Key.ToLower() == key.ToLower() && pre.DbType!= LoadBalanceDbType.Main).FirstOrDefault();
            return bestConnectionString.Value;
        }

        /// <summary>
        /// 获取主数据库连接字符串
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回对应的字符串</returns>
        private string GetMainConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return GetDefaultConnectionString();

            var mainConnectionString = NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConnectionStrings.Where(pre => pre.Key.ToLower() == key.ToLower() && pre.DbType == LoadBalanceDbType.Main).FirstOrDefault();
            return mainConnectionString.Value;
        }

        /// <summary>
        /// 获取默认选择数据库连接字符串
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>返回对应的字符串</returns>
        private string GetDefaultConnectionString()
        {
            return NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConnectionStrings.Where(pre => pre.Key.ToLower() == defaultConnectionStringKey.ToLower()).FirstOrDefault().Value;
        }
    }
}
