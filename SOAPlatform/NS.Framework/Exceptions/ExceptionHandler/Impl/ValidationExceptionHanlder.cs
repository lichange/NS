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
    /// 数据验证异常处理器
    /// </summary>
    public class ValidationExceptionHanlder : IExceptionHanlder
    {
        public ValidationExceptionHanlder()
        {
        }
        public ValidationExceptionHanlder(ExceptionHandlerContext context)
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
                return typeof(ValidationException);
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
            var tempMessage = string.Format("统一验证的过程中出现异常，具体如下：Operator:{0};Target:{1};Method:{2};Parameter:{3};Detail:{4}", this.Context.Operator, this.Context.Target, this.Context.Parameters, this.Context.Method, Context.Exception.Message);
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
