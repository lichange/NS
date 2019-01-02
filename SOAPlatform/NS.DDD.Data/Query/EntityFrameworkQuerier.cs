using Microsoft.EntityFrameworkCore;
using NS.DDD.Core;
using NS.DDD.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data.Querier
{
    public class EntityFrameworkQuerier<TModelType> : Querier<TModelType>, IQuerier<TModelType>
        where TModelType : class, IAggregateRoot, new()
    {
        private IEntityFrameworkQuerierContext tempContext;
        public EntityFrameworkQuerier(IEntityFrameworkQuerierContext context)
            : base(context)
        {
            this.tempContext = context;
        }

        public override IList<TModelType> FindAll()
        {
            try
            {
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                this.DealException(ex);
            }

            return new List<TModelType>();
        }

        public override IList<TModelType> FindAll(int pageIndex, int pageCount)
        {
            try
            {
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            }
            catch (Exception ex)
            {
                this.DealException(ex);
            }

            return new List<TModelType>();
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query)
        {
            try
            {
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).ToList();
            }
            catch (Exception ex)
            {
                this.DealException(ex);
            }

            return new List<TModelType>();
        }

        public override IList<TModelType> FindBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, System.Linq.Expressions.Expression<Func<TModelType, TKey>> order)
        {
            try
            {
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).OrderBy(order).ToList();
            }
            catch (Exception ex)
            {
                this.DealException(ex);
            }

            return new List<TModelType>();
        }

        private void DealException(Exception ex)
        {
            //var tempErrors = ex.EntityValidationErrors;
            //var tempValidateResult = string.Empty;
            //if (tempErrors != null && tempErrors.Count() > 0)
            //{
            //    foreach (var tempError in tempErrors)
            //    {
            //        if (tempError == null || tempError.ValidationErrors.Count == 0)
            //            continue;

            //        foreach (var validationError in tempError.ValidationErrors)
            //        {
            //            tempValidateResult += string.Format("[{0}:{1}];", validationError.PropertyName, validationError.ErrorMessage);
            //        }
            //    }
            //}
            throw new Framework.Exceptions.ValidationException(ex.Message);
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query, int pageIndex, int pageCount)
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public IEntityFrameworkQuerierContext QuerierContext
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
            return this.tempContext.Context.Set<TModelType>().Find(key);
        }

        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query,
            System.Linq.Expressions.Expression<Func<TModelType, dynamic>> order, int pageIndex, int pageCount)
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).OrderBy(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        public override IList<TModelType> FindAll(System.Linq.Expressions.Expression<Func<TModelType, dynamic>> order,
            int pageIndex, int pageCount)
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().OrderBy(order.Compile()).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        /// <summary>
        /// 获取Max列返回的对象
        /// </summary>
        /// <param name="max">Max比较列</param>
        /// <returns></returns>
        public override dynamic Max(System.Linq.Expressions.Expression<Func<TModelType, dynamic>> max)
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().Max(max.Compile());
        }

        /// <summary>
        /// 获取当前类型的总记录数
        /// </summary>
        /// <param name="max">实体类型</param>
        /// <returns></returns>
        public override int Count()
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().Count();
        }

        /// <summary>
        /// Get all by query (Expression) (Paged)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageCount">Page Count</param>
        /// <param name="rowCount">根据条件查询的总记录数</param>
        /// <returns></returns>
        public override IList<TModelType> FindBy(System.Linq.Expressions.Expression<Func<TModelType, bool>> query,
            int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            if (pageIndex < 0 || pageCount < 0)
            {
                rowCount = 0;
                return new List<TModelType>();
            }

            rowCount = this.Count(query);

            if (rowCount <= 0)
                return new List<TModelType>();

            if (pageIndex == 0)
                pageIndex = 1;

            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            else
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        /// <summary>
        /// Get all by query (Expression) (Paged)
        /// </summary>
        /// <param name="query">Expression to evaluate</param>
        /// <param name="order">Expression to evaluate</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageCount">Page Count</param>
        /// <param name="rowCount">根据条件查询的总记录数</param>
        /// <returns></returns>
        public override IList<TModelType> FindBy<TKey>(System.Linq.Expressions.Expression<Func<TModelType, bool>> query,
            System.Linq.Expressions.Expression<Func<TModelType, TKey>> order, int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            if (pageIndex < 0 || pageCount < 0)
            {
                rowCount = 0;
                return new List<TModelType>();
            }

            rowCount = this.Count(query);

            if (rowCount <= 0)
                return new List<TModelType>();

            if (pageIndex == 0)
                pageIndex = 1;

            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).OrderBy(order).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            else
                return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).OrderByDescending(order).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }

        #region 执行分组查询

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
        public override IList<TModelType> FindGroupBy<TKey>(Expression<Func<TModelType, bool>> query,
            Expression<Func<TModelType, TKey>> groupBy,
            Expression<Func<TModelType, TKey>> order,
          int pageIndex, int pageCount, OrderType orderType, out int rowCount)
        {
            var tempList = new List<TModelType>();
            if (pageIndex < 0 || pageCount < 0)
            {
                rowCount = 0;
                return tempList;
            }

            var tempSelectList = this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).GroupBy(groupBy).Select(g => new
            {
                Group =
                    g.Key,
                Member = g
            });

            rowCount = tempSelectList.Count();

            if (rowCount <= 0)
                return tempList;

            if (pageIndex == 0)
                pageIndex = 1;

            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return tempSelectList.Select(pre => pre.Member.OrderBy(order.Compile()).FirstOrDefault()).OrderBy(order).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
            else
                return tempSelectList.Select(pre => pre.Member.OrderBy(order.Compile()).FirstOrDefault()).OrderByDescending(order).Skip((pageIndex - 1) * pageCount).Take(pageCount).ToList();
        }
        /// <typeparam name="TKey">类型</typeparam>
        /// <param name="query">查询表达式</param>
        /// <param name="groupBy">分组表达式</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="count">返回总记录数</param>
        public override IList<TModelType> FindGroupBy<TKey>(Expression<Func<TModelType, bool>> query,
            Expression<Func<TModelType, TKey>> groupBy,
            Expression<Func<TModelType, TKey>> order,
          OrderType orderType, out int count)
        {
            var tempList = new List<TModelType>();
            var tempSelectList = this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).GroupBy(groupBy).Select(g => new
            {
                Group =
                    g.Key,
                Member = g
            });

            count = tempSelectList.Count();

            if (orderType == OrderType.ASC || orderType == OrderType.Default)
                return tempSelectList.Select(pre => pre.Member.OrderBy(order.Compile()).FirstOrDefault()).ToList();
            else
                return tempSelectList.Select(pre => pre.Member.OrderByDescending(order.Compile()).FirstOrDefault()).ToList();
        }
        #endregion

        /// <summary>
        /// 获取条件获取满足条件的总记录数
        /// </summary>
        /// <param name="max">实体类型</param>
        /// <returns></returns>
        public override int Count(System.Linq.Expressions.Expression<Func<TModelType, bool>> query)
        {
            return this.tempContext.Context.Set<TModelType>().AsNoTracking().Where(query).Count();
        }

        //public abstract T ExecuteFunction<T>(string functionName, System.Data.EntityClient.EntityParameter[] parameters);
        //泛型方法用于执行标量存储过程
        public override T ExecuteFunction<T>(string functionName, IDictionary<string, object> parameters)
        {
            try
            {
                var tempList = this.ExecuteFunctions<T>(functionName, parameters);
                if (tempList != null && tempList.Count > 0)
                    return tempList[0];

                return default(T);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
            }
        }
        //public abstract IList<T> ExecuteFunctions<T>(string functionName, System.Data.EntityClient.EntityParameter[] parameters);

        //泛型方法用于执行标量存储过程
        public override IList<T> ExecuteFunctions<T>(string functionName, IDictionary<string, object> parameters)
        {
            throw new Exception(".net standard 暂不支持执行存储过程");
            //try
            //{
            //    StringBuilder stringBuilder = new StringBuilder();
            //    System.Data.SqlClient.SqlParameter[] tempParameters = new System.Data.SqlClient.SqlParameter[parameters.Count + 1];

            //    //循环处理parameter
            //    if (parameters != null && parameters.Count > 0)
            //    {
            //        int i = 0;
            //        foreach (var item in parameters)
            //        {
            //            System.Data.SqlClient.SqlParameter sqlParameter = new System.Data.SqlClient.SqlParameter(string.Format("@{0}", item.Key), item.Value);

            //            tempParameters[i] = sqlParameter;

            //            stringBuilder.Append(string.Format("@{0}", item.Key));
            //            stringBuilder.Append(",");
            //        }

            //        System.Data.SqlClient.SqlParameter outParameter = new System.Data.SqlClient.SqlParameter("@outResult", System.Data.SqlDbType.Variant);
            //        tempParameters[i] = outParameter;

            //        stringBuilder.Append("@outResult output");

            //        tempParameters[parameters.Count].Direction = System.Data.ParameterDirection.Output;
            //    }

            //    var slt = this.tempContext.Context.Database.SqlQuery<T>(string.Format("exec {0} {1}", functionName, stringBuilder.ToString()), tempParameters);
            //    return slt.ToList();
            //}
            //catch (System.Exception)
            //{
            //    throw;
            //}
            //finally
            //{

            //}
        }

        private IList<T> TestExecuteFunctions<T>()
        {
            //TODO...
            IList<T> tempList = new List<T>();

            var tempParameters = new Dictionary<string, object>();
            tempParameters.Add("name", "张三");
            tempParameters.Add("idcard", "65222345666622732222");

            //getorderlist是存储过程名字，目前该方法需要定义返回值，
            //存储过程必须有返回值，默认定义的参数是@outResult，否则会出错
            //tempParameters 是存储过程的参数，定义的格式是@参数名称，@参数值 诸如 [name,张三]
            //程序默认会把 [name,张三] 转化为 （@name,张三），请不要传错，@name与存储过程中的参数名需要一致
            tempList = this.ExecuteFunctions<T>("getorderlist", tempParameters);

            return tempList;
        }

        /// <summary>
        /// 获取查询接口
        /// </summary>
        /// <returns>返回的结果集合</returns>
        public override IQueryable<TModelType> GetQueryable()
        {
            return this.tempContext.Context.Set<TModelType>();
        }

        public override IList<T> ExecuteSQL<T>(string sqlString)
        {
            throw new NotImplementedException();
        }
    }
}
