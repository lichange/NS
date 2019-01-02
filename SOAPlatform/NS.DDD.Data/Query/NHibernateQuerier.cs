using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.DDD.Core;
using NS.DDD.Core.Repository;

namespace NS.DDD.Data
{
    public class NHibernateQuerier<TModelType> : Querier<TModelType>
        where TModelType : class, new()
    {
        private INHibernateQuerierContext tempContext;
        public NHibernateQuerier(INHibernateQuerierContext context)
            : base(context)
        {
            this.tempContext = context;
        }

        public override IList<TModelType> FindAll()
        {
            return this.tempContext.Persistence.AllItems<TModelType>();
        }

        public override IList<TModelType> FindAll(int pageIndex, int pageCount)
        {
            return this.tempContext.Persistence.AllItems<TModelType>().Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query)
        {
            return this.tempContext.Persistence.Query(query.Compile());
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, int pageIndex, int pageCount)
        {
            return this.tempContext.Persistence.Query(query.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public INHibernateQuerierContext QuerierContext
        {
            get
            {
                return this.tempContext;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tempContext.Dispose();
                base.Dispose();
            }
        }

        public override TModelType FindByPrimaryKey(object key)
        {
            //return this.tempContext.Persistence.Query<TModelType>(pre=>pre)
            return default(TModelType);
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, dynamic>> order, int pageIndex, int pageCount)
        {
            return this.tempContext.Persistence.Query(query.Compile()).OrderBy(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public override IList<TModelType> FindAll(System.Linq.Expressions.Expression<Func<TModelType, dynamic>> order, int pageIndex, int pageCount)
        {
            return this.tempContext.Persistence.AllItems<TModelType>().OrderBy(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        /// <summary>
        /// 获取Max列返回的对象
        /// </summary>
        /// <param name="max">Max比较列</param>
        /// <returns></returns>
        public override dynamic Max(System.Linq.Expressions.Expression<Func<TModelType, dynamic>> max)
        {
            return this.tempContext.Persistence.AllItems<TModelType>().Max(max.Compile());
        }

        /// <summary>
        /// 获取当前类型的总记录数
        /// </summary>
        /// <param name="max">实体类型</param>
        /// <returns></returns>
        public override int Count()
        {
            return this.tempContext.Persistence.AllItems<TModelType>().Count();
        }

        public override IList<TModelType> FindBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, TKey>> order)
        {
            try
            {
                return this.tempContext.Persistence.Query<TModelType>(query.Compile()).OrderBy(order.Compile()).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            rowCount = this.Count(query);
            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return this.tempContext.Persistence.Query(query.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            else
                return this.tempContext.Persistence.Query(query.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public override IList<TModelType> FindBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, TKey>> order, int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            rowCount = this.Count(query);
            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return this.tempContext.Persistence.Query(query.Compile()).OrderBy(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            else
                return this.tempContext.Persistence.Query(query.Compile()).OrderByDescending(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public override int Count(System.Linq.Expressions.Expression<Func<TModelType, bool>> query)
        {
            return this.tempContext.Persistence.Query(query.Compile()).Count();
        }

        //泛型方法用于执行标量存储过程
        public override T ExecuteFunction<T>(string functionName, IDictionary<string, object> parameters)
        {
            return default(T);
        }

        //泛型方法用于执行标量存储过程
        public override IList<T> ExecuteFunctions<T>(string functionName, IDictionary<string, object> parameters)
        {
            return this.tempContext.Persistence.ExecStoreProduce<T>(functionName, parameters);
        }

        //private void FillListFromDataReader<T>(System.Data.EntityClient.EntityDataReader obj, ref List<T> tempList)
        //{
        //    //TODO...
        //}

        public override IQueryable<TModelType> GetQueryable()
        {
            throw new NotImplementedException();
        }

        public override IList<TModelType> FindGroupBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, TKey>> groupBy, System.Linq.Expressions.Expression<Func<TModelType, TKey>> order, int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            throw new NotImplementedException();
        }

        public override IList<TModelType> FindGroupBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, TKey>> groupBy, System.Linq.Expressions.Expression<Func<TModelType, TKey>> order, OrderType orderType, out int rowCount)
        {
            throw new NotImplementedException();
        }


        public override IList<T> ExecuteSQL<T>(string sqlString)
        {

            throw new NotImplementedException();
        }


    }

    public class NHibernateQuerierBase
    {
        private INHibernateQuerierContext tempContext;
        public NHibernateQuerierBase(INHibernateQuerierContext context)
        {
            this.tempContext = context;
        }

        public INHibernateQuerierContext QuerierContext
        {
            get
            {
                return this.tempContext;
            }
        }

        ////泛型方法用于执行标量存储过程
        //public  object ExecuteFunctions(string functionName, IDictionary<string, object> parameters)
        //{
        //    return this.tempContext.Persistence.ExecStoreProduce(functionName, parameters);
        //}

        public object ExecuteSQL(string sqlString)
        {
            return this.tempContext.Persistence.ExecuteQuery(sqlString);
        }
    }
}
