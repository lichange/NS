using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;

using NHibernate;

namespace NS.Component.Data
{
    /// <summary>
    /// 持久化数据访问器接口
    /// 1 提供事务和实体类数据的增删改查功能;
    /// 2 提供线程安全的操作;
    /// 3 要求类型是数据库实体类型;
    /// 4 本接口基于Nhibernate技术,如果希望以其他ORM技术实现,请实现IQueryable(T)接口;
    /// </summary>
    public interface IDataAccessor : IDisposable
    {
        /// <summary>
        /// 测试连接是否有效
        /// </summary>
        /// <returns>当数据访问器连接可用时,返回true</returns>
        bool TestConnection();

        /// <summary>
        /// 增加数据项
        /// </summary>
        /// <typeparam name="T">需要增加的实体数据类型</typeparam>
        /// <param name="data">需要增加的实体数据对象</param>
        void AddData<T>(T data) where T : class, new();

        /// <summary>
        /// 批量增加数据项
        /// </summary>
        /// <typeparam name="T">需要增加的实体数据类型</typeparam>
        /// <param name="dataList">需要增加的实体数据对象列表</param>
        void AddData<T>(IList<T> dataList) where T : class, new();

        /// <summary>
        /// 删除数据项
        /// </summary>
        /// <typeparam name="T">需要删除的实体数据类型</typeparam>
        /// <param name="data">需要删除的实体数据对象</param>
        void DeleteData<T>(T data) where T : class, new();

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <typeparam name="T">需要删除的实体数据类型</typeparam>
        /// <param name="dataList">需要删除的实体数据对象列表</param>
        void DeleteData<T>(IList<T> dataList) where T : class, new();

        /// <summary>
        /// 修改数据项
        /// </summary>
        /// <typeparam name="T">需要修改的实体数据类型</typeparam>
        /// <param name="data">需要修改的实体数据对象</param>
        void UpdateData<T>(T data) where T : class, new();

        /// <summary>
        /// 批量修改数据项
        /// </summary>
        /// <typeparam name="T">需要修改的实体数据类型</typeparam>
        /// <param name="dataList">需要修改的实体数据对象列表</param>
        void UpdateData<T>(IList<T> dataList) where T : class, new();

        /// <summary>
        /// 获取全部数据实体,在使用时请注意效率问题
        /// </summary>
        /// <typeparam name="T">需要查询的实体数据类型</typeparam>
        /// <returns>返回实体数据对象列表</returns>
        IList<T> QueryData<T>() where T : class, new();

        /// <summary>
        /// 获取全部数据实体,在使用时请注意效率问题
        /// </summary>
        /// <typeparam name="T">需要查询的实体数据类型</typeparam>
        /// <param name="sqlCondition">HQL语句</param>
        /// <returns>返回实体数据对象列表</returns>
        IList<T> QueryData<T>(string sqlCondition) where T : class, new();

        /// <summary>
        /// 基于NHibernate.IQueryOver(of T)接口进行数据查询，支持扩展方法与Lamda表达式        
        /// </summary>
        /// <typeparam name="T">需要查询的实体数据类型</typeparam>
        /// <param name="callback">使用该方法进行调用者需要的过滤</param>
        /// <returns>返回实体数据对象列表</returns>
        IList<T> QueryDataEx<T>(Func<IQueryOver<T>, IList<T>> callback) where T : class, new();

        /// <summary>
        /// 获取实体类的数据行数
        /// </summary>
        /// <typeparam name="T">实体数据类型</typeparam>
        /// <returns>返回实体类的数据行数</returns>
        int QueryDataCount<T>() where T : class, new();

        /// <summary>
        /// 根据起始位置,以及要获取的最大数据量来获取数据,可用于分页查询
        /// </summary>
        /// <typeparam name="T">实体数据类型</typeparam>
        /// <param name="firstResult">第一条可返回数据的开始位置</param>
        /// <param name="MaxResult">可返回的最大数据数量</param>
        /// <returns>返回分页数据</returns>
        IList<T> QueryData<T>(int firstResult, int MaxResult) where T : class, new();
        
        /// <summary>
        /// 开启事务,请注意,只需在对多个数据进行增删改查并需要原子操作时才使用,
        /// 当单独进行一个读写操作时,默认提供事务管理
        /// </summary>
        /// <returns>事务操作接口</returns>
        IHMTransaction BeginTransaction();

        /// <summary>
        /// 开启事务,并设置自定义的隔离级别
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别</param>
        /// <returns>事务操作接口</returns>
        IHMTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// 用于批量处理数据项
        /// </summary>
        /// <param name="changes">数据项字典,字典中的key值为数据项,value值为改变类型</param>
        void AcceptChanges(Dictionary<Object, ChangeType> changes);

        T LazyLoad<T>(string identifier, Expression<Func<T,object>> property) where T : class;

        void LazyLoad<T>(Action<T> lazyAction, T data) where T : class;

        bool TransactionAction(Action<object[]> action,params object[] objs);

        bool TransactionAction(Action action);

        IList<T> ExecStoreProduce<T>(string storeName,IDictionary<string,object> parameters);

        /// <summary>
        /// 上下文名称
        /// </summary>
        string ContextName
        {
            get;
            set;
        }
        int sqlExecuteNonQuery(string queryString);

        object sqlExecute(string queryString);
    }
}
