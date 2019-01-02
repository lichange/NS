using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NS.DDD.Core.Validation;

namespace NS.Framework.IOC
{
    public class LogicValidatorContainer
    {
        public static void RegisterType(Type type)
        {
            InternalLogicValidatorFactory.Instance.RegisterType(type);
        }

        public static ILogicValidator CreateInstance(System.Reflection.MethodInfo methodInfo, System.Reflection.ParameterInfo parameterInfo)
        {
            return (ILogicValidator)InternalLogicValidatorFactory.Instance.CreateInstance(methodInfo, parameterInfo);
        }
    }
}
