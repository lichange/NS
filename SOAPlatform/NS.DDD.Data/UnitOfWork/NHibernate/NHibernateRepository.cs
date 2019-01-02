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
    public class NHibernateRepository<TAggregateRoot> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot,new()
    {
        private INHibernateRepositoryContext tempContext;
        public NHibernateRepository(INHibernateRepositoryContext context)
            : base(context)
        {
            this.tempContext = context;
        }

         public override object Add(TAggregateRoot insertItem)
        {
            if (insertItem == null)
                throw new RepositoryException("对象不能为Null");
             this.Context.RegisterNew(insertItem);
             return true;
         }

         public override bool Add(IList<TAggregateRoot> insertList)
         {
             foreach (var aggregateRoot in insertList)
             {
                 if (aggregateRoot == null)
                     continue;
                 this.Context.RegisterNew(aggregateRoot);
             }
             return true;
         }

         public override bool Update(TAggregateRoot updateItem)
         {
             if (updateItem == null)
                 throw new RepositoryException("对象不能为Null");
             this.Context.RegisterModified(updateItem);
             return true;
         }

         public override bool Update(IList<TAggregateRoot> updateList)
         {
             foreach (var aggregateRoot in updateList)
             {
                 if (aggregateRoot == null)
                     continue;
                 this.Context.RegisterModified(aggregateRoot);
             }
             return true;
         }

         public override bool Delete(TAggregateRoot deleteItem)
         {
             if (deleteItem == null)
                 throw new RepositoryException("对象不能为Null");
             this.Context.RegisterDeleted(deleteItem);
             return true;
         }

         public override bool Delete(IList<TAggregateRoot> deleteList)
         {
             foreach (var aggregateRoot in deleteList)
             {
                 if(aggregateRoot==null)
                     continue;

                 this.Context.RegisterDeleted(aggregateRoot);
             }
             return true;
         }

         public override TAggregateRoot GetObjectByPrimaryKey(Func<TAggregateRoot, bool> filter)
         {
             return this.tempContext.Persistence.GetObjectByPrimaryKey<TAggregateRoot>(filter);
         }

         public override IList<TAggregateRoot> Query(Func<TAggregateRoot, bool> filter)
         {
             return this.tempContext.Persistence.Query(filter);
         }

         public override IList<TAggregateRoot> AllItems()
         {
             return this.tempContext.Persistence.AllItems<TAggregateRoot>();
         }

         public override IList<TResult> GetPageInfo<TResult>(int pageRecordCount, Func<TAggregateRoot, TResult> propertySelector, Func<TAggregateRoot, bool> filter, ref int recordCount)
         {
             return this.tempContext.Persistence.GetPageInfo<TAggregateRoot, TResult>(pageRecordCount, propertySelector, filter, ref recordCount);
         }

         public override bool TransactionAction(Action<object[]> action, params object[] objs)
         {
             return this.tempContext.Persistence.TransactionAction(action, objs);
         }

         public override bool TransactionAction(Action action)
         {
             return this.tempContext.Persistence.TransactionAction(action);
         }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 this.tempContext.Dispose();
                 //base.Dispose();
             }
         }

         public override IList<TAggregateRoot> GetPageInfo(System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
         {
             return this.tempContext.Persistence.AllItems<TAggregateRoot>().OrderBy(sortPredicate.Compile()).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
         }

         public override IList<TAggregateRoot> GetPageInfo(System.Linq.Expressions.Expression<Func<TAggregateRoot, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
         {
             return this.tempContext.Persistence.Query<TAggregateRoot>(queryPredicate.Compile()).OrderBy(sortPredicate.Compile()).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
         }

         public override TAggregateRoot GetObjectByPrimaryKey(object key)
         {
             return this.tempContext.Persistence.Query<TAggregateRoot>(pre => pre.GetAggregateID() == key).FirstOrDefault();
         }

         public override bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities)
         {
             throw new NotImplementedException();
         }
    }

    public class NHibernateRepository : Repository
    {
        private INHibernateRepositoryContext tempContext;
        public NHibernateRepository(INHibernateRepositoryContext context)
            : base(context)
        {
            this.tempContext = context;
        }

        public override object Add<TAggregateRoot>(TAggregateRoot insertItem) 
        {
            if (insertItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterNew(insertItem);
            return true;
        }

        public override bool Add<TAggregateRoot>(IList<TAggregateRoot> insertList) 
        {
            foreach (var aggregateRoot in insertList)
            {
                if (aggregateRoot == null)
                    continue;
                this.Context.RegisterNew(aggregateRoot);
            }
            return true;
        }

        public override bool Update<TAggregateRoot>(TAggregateRoot updateItem) 
        {
            if (updateItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterModified(updateItem);
            return true;
        }

        public override bool Update<TAggregateRoot>(IList<TAggregateRoot> updateList)
        {
            foreach (var aggregateRoot in updateList)
            {
                if (aggregateRoot == null)
                    continue;
                this.Context.RegisterModified(aggregateRoot);
            }
            return true;
        }

        public override bool Delete<TAggregateRoot>(TAggregateRoot deleteItem)
        {
            if (deleteItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterDeleted(deleteItem);
            return true;
        }

        public override bool Delete<TAggregateRoot>(IList<TAggregateRoot> deleteList)
        {
            foreach (var aggregateRoot in deleteList)
            {
                if (aggregateRoot == null)
                    continue;

                this.Context.RegisterDeleted(aggregateRoot);
            }
            return true;
        }

        public override TAggregateRoot GetObjectByPrimaryKey<TAggregateRoot>(Func<TAggregateRoot, bool> filter)
        {
            return this.tempContext.Persistence.GetObjectByPrimaryKey<TAggregateRoot>(filter);
        }

        public override IList<TAggregateRoot> Query<TAggregateRoot>(Func<TAggregateRoot, bool> filter)
        {
            return this.tempContext.Persistence.Query(filter);
        }

        public override IList<TAggregateRoot> AllItems<TAggregateRoot>()
        {
            return this.tempContext.Persistence.AllItems<TAggregateRoot>();
        }

        public override IList<TResult> GetPageInfo<TResult,TAggregateRoot>(int pageRecordCount, Func<TAggregateRoot, TResult> propertySelector, Func<TAggregateRoot, bool> filter, ref int recordCount)
        {
            return this.tempContext.Persistence.GetPageInfo<TAggregateRoot, TResult>(pageRecordCount, propertySelector, filter, ref recordCount);
        }

        public override bool TransactionAction(Action<object[]> action, params object[] objs)
        {
            return this.tempContext.Persistence.TransactionAction(action, objs);
        }

        public override bool TransactionAction(Action action)
        {
            return this.tempContext.Persistence.TransactionAction(action);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tempContext.Dispose();
                //base.Dispose();
            }
        }

        public override IList<TAggregateRoot> GetPageInfo<TAggregateRoot>(System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return this.tempContext.Persistence.AllItems<TAggregateRoot>().OrderBy(sortPredicate.Compile()).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public override IList<TAggregateRoot> GetPageInfo<TAggregateRoot>(System.Linq.Expressions.Expression<Func<TAggregateRoot, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return this.tempContext.Persistence.Query<TAggregateRoot>(queryPredicate.Compile()).OrderBy(sortPredicate.Compile()).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public override TAggregateRoot GetObjectByPrimaryKey<TAggregateRoot>(object key) 
        {
            return this.tempContext.Persistence.Query<TAggregateRoot>(pre => pre.GetAggregateID() == key).FirstOrDefault();
        }

        public override bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities)
        {
            throw new NotImplementedException();
        }
    }

    public class NHibernateRepositoryBase : RepositoryBase
    {
        private INHibernateRepositoryContextBase tempContext;
        public NHibernateRepositoryBase(INHibernateRepositoryContextBase context)
            : base(context)
        {
            this.tempContext = context;
        }

        public override object Add(object insertItem)
        {
            if (insertItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterNew(insertItem);
            return true;
        }

        public override bool Add(IList<object> insertList)
        {
            foreach (var aggregateRoot in insertList)
            {
                if (aggregateRoot == null)
                    continue;
                this.Context.RegisterNew(aggregateRoot);
            }
            return true;
        }

        public override bool Update(object updateItem)
        {
            if (updateItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterModified(updateItem);
            return true;
        }

        public override bool Update(IList<object> updateList)
        {
            foreach (var aggregateRoot in updateList)
            {
                if (aggregateRoot == null)
                    continue;
                this.Context.RegisterModified(aggregateRoot);
            }
            return true;
        }

        public override bool Delete(object deleteItem)
        {
            if (deleteItem == null)
                throw new RepositoryException("对象不能为Null");
            this.Context.RegisterDeleted(deleteItem);
            return true;
        }

        public override bool Delete(IList<object> deleteList)
        {
            foreach (var aggregateRoot in deleteList)
            {
                if (aggregateRoot == null)
                    continue;

                this.Context.RegisterDeleted(aggregateRoot);
            }
            return true;
        }
        
        public override bool TransactionAction(Action<object[]> action, params object[] objs)
        {
            return this.tempContext.Persistence.TransactionAction(action, objs);
        }

        public override bool TransactionAction(Action action)
        {
            return this.tempContext.Persistence.TransactionAction(action);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tempContext.Dispose();
                //base.Dispose();
            }
        }

        public override object GetObjectByPrimaryKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
