using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NS.DDD.Core
{
    /// <summary>
    /// 事件总线-负责订阅事件和派发事件
    /// </summary>
    internal class DomainEventFactory
    {
        private static readonly object lock_flag = new object();
        private static DomainEventFactory _factory = null;

        public static DomainEventFactory Instance
        {
            get
            {
                if (_factory == null)
                {
                    lock (lock_flag)
                    {
                        if (_factory == null)
                            _factory = new DomainEventFactory();
                    }
                }

                return _factory;
            }
        }

        #region 私有成员

        /// <summary>
        /// 事件过滤器
        /// </summary>
        private readonly Hashtable filterInfos = Hashtable.Synchronized(new Hashtable());

        #endregion

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="eventArgs">需要派发的事件</param>
        public virtual void Publish<T>(T eventArgs)
        {
            if (eventArgs == null)
                return;

            List<EventBusHandler<T>> callbacks = new List<EventBusHandler<T>>();

            lock (filterInfos.SyncRoot)
            {
                foreach (var key in filterInfos.Keys)
                {
                    if (filterInfos[key] == null)
                    {
                        var tempHandler = key as EventBusHandler<T>;

                        if (tempHandler == null)
                            continue;

                        callbacks.Add(tempHandler);
                    }
                    else if ((filterInfos[key] is Type[]) && (filterInfos[key] as Type[]).Contains<Type>(eventArgs.GetType()))
                    {
                        callbacks.Add(key as EventBusHandler<T>);
                    }
                    else if (filterInfos[key] is Func<T, bool>)
                    {
                        Func<T, bool> filter = (Func<T, bool>)filterInfos[key];
                        if (filter(eventArgs))
                            callbacks.Add(key as EventBusHandler<T>);
                    }
                }
            }
            IList<EventBusHandler<T>> tempCallBackList = new List<EventBusHandler<T>>();

            foreach (EventBusHandler<T> handler in callbacks)
            {
                if (tempCallBackList.Where(item => 
                    item.Target != null && handler.Target!=null 
                    && item.Target.GetType().FullName == handler.Target.GetType().FullName 
                    && item.Method.Name == handler.Method.Name
                    && item.Method.GetParameters().Length == handler.Method.GetParameters().Length).Count()>0)
                    continue;

                tempCallBackList.Add(handler);
            }

            foreach (EventBusHandler<T> callback in tempCallBackList)
            {
                callback(eventArgs);
            }
        }

        #region 订阅消息与取消订阅消息

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        public void Subscribe<T>(EventBusHandler<T> handler)
        {
            lock (filterInfos.SyncRoot)
                if (this.ValidateContains<T>(handler))
                    filterInfos.Add(handler, null);
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        public void UnSubscribe<T>(EventBusHandler<T> handler)
        {
            lock (filterInfos.SyncRoot)
                if (filterInfos.Contains(handler))
                    filterInfos.Remove(handler);
        }

        /// <summary>
        /// 订阅指定的事件类型
        /// </summary>
        /// <param name="handler">消息处理方法</param>
        /// <param name="filter">需要订阅的事件类型数组</param>
        public void Subscribe<T>(EventBusHandler<T> handler, Type[] filter)
        {
            lock (filterInfos.SyncRoot)
                if (this.ValidateContains<T>(handler))
                    filterInfos.Add(handler, filter);
        }

        private bool ValidateContains<T>(EventBusHandler<T> handler)
        {
            //if (!filterInfos.Contains(handler))
            //    return true;

            var flag =true;

            foreach (Delegate item in filterInfos.Keys)
            {
                if (item.Target ==handler.Target && item.Method.Name == handler.Method.Name
                    && item.Method.GetParameters().Length == handler.Method.GetParameters().Length)
                {
                    flag=false;
                    break;
                }
            }

            return flag;
        }

        /// <summary>
        /// 订阅满足过滤条件的事件
        /// </summary>
        /// <param name="handler">事件处理方法</param>
        /// <param name="filter">事件过滤方法</param>
        public void Subscribe<T>(EventBusHandler<T> handler, Func<T, bool> filter)
        {
            lock (filterInfos.SyncRoot)
                if (!filterInfos.Contains(handler))
                    filterInfos.Add(handler, filter);
        }

        #endregion
    }

    /// <summary>
    /// 事件处理委托
    /// </summary>
    /// <param name="message">需处理的事件参数</param>
    public delegate void EventBusHandler<T>(T tEvent);
}
