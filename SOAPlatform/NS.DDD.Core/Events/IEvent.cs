using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Events
{
    /// <summary>
    /// 事件接口-要求能被EventBus处理的对象，都必须从该接口继承
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 事件调用执行的时间
        /// </summary>
        DateTime UtcTimestamp { get; }
    }
}
