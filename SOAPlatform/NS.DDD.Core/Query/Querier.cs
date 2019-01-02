using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NS.DDD.Core.Repository;

namespace NS.DDD.Core
{
    /// <summary>
    /// 查询器接口定义-负责处理特殊业务的查询处理
    /// </summary>
    /// <typeparam name="TDomainModel">领域模型类型</typeparam>
    public abstract class Querier<TDomainModel> : IQuerier<TDomainModel> where TDomainModel : class, new()
    {
        protected readonly IQuerierContext _querierContext;

        public Querier(IQuerierContext querierContext)
        {
            this._querierContext = querierContext;
        }

        /// <summary>
        /// 获取所有领域对象信息
        /// </summary>
        /// <returns>返回LIst集合</returns>
        public abstract IList<TDomainModel> FindAll();

        /// <summary>
        /// 根据主键获取领域模型信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract TDomainModel FindByPrimaryKey(object key);

        /// <summary>
        /// Get all elements (Paged)
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Page count</param>
        /// <returns></returns>
        public abstract IList<TDomainModel> FindAll(int pageIndex, int pageCount);

        /// <summary>
        /// Get all by query (Expression)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <returns>List of qualified entities</returns>
        public abstract IList<TDomainModel> FindBy(Expression<Func<TDomainModel, bool>> query);

        /// <summary>
        /// Get all by query (Expression) (Paged)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageCount">Page Count</param>
        /// <returns></returns>
        public abstract IList<TDomainModel> FindBy(Expression<Func<TDomainModel, bool>> query, int pageIndex,
                                                   int pageCount);

        #region IDisposable 成员

        public void Dispose()
        {
            this.ExplicitDispose();
        }

        #region Protected Methods
        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">A <see cref="System.Boolean"/> value which indicates whether
        /// the object should be disposed explicitly.</param>
        protected abstract void Dispose(bool disposing);
        /// <summary>
        /// Provides the facility that disposes the object in an explicit manner,
        /// preventing the Finalizer from being called after the object has been
        /// disposed explicitly.
        /// </summary>
        protected void ExplicitDispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #endregion

        public abstract IList<TDomainModel> FindBy(Expression<Func<TDomainModel, bool>> query, Expression<Func<TDomainModel, dynamic>> order, int pageIndex, int pageCount);

        public abstract IList<TDomainModel> FindAll(Expression<Func<TDomainModel, dynamic>> order, int pageIndex, int pageCount);

        /// <summary>
        /// 获取Max列返回的对象
        /// </summary>
        /// <param name="max">Max比较列</param>
        /// <returns></returns>
        public abstract dynamic Max(Expression<Func<TDomainModel, dynamic>> max);

        /// <summary>
        /// 获取当前类型的总记录数
        /// </summary>
        /// <returns></returns>
        public abstract int Count();

        /// <summary>
        /// Get all by query (Expression) (Paged)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageCount">Page Count</param>
        /// <param name="rowCount">根据条件查询的总记录数</param>
        /// <returns></returns>
        public abstract IList<TDomainModel> FindBy(Expression<Func<TDomainModel, bool>> query,
            int pageIndex, int pageCount, OrderType orderType, out int rowCount);

        /// <summary>
        /// Get all by query (Expression) (Paged)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <param name="order">Expression to evaluate</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageCount">Page Count</param>
        /// <param name="rowCount">根据条件查询的总记录数</param>
        /// <returns></returns>
        public abstract IList<TDomainModel> FindBy<TKey>(Expression<Func<TDomainModel, bool>> query,
            Expression<Func<TDomainModel, TKey>> order, int pageIndex, int pageCount, OrderType orderType, out int rowCount);

        /// <summary>
        /// 获取条件获取满足条件的总记录数
        /// </summary>
        /// <param name="max">实体类型</param>
        /// <returns></returns>
        public abstract int Count(Expression<Func<TDomainModel, bool>> query);

        public abstract IList<TDomainModel> FindBy<TKey>(System.Linq.Expressions.Expression<Func<TDomainModel, bool>> query,
            System.Linq.Expressions.Expression<Func<TDomainModel, TKey>> order);

        //泛型方法用于执行标量存储过程
        //public abstract T ExecuteFunction<T>(string functionName, System.Data.EntityClient.EntityParameter[] parameters);
        public abstract T ExecuteFunction<T>(string functionName, IDictionary<string, object> parameters);
        //泛型方法用于执行标量存储过程
        //public abstract IList<T> ExecuteFunctions<T>(string functionName, System.Data.EntityClient.EntityParameter[] parameters);

        public abstract IList<T> ExecuteFunctions<T>(string functionName, IDictionary<string, object> parameters);

        public abstract IList<T> ExecuteSQL<T>(string sqlString);

        /// <summary>
        /// 获取查询接口
        /// </summary>
        /// <returns>返回的结果集合</returns>
        public abstract IQueryable<TDomainModel> GetQueryable();

        /// <summary>
        /// 分组查询方法
        /// </summary>
        /// <typeparam name="TKey">类型</typeparam>
        /// <param name="query">查询表达式</param>
        /// <param name="groupBy">分组表达式</param>
        /// <param name="order">排序表达式</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageCount">页面记录数</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="rowCount">返回总记录数</param>
        /// <returns></returns>
        public abstract IList<TDomainModel> FindGroupBy<TKey>(Expression<Func<TDomainModel, bool>> query,
            Expression<Func<TDomainModel, TKey>> groupBy,
            Expression<Func<TDomainModel, TKey>> order,
          int pageIndex, int pageCount, OrderType orderType, out int rowCount);

        /// <typeparam name="TKey">类型</typeparam>
        /// <param name="query">查询表达式</param>
        /// <param name="groupBy">分组表达式</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="rowCount">返回总记录数</param>
        public abstract IList<TDomainModel> FindGroupBy<TKey>(Expression<Func<TDomainModel, bool>> query,
            Expression<Func<TDomainModel, TKey>> groupBy,
            Expression<Func<TDomainModel, TKey>> order,
          OrderType orderType, out int rowCount);

    }
}
