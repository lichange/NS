using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace NS.Framework.Exceptions
{
    /// <summary>
    /// 异常处理上下文
    /// </summary>
    public class ExceptionHandlerContext
    {
        public Exception Exception
        {
            get;
            set;
        }

        public ExceptionContext ExceptionContext
        {
            get;
            set;
        }

        public ActionExecutedContext HttpActionExecutedContext
        {
            get;
            set;
        }

        /// <summary>
        /// 异常对象
        /// </summary>
        public object Target
        {
            get;
            set;
        }

        /// <summary>
        /// 异常方法
        /// </summary>
        public MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// 参数集合
        /// </summary>
        public IList<object> Parameters
        {
            get;
            set;
        }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Operator
        {
            get;
            set;
        }
    }
}
