using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NS.DDD.Core.Events;
using NS.Framework.Attributes;
using NS.Framework.IOC;
using NS.Framework.Utility;

namespace NS.DDD.Core.EventBus
{
    /// <summary>
    /// 默认的事件总线实现。
    /// </summary>
    [Export(typeof(IEventBus))]
    public class DefaultEventBus : IEventBus
    {
        //private IEventHandlerRegistry _handlerRegistry;

        public DefaultEventBus()
        {
            //RequireValidate.NotNull(eventHandlerRegistry, "eventHandlerRegistry");
            //_handlerRegistry = eventHandlerRegistry;
        }

        public void Publish<TEvent>(TEvent evnt) where TEvent : class ,IEvent
        {
            Check.NotNull<TEvent>(evnt, "evnt");
            var handlers = EventHandlerContainer.GetEventHandlers<TEvent>();
            foreach (var handler in handlers)
            {
                EventHandlerInvoker.Invoke(handler, evnt);
            }
        }

        public bool RegisterHandler(Type handlerType)
        {
            Check.NotNull(handlerType, "handlerType");
            return EventHandlerContainer.RegisterHandler(handlerType);
        }

        public bool UnregisterHandler(Type handlerType)
        {
            Check.NotNull(handlerType, "handlerType");
            return EventHandlerContainer.UnregisterHandler(handlerType);
        }

        public void UnregisterHandlers(params Type[] handlerTypes)
        {
            Check.NotNull(handlerTypes, "handlerTypes");
            EventHandlerContainer.UnregisterHandlers(handlerTypes);
        }

        public void UnregisterAllHandlers()
        {
            //_handlerRegistry.UnregisterAllHandlers();
        }
    }
}
