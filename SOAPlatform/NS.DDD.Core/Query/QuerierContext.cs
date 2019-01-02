using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NS.DDD.Core.Repository;
using NS.Framework.Utility;

namespace NS.DDD.Core
{
    /// <summary>
    /// 查询器接口定义-负责处理特殊业务的查询处理
    /// </summary>
    /// <typeparam name="TDomainModel">领域模型类型</typeparam>
    public abstract class QuerierContext : DisposableObject, IQuerierContext
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Dispose();
        }

        public abstract void Dispose();
    }
}
