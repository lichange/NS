using Microsoft.EntityFrameworkCore;
using NS.DDD.Core.Repository;
using NS.DDD.Core.UnitOfWork;

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
    public interface IEntityFrameworkRepositoryContext : IRepositoryContext
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

        #region Methods

        /// <summary>
        /// 将指定的聚合根标注为“新建”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterNewEntity<TAggregateRoot>(TAggregateRoot obj)
            where TAggregateRoot : class;
        /// <summary>
        /// 将指定的聚合根标注为“更改”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterModifiedEntity<TAggregateRoot>(TAggregateRoot obj)
            where TAggregateRoot : class;
        /// <summary>
        /// 将指定的聚合根标注为“删除”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterDeletedEntity<TAggregateRoot>(TAggregateRoot obj)
            where TAggregateRoot : class ;

        #endregion
    }
}
