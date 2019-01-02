using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.UnitOfWork;

namespace NS.DDD.Core.Repository
{
    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// </summary>
    /// <remarks>
    /// Indeed, one might think that IDbSet already a generic repository and therefore
    /// would not need this item. Using this interface allows us to ensure PI principle
    /// within our domain model
    /// </remarks>
    /// <typeparam name="TEntity">Type of entity for this repository </typeparam>
    public interface ISharePointRepository<TEntity,TMapEntity> : IDisposable
        where TEntity : class, IAggregateRoot, new()
        where TMapEntity : class, new()
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储所使用的仓储上下文实例。
        /// </summary>
        ISharePointRepositoryContext Context
        {
            get;
        }
        #endregion

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Add(TEntity insertItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Add(IList<TEntity> insertList);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update(TEntity updateItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update(IList<TEntity> updateList);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete(TEntity insertItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete(IList<TEntity> deleteList);

        TEntity GetObjectByPrimaryKey(Func<TEntity, bool> filter);

        IList<TEntity> Query(Func<TEntity, bool> filter);

        IList<TEntity> AllItems();

        IList<TResult> GetPageInfo<TResult>(int pageRecordCount, Func<TEntity, TResult> propertySelector,
                                            Func<TEntity, bool> filter, ref int recordCount);

        /// <summary>
        /// 以指定的排序字段和排序方式，以及分页参数，从仓储中查找所有聚合根。
        /// </summary>
        /// <param name="sortPredicate">用于表述排序字段的Lambda表达式。</param>
        /// <param name="sortOrder">排序方式。</param>
        /// <param name="pageNumber">分页的页码。</param>
        /// <param name="pageSize">分页的页面大小。</param>
        /// <returns>带有分页信息的聚合根集合。</returns>
        IList<TEntity> GetPageInfo(Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        /// <summary>
        /// 以指定的排序字段和排序方式，以及分页参数，从仓储中查找所有聚合根。
        /// </summary>
        /// <param name="queryPredicate">用于过滤信息的Lambda表达式。</param>
        /// <param name="sortPredicate">用于表述排序字段的Lambda表达式。</param>
        /// <param name="sortOrder">排序方式。</param>
        /// <param name="pageNumber">分页的页码。</param>
        /// <param name="pageSize">分页的页面大小。</param>
        /// <returns>带有分页信息的聚合根集合。</returns>
        IList<TEntity> GetPageInfo(Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        bool TransactionAction(Action<object[]> action, params object[] objs);

        bool TransactionAction(Action action);
    }
}
