using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using NS.Component.NHibernate;
using NHibernate.Engine;

namespace NS.Component.Data.Impl
{
    /// <summary>
    /// NHibernate数据访问类，负责实现基于NHibernate的持久化数据存取服务;
    /// 实现线程安全,1 首先保证一个实例中一次只有一个方法被一个线程执行;
    /// 2 保证事务不会被重复启动;
    /// 3 在事务结束前调用Dispose将无效;
    /// </summary>
    internal class WCFNHibernateDataAccessor : IDataAccessor
    {
        private readonly object syncLock = new object();
        private readonly DataAccessorConfiguration config;
        private System.Data.IsolationLevel IsoLevel = System.Data.IsolationLevel.Unspecified;
        private ISession singletonSession;

        private ISession GetSession()
        {
            //if (NS.Component.NHibernate.NHibernateContext.Current != null)
            //    singletonSession= NS.Component.NHibernate.NHibernateContext.Current.Session;
            //else
            singletonSession = NHibernateFactory.OpenSession();

            return singletonSession;
        }

        public WCFNHibernateDataAccessor(DataAccessorConfiguration config)
        {
            this.config = config;
            if (config.IsoLevel != null)
                IsoLevel = config.IsoLevel.Value;

            NHibernateFactory.Initialize(this.config);
        }

        #region 接口实现

