using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NS.DDD.Core;
using NS.Component.Data;
using NS.Component.Data.Impl;
using NS.Framework.Attributes;

namespace NS.DDD.Data
{
    /// <summary>
    /// NHibernate Context;
    /// </summary>
    [Export(typeof(INHibernateQuerierContext))]
    public class NHibernateQuerierContext : QuerierContext, INHibernateQuerierContext
    {
        private readonly ThreadLocal<IPersistenceDAL> localPersistenceDAL;
        public NHibernateQuerierContext()
        {
            localPersistenceDAL = new ThreadLocal<IPersistenceDAL>(() => new PersistenceDAL(""));
        }

        public IPersistenceDAL Persistence
        {
            get { return this.localPersistenceDAL.Value; }
        }

        public override void Dispose()
        {
            this.localPersistenceDAL.Dispose();
            base.Dispose(true);
        }
    }
}
