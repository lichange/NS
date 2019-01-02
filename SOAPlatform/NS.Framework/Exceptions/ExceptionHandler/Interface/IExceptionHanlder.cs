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
    public interface IExceptionHanlder
    {
        /// <summary>
        /// Handler 处理的异常类型
        /// </summary>
        Type ExceptionType
        {
            get;
        }

        /// <summary>
        /// 异常对象上下文
        /// </summary>
        ExceptionHandlerContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// 异常处理方法
        /// </summary>
        void Handler();

        /// <summary>
        /// 处理结果
        /// </summary>
        object Result
        {
            get;
            set;
        }

        string ErrorMessage
        {
            get;
            set;
        }
    }
}
