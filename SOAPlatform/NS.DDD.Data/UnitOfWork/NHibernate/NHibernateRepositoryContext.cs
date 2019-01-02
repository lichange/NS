using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.DDD.Core.Repository;
using NS.Component.Data;
using NS.Component.Data.Impl;
using NS.Framework.Attributes;

namespace NS.DDD.Data
{
    /// <summary>
    /// NHibernate Context;
    /// </summary>
    [Export(typeof(INHibernateRepositoryContext))]
    public class NHibernateRepositoryContext : RepositoryContext, INHibernateRepositoryContext
    {
        private static readonly object lock_flag = new object();
        private readonly ThreadLocal<IPersistenceDAL> localPersistenceDAL;
        public NHibernateRepositoryContext(string key)
        {
            lock (lock_flag)
                localPersistenceDAL = new ThreadLocal<IPersistenceDAL>(()=>new PersistenceDAL(key));
        }

        public override void Commit()
        {
            if (!Committed)
            {
                var newList = NewCollection.Select(pre => pre.Value).ToList();
                var modifiedList = ModifiedCollection.Select(pre => pre.Value).ToList();
                var deleteList = DeletedCollection.Select(pre => pre.Value).ToList();

                localPersistenceDAL.Value.PersistenceToDatabase(newList, modifiedList, deleteList);

                //执行事务操作。
                Committed = true;

                BaseCommit();
            }
        }

        public override void Rollback()
        {
            //回滚操作
            Committed = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!Committed)
                    Commit();
                base.Dispose(disposing);
            }
        }

        public IPersistenceDAL Persistence
        {
            get
            {
                return this.localPersistenceDAL.Value;
            }
        }
    }

    /// <summary>
    /// NHibernate Context;
    /// </summary>
    [Export(typeof(INHibernateRepositoryContextBase))]
    public class NHibernateRepositoryContextBase : RepositoryContextBase, INHibernateRepositoryContextBase
    {
        private static readonly object lock_flag = new object();
        private readonly ThreadLocal<IPersistenceDAL> localPersistenceDAL;
        public NHibernateRepositoryContextBase(string key)
        {
            lock (lock_flag)
                localPersistenceDAL = new ThreadLocal<IPersistenceDAL>(() => new PersistenceDAL(key));
        }

        public override void Commit()
        {
            if (!Committed)
            {
                var newList = NewCollection.Select(pre => pre.Value).ToList();
                var modifiedList = ModifiedCollection.Select(pre => pre.Value).ToList();
                var deleteList = DeletedCollection.Select(pre => pre.Value).ToList();

                localPersistenceDAL.Value.PersistenceToDatabase(newList, modifiedList, deleteList);

                //执行事务操作。
                Committed = true;

                BaseCommit();
            }
        }

        public override void Rollback()
        {
            //回滚操作
            Committed = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!Committed)
                    Commit();
                base.Dispose(disposing);
            }
        }

        public IPersistenceDAL Persistence
        {
            get
            {
                return this.localPersistenceDAL.Value;
            }
        }
    }
}
