using System.Data;
using System.Reflection;
using NS.DDD.Core;
using NS.DDD.Core.Repository;
using NS.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.Framework.IOC;
using EmitMapper;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace NS.DDD.Data
{
    [Export(typeof(IEntityFrameworkRepositoryContext))]
    public class EntityFrameworkRepositoryContext<TDataContext> : RepositoryContext, IEntityFrameworkRepositoryContext where TDataContext : DbContext, new()
    {
        private readonly ThreadLocal<TDataContext> localCtx = new ThreadLocal<TDataContext>(() => new TDataContext());
        private readonly string AggregateID = "AggregateID";
        private IList<TempRelation> aggregateRoots = new List<TempRelation>();

        /// <summary>
        /// 获取当前仓储上下文
        /// </summary>
        public DbContext Context
        {
            get
            {
                return localCtx.Value;
            }
        }

        public override void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj)
        {
            if (obj.GetAggregateID() == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                //this.PrepareToDo(obj);

                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(obj.GetAggregateID());
                if (entityToUpdate != null)
                {
                    // //localCtx.Value.Set<TAggregateRoot>().Attach(obj);
                    // //var objID = obj.AggregateID;
                    // //var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Where(pre => pre.AggregateID == objID).FirstOrDefault();
                    ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>().Map(obj, entityToUpdate);
                    localCtx.Value.Set<TAggregateRoot>().Remove(entityToUpdate);
                }
                else
                {
                    localCtx.Value.Set<TAggregateRoot>().Attach(obj);
                    localCtx.Value.Set<TAggregateRoot>().Remove(obj);
                }
            }

            //localCtx.Value.Set<TAggregateRoot>().Remove(obj);
            Committed = false;
        }

        public override void RegisterModified<TAggregateRoot>(TAggregateRoot obj)
        {
            if (obj.GetAggregateID() == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                //this.PrepareToDo(obj);

                if (aggregateRoots.Where(pre => pre.AggregateId == obj.GetAggregateID()).Count() > 0)
                    return;

                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(obj.GetAggregateID());
                if (entityToUpdate != null)
                {
                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>()
                              .Map(obj, entityToUpdate);
                    localCtx.Value.Entry<TAggregateRoot>(entityToUpdate).State = EntityState.Modified;
                }
                else
                {
                    if (obj.GetAggregateID() == null)
                    {

                    }

                    localCtx.Value.Set<TAggregateRoot>().Add(obj);
                    //localCtx.Value.Set<TAggregateRoot>().Attach(obj);
                    //localCtx.Value.Entry<TAggregateRoot>(obj).State = System.Data.EntityState.Modified;
                }

                //var key = new TempRelation()
                //{
                //    AggregateId = obj.AggregateID.ToString(),
                //    PropertyClass = entityToUpdate != null ? entityToUpdate : obj,
                //    Property = null,
                //    PropertyValue = entityToUpdate != null ? entityToUpdate : obj,
                //};

                //this.aggregateRoots.Add(key);
            }

            Committed = false;
        }

        public override void RegisterNew<TAggregateRoot>(TAggregateRoot obj)
        {
            if (obj.GetAggregateID() == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                //this.PrepareToDo(obj);

                if (aggregateRoots.Where(pre => pre.AggregateId == obj.GetAggregateID()).Count() > 0)
                    return;

                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(obj.GetAggregateID());
                if (entityToUpdate != null)
                {
                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>()
                              .Map(obj, entityToUpdate);
                    localCtx.Value.Entry<TAggregateRoot>(entityToUpdate).State = EntityState.Modified;
                }
                else
                {
                    localCtx.Value.Set<TAggregateRoot>().Add(obj);
                }

                //var key = new TempRelation()
                //{
                //    AggregateId = obj.AggregateID.ToString(),
                //    PropertyClass = entityToUpdate != null ? entityToUpdate : obj,
                //    Property = null,
                //    PropertyValue = entityToUpdate != null ? entityToUpdate : obj,
                //};

                //this.aggregateRoots.Add(key);

                //var objID = obj.AggregateID.ToString();
                //var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Where(pre => pre.AggregateID.ToString() == objID).FirstOrDefault();
                //EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>().Map(obj, entityToUpdate);
                //localCtx.Value.Entry<TAggregateRoot>(obj).State = System.Data.EntityState.Added;
            }

            //localCtx.Value.Set<TAggregateRoot>().Add(obj);
            Committed = false;
        }

        //private void PrepareToDo(object obj)
        //{
        //    this.GetAllReferenceType(obj, ref aggregateRoots);

        //    if (aggregateRoots.Count > 0)
        //    {
        //        foreach (var tempRelation in aggregateRoots)
        //        {
        //            var aggregateRoot = tempRelation.PropertyValue;

        //            if (aggregateRoot == null)
        //                continue;

        //            try
        //            {
        //                this.localCtx.Value.Entry(aggregateRoot);
        //            }
        //            catch (InvalidOperationException ex)
        //            {
        //                continue;
        //            }

        //            if (aggregateRoot is IAggregateRoot)
        //            {
        //                IAggregateRoot tempAggregateRoot = aggregateRoot as IAggregateRoot;
        //                var tempAggregateRootToUpdate = localCtx.Value.Set(tempAggregateRoot.GetType()).Find(tempAggregateRoot.GetAggregateID());
        //                if (tempAggregateRootToUpdate != null)
        //                {
        //                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<IAggregateRoot, IAggregateRoot>().Map(tempAggregateRoot, (IAggregateRoot)tempAggregateRootToUpdate);
        //                    localCtx.Value.Entry(tempAggregateRootToUpdate).State = EntityState.Modified;

        //                    if (tempRelation.Property != null)
        //                        tempRelation.Property.SetValue(tempRelation.PropertyClass, tempAggregateRootToUpdate, null);
        //                }
        //                else
        //                {
        //                    localCtx.Value.Set(tempAggregateRoot.GetType()).Add(aggregateRoot);
        //                }
        //            }
        //            else if (aggregateRoot is IEntity)
        //            {
        //                IEntity tempAggregateRoot = aggregateRoot as IEntity;

        //                var objProperty = obj.GetType().GetProperties().Where(pre => pre.Name == AggregateID).LastOrDefault();

        //                if (objProperty == null)
        //                    return;

        //                var value = objProperty.GetValue(obj, null);

        //                if (value == null)
        //                    return;

        //                var tempAggregateRootToUpdate = localCtx.Value.Set(tempAggregateRoot.GetType()).Find(value);
        //                if (tempAggregateRootToUpdate != null)
        //                {
        //                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<IEntity, IEntity>().Map(tempAggregateRoot, (IEntity)tempAggregateRootToUpdate);
        //                    localCtx.Value.Entry(tempAggregateRootToUpdate).State = EntityState.Modified;

        //                    if (tempRelation.Property != null)
        //                        tempRelation.Property.SetValue(tempRelation.PropertyClass, tempAggregateRootToUpdate, null);
        //                }
        //                else
        //                {
        //                    localCtx.Value.Set(tempAggregateRoot.GetType()).Add(aggregateRoot);
        //                }
        //            }

        //        }
        //    }
        //}

        private void GetAllReferenceType(object obj, ref IList<TempRelation> tempPropertys)
        {
            if (obj == null)
                return;

            var objPropertys = obj.GetType().GetProperties();

            if (objPropertys == null || objPropertys.Length == 0)
                return;

            foreach (var propertyInfo in objPropertys)
            {
                if (!(propertyInfo.PropertyType.GetInterfaces().Where(pre => pre.Name == typeof(IAggregateRoot).Name).Count() > 0))
                    continue;

                var subObj = propertyInfo.GetValue(obj, null);

                this.GetAllReferenceType(subObj, ref tempPropertys);

                if (subObj == null)
                    continue;

                var aggregateRootProperty = subObj.GetType().GetProperties().Where(pre => pre.Name == AggregateID).LastOrDefault();

                var aggregateId = aggregateRootProperty.GetValue(subObj, null);

                var propertyValue = propertyInfo.GetValue(obj, null);

                if (propertyValue == null)
                    continue;

                var key = new TempRelation()
                {
                    AggregateId = aggregateId.ToString(),
                    PropertyClass = obj,
                    Property = propertyInfo,
                    PropertyValue = propertyValue
                };

                var flag = tempPropertys.Where(pre => pre.AggregateId == key.AggregateId).Count() > 0;

                if (!flag)
                    tempPropertys.Add(key);
            }
        }

        public override void Commit()
        {
            if (!Committed)
            {
                try
                {
                   // var validationErrors = localCtx.Value.GetValidationErrors();
                    var count = localCtx.Value.SaveChanges();
                    Committed = true;
                    aggregateRoots.Clear();

                    //if (true)
                    //{
                    //    //this.GetEntityOperateLogs(this.Context);
                    //    //写操作日志
                    //}
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                    {
                        SqlException sqlEx = e.InnerException.InnerException as SqlException;
                        string msg = DbExceptionHelper.GetSqlExceptionMessage(sqlEx.Number);
                        throw new Framework.Exceptions.ValidationException("提交数据更新时发生异常：" + msg, sqlEx);
                    }
                    throw;
                }
                catch (Exception ex)
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

                    var logger = ObjectContainer.CreateInstance<NS.Framework.Log.ILogProvider>();
                    logger.Error("持久化数据过程中出现错误:" + ex.Message);

                    throw new Framework.Exceptions.ValidationException(ex.Message);
                }

            }
        }

        #region 获取EF 数据库操作日志

        /// <summary>
        /// 获取数据上下文的变更日志信息
        /// </summary>
        public IEnumerable<OperatingLog> GetEntityOperateLogs(DbContext dbContext)
        {
            throw new Exception(".net standard 暂不支持原有的数据库日志操作");
            //string[] nonLoggingTypeNames = { };

            //ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            //ObjectStateManager manager = objectContext.ObjectStateManager;

            //IEnumerable<ObjectStateEntry> entries = manager.GetObjectStateEntries(EntityState.Added)
            //    .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            //IEnumerable<OperatingLog> logs = entries.Select(GetAddedLog);

            //entries = manager.GetObjectStateEntries(EntityState.Modified)
            //    .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            //logs = logs.Union(entries.Select(GetModifiedLog));

            //entries = manager.GetObjectStateEntries(EntityState.Deleted)
            //    .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            //logs = logs.Union(entries.Select(GetDeletedLog));
            //return logs;
        }

        /// <summary>
        /// 获取添加数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        //private static OperatingLog GetAddedLog(ObjectStateEntry entry)
        //{
        //    OperatingLog log = new OperatingLog
        //    {
        //        EntityName = entry.EntitySet.ElementType.Name,
        //        OperateType = OperatingType.Insert
        //    };
        //    for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
        //    {
        //        string name = entry.CurrentValues.GetName(i);
        //        if (name == "Timestamp")
        //        {
        //            continue;
        //        }
        //        object value = entry.CurrentValues.GetValue(i);
        //        OperatingLogItem logItem = new OperatingLogItem()
        //        {
        //            Field = name,
        //            NewValue = value == null ? null : value.ToString()
        //        };
        //        log.LogItems.Add(logItem);
        //    }
        //    return log;
        //}

        /// <summary>
        /// 获取修改数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        //private static OperatingLog GetModifiedLog(ObjectStateEntry entry)
        //{
        //    OperatingLog log = new OperatingLog()
        //    {
        //        EntityName = entry.EntitySet.ElementType.Name,
        //        OperateType = OperatingType.Update
        //    };
        //    for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
        //    {
        //        string name = entry.CurrentValues.GetName(i);
        //        if (name == "Timestamp")
        //        {
        //            continue;
        //        }
        //        object currentValue = entry.CurrentValues.GetValue(i);
        //        object originalValue = entry.OriginalValues[name];
        //        if (currentValue.Equals(originalValue))
        //        {
        //            continue;
        //        }
        //        OperatingLogItem logItem = new OperatingLogItem()
        //        {
        //            Field = name,
        //            NewValue = currentValue == null ? null : currentValue.ToString(),
        //            OriginalValue = originalValue == null ? null : originalValue.ToString()
        //        };
        //        log.LogItems.Add(logItem);
        //    }
        //    return log;
        //}

        /// <summary>
        /// 获取删除数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        //private static OperatingLog GetDeletedLog(ObjectStateEntry entry)
        //{
        //    OperatingLog log = new OperatingLog()
        //    {
        //        EntityName = entry.EntitySet.ElementType.Name,
        //        OperateType = OperatingType.Delete
        //    };
        //    for (int i = 0; i < entry.OriginalValues.FieldCount; i++)
        //    {
        //        string name = entry.OriginalValues.GetName(i);
        //        if (name == "Timestamp")
        //        {
        //            continue;
        //        }
        //        object originalValue = entry.OriginalValues[i];
        //        OperatingLogItem logItem = new OperatingLogItem()
        //        {
        //            Field = name,
        //            OriginalValue = originalValue == null ? null : originalValue.ToString()
        //        };
        //        log.LogItems.Add(logItem);
        //    }
        //    return log;
        //}

        #endregion

        public override void Rollback()
        {
            Committed = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!Committed)
                    Commit();
                localCtx.Value.Dispose();
                localCtx.Dispose();
                base.Dispose(disposing);
            }
        }

        #region 对实体的支持

        public void RegisterNewEntity<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class
        {
            var objProperty = obj.GetType().GetProperties().Where(pre => pre.Name == AggregateID).LastOrDefault();

            if (objProperty == null)
                return;

            var value = objProperty.GetValue(obj, null);

            if (value == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                //this.PrepareToDo(obj);

                if (aggregateRoots.Where(pre => pre.AggregateId == value).Count() > 0)
                    return;

                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(value);
                if (entityToUpdate != null)
                {
                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>()
                              .Map(obj, entityToUpdate);
                    localCtx.Value.Entry<TAggregateRoot>(entityToUpdate).State = EntityState.Modified;
                }
                else
                {
                    localCtx.Value.Set<TAggregateRoot>().Add(obj);
                }
                //var objID = obj.AggregateID.ToString();
                //var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Where(pre => pre.AggregateID.ToString() == objID).FirstOrDefault();
                //EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>().Map(obj, entityToUpdate);
                //localCtx.Value.Entry<TAggregateRoot>(obj).State = System.Data.EntityState.Added;

                //var key = new TempRelation()
                //{
                //    AggregateId = value.ToString(),
                //    PropertyClass = entityToUpdate != null ? entityToUpdate : obj,
                //    Property = null,
                //    PropertyValue = entityToUpdate != null ? entityToUpdate : obj,
                //};

                //this.aggregateRoots.Add(key);

            }

            //localCtx.Value.Set<TAggregateRoot>().Add(obj);
            Committed = false;
        }

        public void RegisterModifiedEntity<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class
        {
            var objProperty = obj.GetType().GetProperties().Where(pre => pre.Name == AggregateID).LastOrDefault();

            if (objProperty == null)
                return;

            var value = objProperty.GetValue(obj, null);

            if (value == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                //this.PrepareToDo(obj);

                if (aggregateRoots.Where(pre => pre.AggregateId == value).Count() > 0)
                    return;

                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(value);
                if (entityToUpdate != null)
                {
                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>()
                              .Map(obj, entityToUpdate);
                    localCtx.Value.Entry<TAggregateRoot>(entityToUpdate).State = EntityState.Modified;
                }
                else
                {
                    localCtx.Value.Set<TAggregateRoot>().Add(obj);
                    //localCtx.Value.Entry<TAggregateRoot>(obj).State = System.Data.EntityState.Modified;
                }

                //var key = new TempRelation()
                //{
                //    AggregateId = value.ToString(),
                //    PropertyClass = entityToUpdate != null ? entityToUpdate : obj,
                //    Property = null,
                //    PropertyValue = entityToUpdate != null ? entityToUpdate : obj,
                //};

                //this.aggregateRoots.Add(key);
            }

            Committed = false;
        }

        public void RegisterDeletedEntity<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class
        {
            var objProperty = obj.GetType().GetProperties().Where(pre => pre.Name == AggregateID).LastOrDefault();

            if (objProperty == null)
                return;

            var value = objProperty.GetValue(obj, null);

            if (value == null)
                return;

            var entry = localCtx.Value.Entry(obj);

            if (entry == null)
                return;

            if (entry.State == EntityState.Detached)
            {
                var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Find(value);
                if (entityToUpdate != null)
                    // //localCtx.Value.Set<TAggregateRoot>().Attach(obj);
                    // //var objID = obj.AggregateID;
                    // //var entityToUpdate = localCtx.Value.Set<TAggregateRoot>().Where(pre => pre.AggregateID == objID).FirstOrDefault();
                    EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TAggregateRoot, TAggregateRoot>().Map(obj, entityToUpdate);
                localCtx.Value.Set<TAggregateRoot>().Remove(entityToUpdate);
            }
            else
            {
                localCtx.Value.Set<TAggregateRoot>().Attach(obj);
                localCtx.Value.Set<TAggregateRoot>().Remove(obj);
            }

            Committed = false;
        }

        #endregion

    }

    internal class TempRelation
    {
        /// <summary>
        /// 反射取出来的属性
        /// </summary>
        public System.Reflection.PropertyInfo Property
        {
            get;
            set;
        }

        /// <summary>
        /// 当前属性所属对象
        /// </summary>
        public object PropertyClass
        {
            get;
            set;
        }

        /// <summary>
        /// 聚合根主键Id
        /// </summary>
        public string AggregateId
        {
            get;
            set;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public object PropertyValue
        {
            get;
            set;
        }
    }
}
