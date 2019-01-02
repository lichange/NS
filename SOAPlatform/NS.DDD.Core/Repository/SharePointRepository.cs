using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NS.DDD.Core.UnitOfWork;
using NS.Framework.Utility;

namespace NS.DDD.Core.Repository
{
    public abstract class SharePointRepository<TEntity, TEntityMap> : DisposableObject, ISharePointRepository<TEntity, TEntityMap>
      where TEntity :class, IAggregateRoot ,new()
        where TEntityMap : class, new()
    {
        #region Private Fields
        private readonly ISharePointRepositoryContext _context;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>Repository&lt;TAggregateRoot&gt;</c> class.
        /// </summary>
        /// <param name="context">The repository context being used by the repository.</param>
        public SharePointRepository(ISharePointRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException("repositoryContext");

            this._context = context;
        }
        #endregion

        /// <summary>
        /// 工作单元
        /// </summary>
        public ISharePointRepositoryContext Context
        {
            get
            {
                return _context;
            }
        }

        public abstract bool Add(TEntity insertItem);

        public abstract bool Add(IList<TEntity> insertList);

        public abstract bool Update(TEntity updateItem);

        public abstract bool Update(IList<TEntity> updateList);

        public abstract bool Delete(TEntity insertItem);

        public abstract bool Delete(IList<TEntity> deleteList);

        public abstract TEntity GetObjectByPrimaryKey(Func<TEntity, bool> filter);

        public abstract IList<TEntity> Query(Func<TEntity, bool> filter);

        public abstract IList<TEntity> AllItems();

        public abstract IList<TResult> GetPageInfo<TResult>(int pageRecordCount, Func<TEntity, TResult> propertySelector,
                                                            Func<TEntity, bool> filter, ref int recordCount);

        public abstract bool TransactionAction(Action<object[]> action, params object[] objs);

        public abstract bool TransactionAction(Action action);

        public abstract IList<TEntity> GetPageInfo(System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);

        public abstract IList<TEntity> GetPageInfo(System.Linq.Expressions.Expression<Func<TEntity, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TEntity, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize);
    }
}
