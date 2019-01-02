using NS.Framework.IOC;
using NS.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Exceptions
{
    /// <summary>
    /// 统一的Except Handler 接口，采用命令模式，该handler接口是核心定义
    /// </summary>
    public class DefaultExceptionHanlder : IExceptionHanlder
    {
        public DefaultExceptionHanlder()
        {

        }
        public DefaultExceptionHanlder(ExceptionHandlerContext context)
        {
            this.Context = context;
        }
        /// <summary>
        /// Handler 处理的异常类型
        /// </summary>
        public Type ExceptionType
        {
            get
            {
                return typeof(Exception);
            }
        }

        /// <summary>
        /// 异常对象上下文
        /// </summary>
        public ExceptionHandlerContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// 异常处理方法
        /// </summary>
        public void Handler()
        {
            //记录日志
            ILogProvider logProvider = ObjectContainer.CreateInstance<ILogProvider>();
            var tempMessage = string.Format("Operator:{0};Target:{1};Method:{2};Parameter:{3};Detail:{4}", this.Context.Operator, this.Context.Target, this.Context.Parameters, this.Context.Method, Context.Exception.Message);
            logProvider.Info(Context.Exception.Message, Context.Exception);
            this.ErrorMessage = tempMessage;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public object Result
        {
            get;
            set;
        }
    }
}
