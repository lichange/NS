
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 离线数据同步服务
    /// </summary>
    public interface IOfflineSyncService
    {
        /// <summary>
        /// 同步离线数据库中的数据到服务器上
        /// </summary>
        /// <returns>返回操作的结果</returns>
        bool Sync();
    }
}
