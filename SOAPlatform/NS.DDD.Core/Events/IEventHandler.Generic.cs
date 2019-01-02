using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Events
{
    /// <summary>
    /// 同步执行的事件处理操作
    /// </summary>
    /// <typeparam name="TEvent">事件对象</typeparam>
    public interface IEventHandler<in TEvent>
        where TEvent : IEvent
    {
        /// <summary>
        /// 操作处理方法
        /// </summary>
        /// <param name="evnt">事件对象</param>
        void Handle(TEvent evnt);
    }
}
