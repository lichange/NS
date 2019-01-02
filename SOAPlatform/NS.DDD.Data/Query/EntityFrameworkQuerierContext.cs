using Microsoft.EntityFrameworkCore;
using NS.DDD.Core;
using NS.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NS.DDD.Data.Querier
{
    [Export(typeof(IEntityFrameworkQuerierContext))]
    public class EntityFrameworkQuerierContext<TDataContext> : QuerierContext, IEntityFrameworkQuerierContext where TDataContext : DbContext, new()
    {
        private readonly ThreadLocal<TDataContext> localCtx = new ThreadLocal<TDataContext>(() => new TDataContext());

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

        public override void Dispose()
        {
            localCtx.Value.Dispose();
            localCtx.Dispose();
            base.Dispose(true);
        }
    }
}
