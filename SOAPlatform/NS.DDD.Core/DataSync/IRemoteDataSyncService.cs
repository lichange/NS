
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
    /// 远程服务器同步数据服务
    /// </summary>
    public interface IRemoteDataSyncService
    {
        /// <summary>
        /// 同步离线数据库中的数据到服务器上
        /// </summary>
        /// <returns>返回操作的结果</returns>
        bool Sync(DataSyncContract dataContract);
        /// <summary>
        /// 同步离线数据库中的数据到云端上
        /// </summary>
        /// <returns>返回操作的结果</returns>
        bool SyncClound(DataSyncContract dataContract);

        /// <summary>
        /// 获取服务器上所有的待同步的数据
        /// </summary>
        /// <returns>返回结果集合</returns>
        IList<ServerDataSyncContract> GetSyncDatas();

        /// <summary>
        /// 获取服务器上指定实体类型的待同步的数据
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns></returns>
        IList<ServerDataSyncContract> GetSyncDatas(string entityType);
    }
}