        public bool TestConnection()
        {
            try
            {
                lock (syncLock)
                {
                    ISession sess = GetSession();
                    bool result = sess.IsConnected;
                    sess.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #region 增删改

        public void AddOrUpdate<T>(T data)
            where T : class, new()
        {
            IList<T> dataList = new List<T>();
            dataList.Add(data);
            AddOrUpdate<T>(dataList);
        }

        public void AddOrUpdate<T>(IList<T> dataList)
            where T : class, new()
        {
            lock (syncLock)
            {
                ISession Session = GetSession();
                try
                {
                    Session.FlushMode = FlushMode.Commit;
                    ITransaction trans = BeginTransaction(Session);
                    try
                    {
                        foreach (T data in dataList)
                        {
                            var tempData = data;
                            if (Session.Contains(data))
                            {
                                var tempIdentifier = Session.GetIdentifier(data);
                                if (tempIdentifier != null)
                                {
                                    var tempT = Session.Get<T>(tempIdentifier);
                                    if (tempT != null)
                                        //var entityData= Session.SessionFactory.GetClassMetadata(data.GetType());
                                        Session.Replicate(data.GetType().Name, data, ReplicationMode.Overwrite);
                                }
                                //var tempData = Session.Merge(data);
                            }
                            else
                            {
                                var metaData = Session.SessionFactory.GetClassMetadata(data.GetType());
                                if (metaData == null)
                                    metaData = Session.SessionFactory.GetClassMetadata(data.GetType().BaseType);
                                var tempIden = metaData.GetIdentifier(data);
                                tempData = Session.Load<T>(tempIden);

                                Session.Replicate(data.GetType().Name, data, ReplicationMode.Overwrite);
                            }
                            Session.SaveOrUpdate(data);
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
                finally
                {
                    Session.Close();
                }
            }
        }

        public void Delete<T>(T data)
            where T : class, new()
        {
            IList<T> dataList = new List<T>();
            dataList.Add(data);
            Delete<T>(dataList);
        }

        public void Delete<T>(IList<T> dataList)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        Session.FlushMode = FlushMode.Commit;
                        ITransaction trans = BeginTransaction(Session);
                        try
                        {
                            foreach (T data in dataList)
                            {
                                var tempData = data;
                                if (Session.Contains(data))
                                {
                                    var tempIdentifier = Session.GetIdentifier(data);
                                    if (tempIdentifier != null)
                                    {
                                        var tempT = Session.Get<T>(tempIdentifier);
                                        if (tempT != null)
                                            //var entityData= Session.SessionFactory.GetClassMetadata(data.GetType());
                                            Session.Replicate(data.GetType().Name, data, ReplicationMode.LatestVersion);
                                    }
                                    //var tempData = Session.Merge(data);
                                }
                                else
                                {
                                    var metaData = Session.SessionFactory.GetClassMetadata(data.GetType());
                                    if (metaData == null)
                                        metaData = Session.SessionFactory.GetClassMetadata(data.GetType().BaseType);
                                    var tempIden = metaData.GetIdentifier(data);
                                    tempData = Session.Load<T>(tempIden);

                                    Session.Replicate(data.GetType().Name, data, ReplicationMode.Overwrite);
                                }
                                Session.Delete(data);
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            trans.Dispose();
                        }
                    }
                    finally
                    {
                        Session.Close();
                    }
                }
            }
        }

        private ITransaction BeginTransaction(ISession Session)
        {
            ITransaction trans = IsoLevel == IsolationLevel.Unspecified ? Session.BeginTransaction() : Session.BeginTransaction(IsoLevel);
            return trans;
        }

        public void AcceptChanges(Dictionary<object, ChangeType> changes)
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    Session.FlushMode = FlushMode.Commit;
                    try
                    {
                        ITransaction trans = BeginTransaction(Session);
                        try
                        {
                            foreach (var item in changes)
                            {
                                var classMetadata = Session.SessionFactory.GetClassMetadata(item.Key.GetType());
                                if (classMetadata == null)
                                    classMetadata = Session.SessionFactory.GetClassMetadata(item.Key.GetType().BaseType);
                                switch (item.Value)
                                {
                                    case ChangeType.Add:
                                    case ChangeType.Update:
                                        if (Session.Contains(item.Key))
                                        {
                                            var tempIdentifier = Session.GetIdentifier(item.Key);
                                            if (tempIdentifier != null)
                                            {
                                                var tempT = Session.Get(item.Key.GetType().Name, tempIdentifier);
                                                if (tempT != null)
                                                    //var entityData= Session.SessionFactory.GetClassMetadata(data.GetType());
                                                    Session.Replicate(item.Key.GetType().Name, item.Key, ReplicationMode.Overwrite);
                                            }
                                            //var tempData = Session.Merge(data);
                                        }
                                        else
                                        {
                                            var metaData = Session.SessionFactory.GetClassMetadata(item.Key.GetType());
                                            if (metaData == null)
                                                metaData = Session.SessionFactory.GetClassMetadata(item.Key.GetType().BaseType);
                                            var tempIden = metaData.GetIdentifier(item.Key);
                                            var tempData = Session.Load(item.Key.GetType().Name, tempIden);

                                            Session.Replicate(item.Key.GetType().Name, item.Key, ReplicationMode.Overwrite);
                                        }
                                        Session.SaveOrUpdate(item.Key);
                                        break;
                                    case ChangeType.Delete:
                                        Session.Replicate(classMetadata.EntityName, item.Key, ReplicationMode.Overwrite);
                                        Session.Delete(item.Key);
                                        break;
                                }
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            trans.Dispose();
                        }
                    }
                    finally
                    {
                        Session.Close();
                    }
                }
            }
        }

        #endregion

        #region 查询

        public IList<T> Query<T>()
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        return Session.CreateCriteria(typeof(T)).List<T>();
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public int QueryCount<T>()
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        var classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T));
                        if (classMetadata == null)
                            classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T).BaseType);
                        IQuery query = Session.CreateQuery("select count(*) from " + classMetadata.EntityName);
                        System.Collections.IEnumerable e = query.Enumerable();
                        e.GetEnumerator().MoveNext();
                        return (int)e.GetEnumerator().Current;
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public IList<T> Query<T>(int firstResult, int MaxResult)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        return Session.CreateCriteria(typeof(T)).SetFirstResult(firstResult).SetMaxResults(MaxResult).List<T>();
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public IList<T> Query<T>(string queryString)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        return Session.CreateQuery(queryString).List<T>();
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }
        //sj 1111
        public int sqlExecuteNonQuery(string queryString)
        {
            lock (syncLock)
            {
                //ISessionFactoryImplementor sfi = NHibernateFactory.GetISessionFactoryImplementor();
                using (ISession Session = GetSession())
                {
                    IDbConnection conn = Session.Connection;
                    ConnectionState connState = conn.State;
                    //if (connState == ConnectionState.Closed)
                    //{
                    //    conn.Open();
                    //}
                    IDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = queryString;
                    //string connString = conn.ConnectionString;
                    //string dbName = conn.Database;
                    try
                    {
                        int i = cmd.ExecuteNonQuery();
                        return i;
                    }
                    //catch (Exception e)
                    //{
                    //    string s = e.ToString();
                    //    return 1;
                    //}
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public int QueryCount<T>(string queryString)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        var classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T));
                        if (classMetadata == null)
                            classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T).BaseType);
                        IQuery query = Session.CreateQuery("select count(*) from (" + queryString + ")");
                        System.Collections.IEnumerable e = query.Enumerable();
                        e.GetEnumerator().MoveNext();
                        return (int)e.GetEnumerator().Current;
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public IList<T> Query<T>(string queryString, int firstResult, int maxResults)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        var classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T));
                        if (classMetadata == null)
                            classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T).BaseType);
                        IQuery query = Session.CreateQuery(queryString);
                        query.SetFirstResult(firstResult);
                        query.SetMaxResults(maxResults);
                        return query.List<T>();
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        public IList<T> Query<T>(Func<IQueryOver<T, T>, IList<T>> customQuery)
            where T : class, new()
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    try
                    {
                        if (customQuery == null)
                            return Session.QueryOver<T>().List<T>();
                        else
                            return customQuery(Session.QueryOver<T>());
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        #endregion

        #endregion

        #region 和Dispose相关的代码

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (config != null)
                        NHibernateDataAccessorManager.Clear(config);
                }
                disposed = true;
            }
        }

        ~WCFNHibernateDataAccessor()
        {
            Dispose(false);
        }

        public bool IsDisposed
        {
            get
            {
                return disposed;
            }
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region IDataAccessor 成员

        public void AddData<T>(T data) where T : class, new()
        {
            this.AddOrUpdate<T>(data);
        }

        public void AddData<T>(IList<T> dataList) where T : class, new()
        {
            this.AddOrUpdate<T>(dataList);
        }

        public void DeleteData<T>(T data) where T : class, new()
        {
            this.Delete<T>(data);
        }

        public void DeleteData<T>(IList<T> dataList) where T : class, new()
        {
            this.Delete<T>(dataList);
        }

        public void UpdateData<T>(T data) where T : class, new()
        {
            this.AddOrUpdate<T>(data);
        }

        public void UpdateData<T>(IList<T> dataList) where T : class, new()
        {
            this.AddOrUpdate<T>(dataList);
        }

        public IList<T> QueryData<T>() where T : class, new()
        {
            return Query<T>();
        }

        public IList<T> QueryData<T>(string sqlCondition) where T : class, new()
        {
            return Query<T>(sqlCondition);
        }

        public IList<T> QueryDataEx<T>(Func<IQueryOver<T>, IList<T>> callback) where T : class, new()
        {
            return Query<T>(callback);
        }

        public int QueryDataCount<T>() where T : class, new()
        {
            return QueryCount<T>();
        }

        public IList<T> QueryData<T>(int firstResult, int MaxResult) where T : class, new()
        {
            return Query<T>(firstResult, MaxResult);
        }

        public IHMTransaction BeginTransaction()
        {
            return null;
        }

        public IHMTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return null;
        }

        #endregion

        #region 延迟加载

        public void LazyLoad<T>(Action<T> lazyAction, T data) where T : class
        {
            var Session = GetSession();
            try
            {
                var tempData = data;
                if (Session.Contains(data))
                {
                    var tempIdentifier = Session.GetIdentifier(data);
                    if (tempIdentifier != null)
                    {
                        var tempT = Session.Get<T>(tempIdentifier);
                        if (tempT != null)
                            //var entityData= Session.SessionFactory.GetClassMetadata(data.GetType());
                            Session.Replicate(data.GetType().Name, data, ReplicationMode.LatestVersion);
                    }
                    //var tempData = Session.Merge(data);
                }
                else
                {
                    var metaData = Session.SessionFactory.GetClassMetadata(data.GetType());
                    if (metaData == null)
                        metaData = Session.SessionFactory.GetClassMetadata(data.GetType().BaseType);

                    if (metaData != null)
                    {
                        var tempIden = metaData.GetIdentifier(data);
                        tempData = Session.Get<T>(tempIden);
                        //Session.Replicate(metaData.EntityName, data, ReplicationMode.Overwrite);

                    }
                }
                //Session.Lock(tempData, NHibernate.LockMode.None);  
                Session.SaveOrUpdate(tempData);

                lazyAction(tempData);
            }
            finally
            {
            }
        }

        public T LazyLoad<T>(string identifier, Expression<Func<T, object>> property) where T : class
        {
            using (ISession Session = GetSession())
            {
                try
                {
                    if (string.IsNullOrEmpty(identifier))
                        return default(T);

                    if (property == null)
                        return default(T);

                    var memberExpression = property.Body as MemberExpression;
                    if (memberExpression == null)
                        return default(T);

                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    if (propertyInfo == null)
                        return default(T);


                    var getMethod = propertyInfo.GetGetMethod(true);
                    if (getMethod.IsStatic)
                        return default(T);

                    var tempData = Session.Get<T>(identifier);
                    Session.Lock(tempData, LockMode.None);

                    var tempProperty = tempData.GetType().GetProperty(propertyInfo.Name);
                    var lazyLoadValue = tempProperty.GetValue(tempData, null);
                    Session.SaveOrUpdate(tempData);
                    return tempData;
                }
                finally
                {
                    Session.Close();
                }
            }
        }

        #endregion

        #region 事务

        public bool TransactionAction(Action<object[]> tempAction, params object[] objs)
        {
            lock (syncLock)
            {
                ISession Session = GetSession();
                try
                {
                    Session.FlushMode = FlushMode.Commit;
                    ITransaction trans = BeginTransaction(Session);
                    try
                    {
                        tempAction(objs);

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
                finally
                {
                    Session.Close();
                }
            }

            return true;
        }

        public bool TransactionAction(Action tempAction)
        {
            lock (syncLock)
            {
                ISession Session = GetSession();
                try
                {
                    Session.FlushMode = FlushMode.Commit;
                    ITransaction trans = BeginTransaction(Session);
                    try
                    {
                        tempAction();

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return false;
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
                finally
                {
                    Session.Close();
                }
            }

            return true;
        }

        #endregion

        public string ContextName
        {
            get;
            set;
        }

        public IList<T> ExecStoreProduce<T>(string storeName, IDictionary<string, object> parameters)
        {
            lock (syncLock)
            {
                using (ISession Session = GetSession())
                {
                    IList<T> results = new List<T>();

                    //ISessionFactoryImplementor s = (ISessionFactoryImplementor)Session.SessionFactory;
                    //var connection = s.ConnectionProvider.Driver.CreateConnection();
                    IDbCommand cmd = Session.Connection.CreateCommand();
                    cmd.CommandText = storeName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    // 加入参数
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, object> info in parameters)
                        {
                            IDbDataParameter parameter = cmd.CreateParameter();
                            parameter.ParameterName = info.Key; // driver.FormatNameForSql( info.Name );
                            parameter.Value = info.Value;
                            cmd.Parameters.Add(parameter);
                        }
                    }

                    try
                    {
                        IDataReader reader = cmd.ExecuteReader();

                        var properties = typeof(T).GetProperties();
                        var classMetadata = Session.SessionFactory.GetClassMetadata(typeof(T));
                        var propertyNames = classMetadata.PropertyNames;

                        while (reader.Read())
                        {
                            T RowInstance = Activator.CreateInstance<T>();

                            foreach (PropertyInfo Property in properties)
                            {
                                try
                                {
                                    if (!propertyNames.Contains(Property.Name))
                                        continue;

                                    int Ordinal = reader.GetOrdinal(Property.Name);
                                    if (reader.GetValue(Ordinal) != DBNull.Value)
                                    {
                                        Property.SetValue(RowInstance, Convert.ChangeType(reader.GetValue(Ordinal), Property.PropertyType), null);
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }
                            results.Add(RowInstance);
                        }
                        //while (rs.Read())
                        //{
                        //    int fieldCount = rs.FieldCount;
                        //    object[] values = new Object[fieldCount];
                        //    for (int i = 0; i < fieldCount; i++)
                        //        values[i] = rs.GetValue(i);
                        //    //results.Add( values );
                        //}
                    }
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }

                    return results;
                }
            }
        }

        public object sqlExecute(string queryString)
        {
            lock (syncLock)
            {
                //ISessionFactoryImplementor sfi = NHibernateFactory.GetISessionFactoryImplementor();
                using (ISession Session = GetSession())
                {
                    IDbConnection conn = Session.Connection;
                    ConnectionState connState = conn.State;
                    //if (connState == ConnectionState.Closed)
                    //{
                    //    conn.Open();
                    //}
                    IDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = queryString;
                    //string connString = conn.ConnectionString;
                    //string dbName = conn.Database;
                    try
                    {
                        IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();

                        return this.ConverDateReaderToDataTable(rdr);
                    }
                    //catch (Exception e)
                    //{
                    //    string s = e.ToString();
                    //    return 1;
                    //}
                    finally
                    {
                        Session.Clear();
                        Session.Close();
                    }
                }
            }
        }

        private DataTable ConverDateReaderToDataTable(IDataReader dataReader)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                DataColumn myDateColum = new DataColumn();
                myDateColum.DataType = dataReader.GetFieldType(i);
                myDateColum.ColumnName = dataReader.GetName(i);
                dataTable.Columns.Add(myDateColum);
            }
            while (dataReader.Read())
            {
                DataRow myDateRow = dataTable.NewRow();
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    myDateRow[i] = dataReader[i].ToString();
                }
                dataTable.Rows.Add(myDateRow);
                myDateRow = null;
            }
            dataReader.Close();
            return dataTable;
        }
    }
}
