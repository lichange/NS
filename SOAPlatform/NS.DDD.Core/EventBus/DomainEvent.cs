using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.DDD.Core.Events;
using NS.Framework.Utility;

namespace NS.DDD.Core.EventBus
{
    /// <summary>
    /// 领域事件管理器
    /// </summary>
    public static class DomainEvent
    {
        public static void Apply<TEvent>(TEvent evnt)
            where TEvent :class, IEvent
        {
            Check.NotNull<TEvent>(evnt, "evnt");

            var bus = DomainEnvironment.Instance.ImmediateEventBus;

            if (bus == null)
                throw new InvalidOperationException("Immediate event bus is not registered to the TaroEnvironment.");

            bus.Publish<TEvent>(evnt);
        }
    }
}
