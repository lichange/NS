using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Exceptions
{
    /// <summary>
    /// 异常处理器的handler factory
    /// </summary>
    public class ExceptionHandlerFactory
    {
        #region 单例Mode
        private static readonly object lock_flag = new object();
        private static readonly object tempLock_flag = new object();
        public static ExceptionHandlerFactory factory = null;
        private static IDictionary<Type, IList<IExceptionHanlder>> exceptionHandlerContainer = new Dictionary<Type, IList<IExceptionHanlder>>();

        public static ExceptionHandlerFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lock_flag)
                    {
                        if (factory == null)
                        {
                            factory = new ExceptionHandlerFactory();
                            factory.Init();
                        }
                    }
                }
                return factory;
            }
        }
        #endregion

        public void Init()
        {
            exceptionHandlerContainer.Add(typeof(FrameworkException), new List<IExceptionHanlder>() { new FrameworkExceptionHanlder() });
            exceptionHandlerContainer.Add(typeof(ValidationException), new List<IExceptionHanlder>() { new ValidationExceptionHanlder() });
            exceptionHandlerContainer.Add(typeof(Exception), new List<IExceptionHanlder>() { new DefaultExceptionHanlder() });
        }

        /// <summary>
        /// 新增异常处理器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void AddExceptionHandler(Type type, IExceptionHanlder handler)
        {
            if (type == null || handler == null)
                return;

            if (exceptionHandlerContainer.ContainsKey(type))
            {
                exceptionHandlerContainer[type].Add(handler);
            }
            else
            {
                lock (tempLock_flag)
                {
                    exceptionHandlerContainer.Add(type, new List<IExceptionHanlder>() { handler });
                }
            }
        }

        /// <summary>
        /// 新增异常处理器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handler"></param>
        public void AddExceptionHandler<T>(IExceptionHanlder handler) where T : Exception
        {
            if (handler == null)
                return;

            var type = typeof(T);

            if (exceptionHandlerContainer.ContainsKey(type))
            {
                exceptionHandlerContainer[type].Add(handler);
            }
            else
            {
                lock (tempLock_flag)
                {
                    exceptionHandlerContainer.Add(type, new List<IExceptionHanlder>() { handler });
                }
            }
        }

        /// <summary>
        /// 获取异常处理器
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <returns></returns>
        public IExceptionHanlder GetExceptionHandler<T>() where T : Exception
        {
            if (exceptionHandlerContainer.ContainsKey(typeof(T)))
            {
                var tempHanlders = exceptionHandlerContainer[typeof(T)];

                if (tempHanlders.Count > 0)
                    return tempHanlders[0];
            }

            var defaultExceptionHandlers = exceptionHandlerContainer[typeof(Exception)];

            if (defaultExceptionHandlers.Count > 0)
                return defaultExceptionHandlers[0];
            return null;
        }

        /// <summary>
        /// 获取异常处理器
        /// </summary>
        /// <typeparam name="T">异常类型</typeparam>
        /// <returns></returns>
        public IExceptionHanlder GetExceptionHandler(Type type)
        {
            if (exceptionHandlerContainer.ContainsKey(type))
            {
                var tempHanlders = exceptionHandlerContainer[type];

                if (tempHanlders.Count > 0)
                    return tempHanlders[0];
            }

            var defaultExceptionHandlers = exceptionHandlerContainer[typeof(Exception)];

            if (defaultExceptionHandlers.Count > 0)
                return defaultExceptionHandlers[0];
            return null;
        }
    }
}
