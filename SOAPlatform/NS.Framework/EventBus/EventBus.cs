﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Bus
{
    /// <summary>
    /// 事件总线，负责事件的订阅和派发
    /// </summary>
    public class EventBus
    {
        public static void Publish<T>(T eventArgs)
        {
            EventFactory.Instance.Publish<T>(eventArgs);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        public static void Subscribe<T>(EventBusHandler<T> handler)
        {
            EventFactory.Instance.Subscribe<T>(handler);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        public static void UnSubscribe<T>(EventBusHandler<T> handler)
        {
            EventFactory.Instance.UnSubscribe<T>(handler);
        }

        /// <summary>
        /// 订阅指定的事件类型
        /// </summary>
        /// <param name="handler">消息处理方法</param>
        /// <param name="filter">需要订阅的事件类型数组</param>
        public static void Subscribe<T>(EventBusHandler<T> handler, Type[] filter)
        {
            EventFactory.Instance.Subscribe<T>(handler, filter);
        }

        /// <summary>
        /// 订阅满足过滤条件的事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        /// <param name="filter">事件过滤方法</param>
        public static void Subscribe<T>(EventBusHandler<T> handler, Func<T, bool> filter)
        {
            EventFactory.Instance.Subscribe<T>(handler, filter);
        }

        public static void Subscribe<T1>(object ChangeUserHandler)
        {
            throw new NotImplementedException();
        }
    }
}
