using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对特定的方法的参数执行特殊的业务逻辑处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.GenericParameter | AttributeTargets.Property)]
    public class ParameterValidationAttribute : Attribute
    {
        private Type handlerType;
        public ParameterValidationAttribute(Type tempHandlerType)
        {
            this.handlerType = tempHandlerType;
        }

        /// <summary>
        /// 参数进行处理的具体的函数
        /// </summary>
        public Type HandlerType
        {
            get
            {
                return this.handlerType;
            }
        }
    }
}
