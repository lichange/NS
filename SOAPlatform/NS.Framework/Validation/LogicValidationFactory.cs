using NS.Framework.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Validation
{
    /// <summary>
    /// 业务规则验证工厂方法
    /// </summary>
    public class LogicValidationFactory
    {
        public static ILogicValidator CreateValidator(System.Reflection.MethodInfo methodInfo, System.Reflection.ParameterInfo parameterInfo)
        {
            return (ILogicValidator)LogicValidatorContainer.CreateInstance(methodInfo, parameterInfo);
        }
    }
}
