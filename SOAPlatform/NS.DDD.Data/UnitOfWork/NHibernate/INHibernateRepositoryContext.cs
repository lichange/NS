using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.Repository;
using NS.Component.Data;

namespace NS.DDD.Data
{
    public interface INHibernateRepositoryContext : IRepositoryContext
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储上下文所使用的Entity Framework的<see cref="DbContext"/>实例。
        /// </summary>
        IPersistenceDAL Persistence { get; }
        #endregion
    }

    public interface INHibernateRepositoryContextBase : IRepositoryContextBase
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储上下文所使用的Entity Framework的<see cref="DbContext"/>实例。
        /// </summary>
        IPersistenceDAL Persistence
        {
            get;
        }
        #endregion
    }
}
