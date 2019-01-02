using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Registration;

namespace NS.Framework.IOC
{
    /// <summary>
    /// 事件处理Handle注册容器
    /// </summary>
    public class EventHandlerContainer
    {
        public static IList<object> GetEventHandlers<TEvent>()
        {
            return InternalEventHandlerFactory.Instance.GetEventHandlers<TEvent>();
        }

        public static IList<object> GetEventHandlers(Type eventType)
        {
            return InternalEventHandlerFactory.Instance.GetEventHandlers(eventType);
        }

        /// <summary>
        /// 向总线上注册事件
        /// </summary>
        ///<param name="handlerType">处理器类型</param>
        /// <returns>返回操作的结果</returns>
        public static bool RegisterHandler(Type handlerType)
        {
            return InternalEventHandlerFactory.Instance.RegisterHandler(handlerType);
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="handlerType">处理器类型</param>
        /// <returns></returns>
        public static bool UnregisterHandler(Type handlerType)
        {
            return InternalEventHandlerFactory.Instance.UnregisterHandler(handlerType);
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public static void UnregisterHandlers(params Type[] handlerTypes)
        {
            if(handlerTypes.Length==0)
                return;

            foreach (var handlerType in handlerTypes)
            {
                if (handlerType==null)
                    continue;

                InternalEventHandlerFactory.Instance.UnregisterHandlers(handlerType);
            }
        }

        public static int GetRegisterCount()
        {
            return InternalEventHandlerFactory.Instance.GetRegisterCount();
        }

        public static IEnumerable<ContainerRegistration> GetRegisterItems()
        {
            return InternalEventHandlerFactory.Instance.GetRegisterItems();
        }
    }
}
