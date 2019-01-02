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
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class, IAggregateRoot, new()
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储所使用的仓储上下文实例。
        /// </summary>
        IRepositoryContext Context { get; }
        #endregion

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        object Add(TEntity insertItem);

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

        TEntity GetObjectByPrimaryKey(object key);

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

        /// <summary>
        /// 高性能插入数据-10w数据在7秒左右，解决EF多数据插入性能问题-该方式转用Ado.net
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities);
    }

    /// <summary>
    /// Represents the sorting style.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Indicates that the sorting style is not specified.
        /// </summary>
        Unspecified = -1,
        /// <summary>
        /// Indicates an ascending sorting.
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// Indicates a descending sorting.
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// </summary>
    /// <remarks>
    /// Indeed, one might think that IDbSet already a generic repository and therefore
    /// would not need this item. Using this interface allows us to ensure PI principle
    /// within our domain model
    /// </remarks>
    public interface IRepository : IDisposable
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储所使用的仓储上下文实例。
        /// </summary>
        IRepositoryContext Context
        {
            get;
        }
        #endregion

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        object Add<TEntity>(TEntity insertItem) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Add<TEntity>(IList<TEntity> insertList) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update<TEntity>(TEntity updateItem) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update<TEntity>(IList<TEntity> updateList) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete<TEntity>(TEntity insertItem) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete<TEntity>(IList<TEntity> deleteList) where TEntity : class, IAggregateRoot, new();

        TEntity GetObjectByPrimaryKey<TEntity>(object key) where TEntity : class, IAggregateRoot, new();

        TEntity GetObjectByPrimaryKey<TEntity>(Func<TEntity, bool> filter) where TEntity : class, IAggregateRoot, new();

        IList<TEntity> Query<TEntity>(Func<TEntity, bool> filter) where TEntity : class, IAggregateRoot, new();

        IList<TEntity> AllItems<TEntity>() where TEntity : class, IAggregateRoot, new();

        IList<TResult> GetPageInfo<TResult,TEntity>(int pageRecordCount, Func<TEntity, TResult> propertySelector,
                                            Func<TEntity, bool> filter, ref int recordCount) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 以指定的排序字段和排序方式，以及分页参数，从仓储中查找所有聚合根。
        /// </summary>
        /// <param name="sortPredicate">用于表述排序字段的Lambda表达式。</param>
        /// <param name="sortOrder">排序方式。</param>
        /// <param name="pageNumber">分页的页码。</param>
        /// <param name="pageSize">分页的页面大小。</param>
        /// <returns>带有分页信息的聚合根集合。</returns>
        IList<TEntity> GetPageInfo<TEntity>(Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 以指定的排序字段和排序方式，以及分页参数，从仓储中查找所有聚合根。
        /// </summary>
        /// <param name="queryPredicate">用于过滤信息的Lambda表达式。</param>
        /// <param name="sortPredicate">用于表述排序字段的Lambda表达式。</param>
        /// <param name="sortOrder">排序方式。</param>
        /// <param name="pageNumber">分页的页码。</param>
        /// <param name="pageSize">分页的页面大小。</param>
        /// <returns>带有分页信息的聚合根集合。</returns>
        IList<TEntity> GetPageInfo<TEntity>(Expression<Func<TEntity, bool>> queryPredicate, Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize) where TEntity : class, IAggregateRoot, new();

        bool TransactionAction(Action<object[]> action, params object[] objs);

        bool TransactionAction(Action action);

        /// <summary>
        /// 高性能插入数据-10w数据在7秒左右，解决EF多数据插入性能问题-该方式转用Ado.net
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities) where TEntity : class, IAggregateRoot, new();
    }

    /// <summary>
    /// Base interface for implement a "Repository Pattern", for
    /// </summary>
    /// <remarks>
    /// Indeed, one might think that IDbSet already a generic repository and therefore
    /// would not need this item. Using this interface allows us to ensure PI principle
    /// within our domain model
    /// </remarks>
    public interface IRepositoryBase : IDisposable
    {
        #region Properties
        /// <summary>
        /// 获取当前仓储所使用的仓储上下文实例。
        /// </summary>
        IRepositoryContextBase Context
        {
            get;
        }
        #endregion

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        object Add(object insertItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Add(IList<object> insertList);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update(object updateItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Update(IList<object> updateList);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete(object insertItem);

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete(IList<object> deleteList);

        object GetObjectByPrimaryKey(string key);
        
        bool TransactionAction(Action<object[]> action, params object[] objs);

        bool TransactionAction(Action action);
    }
}
