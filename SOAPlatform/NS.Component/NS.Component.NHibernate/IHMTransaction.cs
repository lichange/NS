using System;
using System.Collections.Generic;
using System.Linq;

namespace NS.Component.Data
{
    /// <summary>
    /// 持久化数据访问的事务接口
    /// </summary>
    public interface IHMTransaction : IDisposable
    {
        /// <summary>
        /// 提交数据库事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();
    }
}
