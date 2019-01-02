using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NS.Component.Data.Impl
{
    public class PersistenceDAL : IPersistenceDAL
    {
        private IDataAccessor dataAccessor;
        public PersistenceDAL(string connectionString)
        {
            dataAccessor = DataAccessorBuilder.CreateDataAccessor(connectionString);
        }

        #region IPersistenceDAL 成员

        public bool PersistenceToDatabase<T>(IList<T> insertList, IList<T> updateList, IList<T> deleteList) where T : class, new()
        {
            try
            {
                Dictionary<object, ChangeType> tempDictionary = new Dictionary<object, ChangeType>();

                if (insertList != null)
                    foreach (var itemT in insertList)
                    {
                        tempDictionary.Add(itemT, ChangeType.Add);
                    }

                if (updateList != null)
                    foreach (var itemT in updateList)
                    {
                        tempDictionary.Add(itemT, ChangeType.Update);
                    }

                if (deleteList != null)
                    foreach (var itemT in deleteList)
                    {
                        tempDictionary.Add(itemT, ChangeType.Delete);
                    }

                dataAccessor.AcceptChanges(tempDictionary);
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
                return false;
            }

            return true;
        }

        public bool Save<T>(T insertItem) where T : class, new()
        {
            try
            {
                dataAccessor.UpdateData<T>(insertItem);
            }
            catch (Exception)
            {
                return false;
#if DEBUG
                throw;
#endif
            }
            return true;
        }

        public bool Save<T>(IList<T> insertOrUpdateList) where T : class, new()
        {
            try
            {
                dataAccessor.UpdateData<T>(insertOrUpdateList);
            }
            catch (Exception)
            {
                return false;
#if DEBUG
                throw;
#endif
            }
            return true;
        }

        public bool Delete<T>(T insertItem) where T : class, new()
        {
            try
            {
                dataAccessor.DeleteData<T>(insertItem);
            }
            catch (Exception)
            {
                return false;
#if DEBUG
                throw;
#endif
            }
            return true;
        }

        public bool Delete<T>(IList<T> deleteList) where T : class, new()
        {
            try
            {
                dataAccessor.DeleteData<T>(deleteList);
            }
            catch (Exception)
            {
                return false;
#if DEBUG
                throw;
#endif
            }
            return true;
        }

        #endregion

        #region IEntityCollection<T> 成员

        public IList<T1> GetPageInfo<T, T1>(int pageRecordCount, Func<T, T1> propertySelector, Func<T, bool> filter, ref int recordCount) where T : class, new()
        {
            IList<T1> tempT1List = new List<T1>();

            try
            {
                IEnumerable<T> tList = dataAccessor.QueryDataEx<T>(pre => pre.List<T>().Where(filter).ToList());
                tempT1List = tList.Select(propertySelector).ToList();
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            return tempT1List;
        }

        #endregion

        #region IPersistenceDAL 成员

        public T GetObjectByPrimaryKey<T>(Func<T, bool> filter) where T : class, new()
        {
            try
            {
                //IList<T> tList = dataAccessor.QueryDataEx<T>(pre => pre.List<T>().Where(filter).ToList());

                //if (tList.Count() > 0)
                //    return tList[0];

                return null;
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            return null;
        }

        public IList<T> Query<T>(Func<T, bool> filter) where T : class, new()
        {
            try
            {
                IList<T> tList = dataAccessor.QueryDataEx<T>(pre => pre.List<T>().Where(filter).ToList());

                return tList;
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }

            return null;
        }

        #endregion

        #region IPersistenceDAL 成员

        public IList<T> AllItems<T>() where T : class, new()
        {
            try
            {
                IList<T> tList = dataAccessor.QueryData<T>().ToList();
                return tList;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif

            }
            return null;
        }

        #endregion

        public bool TestConnection()
        {
            return dataAccessor.TestConnection();
        }

        #region IPersistenceDAL 成员

        public void LazyLoad<T>(Action<T> lazyAction, T data) where T : class
        {
            dataAccessor.LazyLoad(lazyAction, data);
        }

        public T LazyLoad<T>(string identifier, Expression<Func<T, object>> property) where T : class
        {
            return dataAccessor.LazyLoad<T>(identifier, property);
        }

        public bool TransactionAction(Action<object[]> action, params object[] objs)
        {
            return dataAccessor.TransactionAction(action, objs);
        }

        public bool TransactionAction(Action action)
        {
            return dataAccessor.TransactionAction(action);
        }

        #endregion

        public void Dispose()
        {
            this.dataAccessor.Dispose();
        }


        public IList<T> ExecStoreProduce<T>(string storeName, IDictionary<string, object> parameters)
        {
            return this.dataAccessor.ExecStoreProduce<T>(storeName, parameters);
        }


        public IList<T> ExecuteSQL<T>(string sqlString) where T : class, new()
        {
            IList<T> cc = new List<T>();
            this.dataAccessor.sqlExecuteNonQuery(sqlString);
            return cc;
        }

        public object ExecuteQuery(string sqlString)
        {
            return this.dataAccessor.sqlExecute(sqlString);
        }
    }
}
