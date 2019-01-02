using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.UnitOfWork;

namespace NS.DDD.Core.Repository
{
    /// <summary>
    /// 表示实现该接口的类型是仓储上下文--仅用于SharePoint。
    /// </summary>
    public interface ISharePointRepositoryContext : IUnitOfWork, IDisposable
    {
        #region Properties
        /// <summary>
        /// 获取仓储上下文的ID。
        /// </summary>
        string ID { get; }
        #endregion

        #region Methods
        /// <summary>
        /// 将指定的聚合根标注为“新建”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterNew<TAggregateRoot>(TAggregateRoot obj,string id)
            where TAggregateRoot : class,new();
        /// <summary>
        /// 将指定的聚合根标注为“更改”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterModified<TAggregateRoot>(TAggregateRoot obj, string id)
            where TAggregateRoot : class,new();
        /// <summary>
        /// 将指定的聚合根标注为“删除”状态。
        /// </summary>
        /// <typeparam name="TAggregateRoot">需要标注状态的聚合根类型。</typeparam>
        /// <param name="obj">需要标注状态的聚合根。</param>
        void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj, string id)
            where TAggregateRoot : class,new();
        #endregion
    }
}
