using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.UnitOfWork;
using NS.Framework.Utility;

namespace NS.DDD.Core.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity>
      where TEntity : class, IAggregateRoot, new()
    {
        #region Private Fields
        private readonly IRepositoryContext _context;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The repository context being used by the repository.</param>
        public Repository(IRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException("repositoryContext");

            this._context = context;
        }
        #endregion

         /// <summary>
        /// Finalizes the object.
        /// </summary>
        ~Repository()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 工作单元
        /// </summary>
        public IRepositoryContext Context
        {
            get
            {
                return _context;
            }
        }

        public abstract object Add(TEntity insertItem);

        public abstract bool Add(IList<TEntity> insertList);

        public abstract bool Update(TEntity updateItem);

        public abstract bool Update(IList<TEntity> updateList);

        public abstract bool Delete(TEntity insertItem);

        public abstract bool Delete(IList<TEntity> deleteList);

        public abstract TEntity GetObjectByPrimaryKey(object key);

        public abstract TEntity GetObjectByPrimaryKey(Func<TEntity, bool> filter);

        public abstract IList<TEntity> Query(Func<TEntity, bool> filter);

        public abstract IList<TEntity> AllItems();

        public abstract IList<TResult> GetPageInfo<TResult>(int pageRecordCount, Func<TEntity, TResult> propertySelector,
                                                            Func<TEntity, bool> filter, ref int recordCount);

        public abstract bool TransactionAction(Action<object[]> action, params object[] objs);

        public abstract bool TransactionAction(Action action);

        public abstract IList<TEntity> GetPageInfo(System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        public abstract IList<TEntity> GetPageInfo(System.Linq.Expressions.Expression<Func<TEntity, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        /// <summary>
        /// 高性能插入数据-10w数据在7秒左右，解决EF多数据插入性能问题-该方式转用Ado.net
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public abstract bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities);

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
    }

    public abstract class Repository : IRepository
    {
        #region Private Fields
        private readonly IRepositoryContext _context;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The repository context being used by the repository.</param>
        public Repository(IRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException("repositoryContext");

            this._context = context;
        }
        #endregion

        /// <summary>
        /// Finalizes the object.
        /// </summary>
        ~Repository()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 工作单元
        /// </summary>
        public IRepositoryContext Context
        {
            get
            {
                return _context;
            }
        }

        public abstract object Add<TEntity>(TEntity insertItem) where TEntity : class, IAggregateRoot, new();

        public abstract bool Add<TEntity>(IList<TEntity> insertList) where TEntity : class, IAggregateRoot, new();

        public abstract bool Update<TEntity>(TEntity updateItem) where TEntity : class, IAggregateRoot, new();

        public abstract bool Update<TEntity>(IList<TEntity> updateList) where TEntity : class, IAggregateRoot, new();

        public abstract bool Delete<TEntity>(TEntity insertItem) where TEntity : class, IAggregateRoot, new();

        public abstract bool Delete<TEntity>(IList<TEntity> deleteList) where TEntity : class, IAggregateRoot, new();

        public abstract TEntity GetObjectByPrimaryKey<TEntity>(object key) where TEntity : class, IAggregateRoot, new();

        public abstract TEntity GetObjectByPrimaryKey<TEntity>(Func<TEntity, bool> filter) where TEntity : class, IAggregateRoot, new();

        public abstract IList<TEntity> Query<TEntity>(Func<TEntity, bool> filter) where TEntity : class, IAggregateRoot, new();

        public abstract IList<TEntity> AllItems<TEntity>() where TEntity : class, IAggregateRoot, new();

        public abstract IList<TResult> GetPageInfo<TResult,TEntity>(int pageRecordCount, Func<TEntity, TResult> propertySelector,
                                                            Func<TEntity, bool> filter, ref int recordCount) where TEntity : class, IAggregateRoot, new();

        public abstract bool TransactionAction(Action<object[]> action, params object[] objs);

        public abstract bool TransactionAction(Action action);

        public abstract IList<TEntity> GetPageInfo<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize) where TEntity : class, IAggregateRoot, new();

        public abstract IList<TEntity> GetPageInfo<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize) where TEntity : class, IAggregateRoot, new();

        /// <summary>
        /// 高性能插入数据-10w数据在7秒左右，解决EF多数据插入性能问题-该方式转用Ado.net
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public abstract bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities) where TEntity : class, IAggregateRoot, new();

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
    }

    public abstract class RepositoryBase : IRepositoryBase
    {
        #region Private Fields
        private readonly IRepositoryContextBase _context;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The repository context being used by the repository.</param>
        public RepositoryBase(IRepositoryContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("repositoryContext");

            this._context = context;
        }
        #endregion

        /// <summary>
        /// Finalizes the object.
        /// </summary>
        ~RepositoryBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 工作单元
        /// </summary>
        public IRepositoryContextBase Context
        {
            get
            {
                return _context;
            }
        }

        public abstract object Add(object insertItem);

        public abstract bool Add(IList<object> insertList);

        public abstract bool Update(object updateItem);

        public abstract bool Update(IList<object> updateList);

        public abstract bool Delete(object insertItem);

        public abstract bool Delete(IList<object> deleteList);
        
        public abstract bool TransactionAction(Action<object[]> action, params object[] objs);

        public abstract bool TransactionAction(Action action);
        
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

        public abstract object GetObjectByPrimaryKey(string key);

        #endregion

        #endregion
    }
}
