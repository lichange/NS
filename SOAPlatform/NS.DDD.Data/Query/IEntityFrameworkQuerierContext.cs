using Microsoft.EntityFrameworkCore;
using NS.DDD.Core;
using NS.DDD.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data
{
    /// <summary>
    /// EntityFramework 仓储上下文
    /// </summary>
    public interface IEntityFrameworkQuerierContext : IQuerierContext
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储上下文所使用的Entity Framework的<see cref="DbContext"/>实例。
        /// </summary>
        DbContext Context
        {
            get;
        }
        #endregion
    }
}
