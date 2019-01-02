using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core;
using NS.Component.Data;

namespace NS.DDD.Data
{
    public interface INHibernateQuerierContext : IQuerierContext
    {
        #region Properties
        /// <summary>
        /// 获取当前持久化上下文所使用的的<see cref="Persistence"/>实例。
        /// </summary>
        IPersistenceDAL Persistence { get;  }
        #endregion
    }
}
