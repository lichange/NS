
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.Dto;

namespace NS.DDD.Core
{
    /// <summary>
    /// 本地离线同步数据存储服务
    /// </summary>
    public interface ILocalDataSyncService
    {
        /// <summary>
        /// 同步离线数据库中的数据到服务器上
        /// </summary>
        /// <returns>返回操作的结果</returns>
        bool Sync(DataSyncContract dataContract);

    }
}
