using Microsoft.EntityFrameworkCore;
using NS.DDD.Core;
using NS.DDD.Core.Repository;
using NS.Framework.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data.UnitOfWork
{
    public class EntityFrameworkRepository<TAggregateRoot> : Repository<TAggregateRoot>
        where TAggregateRoot : class, IAggregateRoot, new()
    {
        private IEntityFrameworkRepositoryContext tempContext;
        public EntityFrameworkRepository(IEntityFrameworkRepositoryContext context)
            : base(context)
        {
            this.tempContext = context;
        }

        public override object Add(TAggregateRoot insertItem)
        {
            if (insertItem == null)
                throw new RepositoryException("对象不能为Null");
            
            if (insertItem.GetAggregateID() == null || string.IsNullOrEmpty(insertItem.GetAggregateID().ToString()))
            {
                this.GeneratorAggregateId(insertItem);
            }

            this.Context.RegisterNew(insertItem);
            return insertItem.GetAggregateID();
        }

        #region 自动创建主键

        private void GeneratorAggregateId(TAggregateRoot insertItem)
        {
            IList<object> tempAggregateRoots = new List<object>();

            this.GetAllReferenceType(insertItem, ref tempAggregateRoots);

            var generator = AggregateIdGeneratorFactory.GetGenerator(typeof(TAggregateRoot));
            //insertItem. = generator.Generate<TAggregateRoot>();

            var parentPropertyName = insertItem.GetType().Name + "Id";

            foreach (var tempAggregateRoot in tempAggregateRoots)
            {
                var tempAggregateRootGenerator = AggregateIdGeneratorFactory.GetGenerator(tempAggregateRoot.GetType());

                var tempPropertys = tempAggregateRoot.GetType().GetProperties();

                if (tempPropertys == null || tempPropertys.Length == 0)
                    continue;

                var tempAggregateIDProperty = tempPropertys.Where(pre => pre.Name == "AggregateID").FirstOrDefault();

                if (tempAggregateIDProperty == null)
                    continue;

                var value = tempAggregateIDProperty.GetValue(tempAggregateRoot, null);

                if (value == null || string.IsNullOrEmpty(value.ToString()))
                {
                    tempAggregateIDProperty.SetValue(tempAggregateRoot, tempAggregateRootGenerator.Generate<TAggregateRoot>(),null);
                }

                var tempParentIDProperty = tempPropertys.Where(pre => pre.Name.ToLower() == parentPropertyName.ToLower()).FirstOrDefault();

                if (tempParentIDProperty == null)
                    continue;

                var value1 = tempParentIDProperty.GetValue(tempAggregateRoot, null);

                if (value1 != null && !string.IsNullOrEmpty(value1.ToString()))
                    continue;

                tempParentIDProperty.SetValue(tempAggregateRoot, insertItem.GetAggregateID(), null);
            }
        }

        private void GetAllReferenceType(object obj, ref  IList<object> tempObjects)
        {
            if (obj == null)
                return;

            var objPropertys = obj.GetType().GetProperties();

            if (objPropertys == null || objPropertys.Length == 0)
                return;

            foreach (var propertyInfo in objPropertys)
            {
                if (propertyInfo.PropertyType.GetGenericArguments().Count() > 0)
                {
                    var argumentTypes = propertyInfo.PropertyType.GetGenericArguments();

                    foreach (var argumentType in argumentTypes)
                    {
                        if (argumentType.GetInterfaces().Where(Predicate => Predicate.Name == typeof(IAggregateRoot).Name || Predicate.Name == typeof(IEntity).Name).Count() == 0)
                            continue;
                    }
                }
                else
                {
                    if (propertyInfo.PropertyType.GetInterfaces().Where(pre => pre.Name == typeof(IAggregateRoot).Name).Count() == 0)
                        continue;
                }

                if (propertyInfo.PropertyType.GetGenericArguments().Count() > 0)
                {
                    var tempRoots = propertyInfo.GetValue(obj, null) as IEnumerable<object>;

                    foreach (var tempRoot in tempRoots)
                    {
                        if (tempRoot == null)
                            continue;

                        tempObjects.Add(tempRoot);
                    }

                    continue;
                }

                var propertyValue = propertyInfo.GetValue(obj, null);

                if (propertyValue == null)
                    continue;

                this.GetAllReferenceType(propertyInfo.GetValue(obj, null), ref tempObjects);

                tempObjects.Add(propertyValue);
            }
        }

        #endregion

        public override bool Add(IList<TAggregateRoot> insertList)
        {
            foreach (var aggregateRoot in insertList)
            {
                this.Add(aggregateRoot);
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
                if (aggregateRoot == null)
                    continue;

                this.Context.RegisterDeleted(aggregateRoot);
            }
            return true;
        }

        public override TAggregateRoot GetObjectByPrimaryKey(Func<TAggregateRoot, bool> filter)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().Where(filter).FirstOrDefault();

        }

        public override IList<TAggregateRoot> Query(Func<TAggregateRoot, bool> filter)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().Where(filter).ToList();
        }

        public override IList<TAggregateRoot> AllItems()
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().ToList();
        }

        public override IList<TResult> GetPageInfo<TResult>(int pageRecordCount, Func<TAggregateRoot, TResult> propertySelector, Func<TAggregateRoot, bool> filter, ref int recordCount)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().Where(filter).Select(propertySelector).Skip((pageRecordCount - 1) * 15).Take(15).ToList();
        }

        /// <summary>
        /// 高性能插入数据-10w数据在7秒左右，解决EF多数据插入性能问题-该方式转用Ado.net
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public override bool AddBitchHighPerformance<TEntity>(IList<TEntity> entities)
        {
            this.tempContext.Context.BulkInsert<TEntity>(entities, 1000);

            return true;
        }

        public override bool TransactionAction(Action<object[]> action, params object[] objs)
        {
            //return this.tempContext.Context.TransactionAction(action, objs);
            return true;
        }

        public override bool TransactionAction(Action action)
        {
            //return this.tempContext.Context.TransactionAction(action);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.tempContext.Dispose();
                base.Dispose();
            }
        }

        public override IList<TAggregateRoot> GetPageInfo(System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().OrderBy(sortPredicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public override IList<TAggregateRoot> GetPageInfo(System.Linq.Expressions.Expression<Func<TAggregateRoot, bool>> queryPredicate, System.Linq.Expressions.Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().AsNoTracking().Where(queryPredicate).OrderBy(sortPredicate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public override TAggregateRoot GetObjectByPrimaryKey(object key)
        {
            return this.tempContext.Context.Set<TAggregateRoot>().Find(key);
        }

        public IEntityFrameworkRepositoryContext RepositoryContext
        {
            get
            {
                return this.tempContext;
            }
        }
    }
}
