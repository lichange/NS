using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NS.Component.Data
{
    public interface IPersistenceDAL : IDisposable
    {
        /// <summary>
        /// 测试连接情况
        /// </summary>
        /// <returns></returns>
        bool TestConnection();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool PersistenceToDatabase<T>(IList<T> insertList, IList<T> updateList, IList<T> deleteList) where T : class,new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Save<T>(T insertItem) where T : class,new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Save<T>(IList<T> insertOrUpdateList) where T : class,new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete<T>(T insertItem) where T : class,new();

        /// <summary>
        /// 将列表中的更改反映在数据库中
        /// </summary>
        bool Delete<T>(IList<T> deleteList) where T : class,new();

        T GetObjectByPrimaryKey<T>(Func<T, bool> filter) where T : class,new();

        IList<T> Query<T>(Func<T, bool> filter) where T : class,new();

        IList<T> AllItems<T>() where T : class,new();

        IList<T1> GetPageInfo<T, T1>(int pageRecordCount, Func<T, T1> propertySelector, Func<T, bool> filter, ref int recordCount) where T : class,new();

        void LazyLoad<T>(Action<T> lazyAction, T data) where T : class;
        T LazyLoad<T>(string identifier, Expression<Func<T, object>> property) where T : class;

        bool TransactionAction(Action<object[]> action, params object[] objs);

        bool TransactionAction(Action action);

        IList<T> ExecStoreProduce<T>(string storeName, IDictionary<string, object> parameters);

        IList<T> ExecuteSQL<T>(string sqlString) where T : class, new();
        
        object ExecuteQuery(string sqlString);
    }
}
