using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NS.DDD.Core.Events;

namespace NS.DDD.Core.EventBus
{
    /// <summary>
    /// 事件总线 - 负责实现领域模型之间的消息通信的总线
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布 事件 - 通知所有监听的对象执行对应操作
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        void Publish<TEvent>(TEvent evnt) where TEvent :class ,IEvent;

        /// <summary>
        /// 向总线上注册事件Handler
        /// </summary>
        /// <param name="handlerType">事件处理器类型</param>
        /// <returns></returns>
        bool RegisterHandler(Type handlerType);

        /// <summary>
        /// 取消注册事件Handler
        /// </summary>
        /// <param name="handlerType">事件处理器类型</param>
        /// <returns></returns>
        bool UnregisterHandler(Type handlerType);

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="handlerTypes">事件处理器类型集合</param>
        void UnregisterHandlers(params Type[] handlerTypes);

        /// <summary>
        /// 清空所有的事件处理器注册信息。
        /// </summary>
        void UnregisterAllHandlers();
    }
}
