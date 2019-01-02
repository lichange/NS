using System.ComponentModel.DataAnnotations;
using NS.DDD.Core.Validation;
using NS.DDD.Core.Internal.Validation;
using NS.Framework.IOC;
using NS.Framework.Log;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NS.Framework.Utility.Reflection;
using ValidationAttribute = NS.Framework.Attributes.ValidationAttribute;
using ValidationException = NS.Framework.Exceptions.ValidationException;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// 用于统一验证的对象拦截器。-支持扩展
    /// </summary>
    public class ValidatorInterceptionBehavior : IInterceptionBehavior
    {
        #region IInterceptionBehavior Members
        /// <summary>
        /// 获取当前行为需要拦截的对象类型接口。
        /// </summary>
        /// <returns>所有需要拦截的对象类型接口。</returns>
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 通过实现此方法来拦截调用并执行所需的拦截行为。
        /// </summary>
        /// <param name="input">调用拦截目标时的输入信息。</param>
        /// <param name="getNext">通过行为链来获取下一个拦截行为的委托。</param>
        /// <returns>从拦截目标获得的返回信息。</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (getNext == null)
                throw new ArgumentNullException("getNext");

            //执行验证操作；
            var tempMethod = input.MethodBase as System.Reflection.MethodInfo;

            var inputParameterCollection = input.Inputs;

            //根据反射获取对象的成员信息
            var realArguments = (object[])ReflectHelper.GetInstanceProperty(inputParameterCollection, "arguments");

            if (inputParameterCollection.Count == 0)
                return getNext().Invoke(input, getNext);

            //获取方法参数
            //System.Reflection.ParameterInfo[] parameterInfos = input.MethodBase.GetParameters();

            IList<EntityValidationResult> tempResults = new List<EntityValidationResult>();
            foreach (var realArgument in realArguments)
            {
                var entityValidationResult = new EntityValidationResult(realArgument as IAggregateRoot, new List<EntityValidationError>());
                tempResults.Add(entityValidationResult);
            }

            var flag = true;

            //进行数据验证
            flag = this.ValidateInputParameter(realArguments, ref tempResults);
            //if (parameterInfos != null && parameterInfos.Length > 0)
            //{
            //    //验证所有的参数，判定参数类型，是否需要验证
            //    flag = this.ValidateInputParameter(parameterInfos, ref results);
            //}

            //进行业务逻辑验证
            var outMessage = string.Empty;
            //flag = this.ValidateInputParameter(tempMethod,inputParameterCollection, out outMessage);
            if (!flag)
            {
                return new VirtualMethodReturn(input, new ValidationException(tempResults[0].ValidationErrors.First().ErrorMessage));
            }

            return getNext().Invoke(input, getNext);
        }

        ///// <summary>
        ///// 验证输入的参数业务规则是否符合预期
        ///// </summary>
        ///// <param name="inputParameterCollection">输入参数</param>
        ///// <returns></returns>
        //protected virtual bool ValidateInputParameter(System.Reflection.MethodInfo methodInfo,IParameterCollection parameterInfos, out string outMessage)
        //{
        //    bool flag = true;
        //    outMessage = string.Empty;
        //    for (int i = 0; i < parameterInfos.Count; i++)
        //    {
        //        ParameterInfo tempParameterInfo = parameterInfos.GetParameterInfo(i);

        //        if (tempParameterInfo == null)
        //            continue;

        //        //var te1 = tempParameterInfo.Attributes;
        //        //var tw1 = tempParameterInfo.CustomAttributes;
        //        //var aa = Attribute.GetCustomAttributes(tempParameterInfo);
        //        //var tt= CustomAttributeData.GetCustomAttributes(tempParameterInfo);
        //        //var attributes = tempParameterInfo.GetCustomAttributes(typeof(ParameterValidationAttribute), false);
        //        //if (attributes == null || attributes.Length == 0)
        //        //    continue;

        //        //var currentAttribute = attributes[0] as ParameterValidationAttribute;
        //        ILogicValidator customerValidator = LogicValidationFactory.CreateValidator(methodInfo,tempParameterInfo);
        //        if (customerValidator == null)
        //            continue;

        //        var validateResult = customerValidator.Validate(parameterInfos[i]);

        //        flag = validateResult.IsSuccess;
        //        outMessage = validateResult.Message;
        //        if (!flag)
        //        {
        //            break;
        //        }
        //    }

        //    return flag;
        //}

        protected virtual bool ValidateInputParameter(object[] parameterInfos, ref IList<EntityValidationResult> validateResults)
        {
            bool flag = true;
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo == null)
                    continue;

                if ((parameterInfo is IAggregateRoot || parameterInfo is IEntity))
                    continue;

                var tempValidationProvider = ValidationFactory.Create();

                var internalValidateEntity = new InternalValidateEntity(parameterInfo as IAggregateRoot);

                var customerValidator = tempValidationProvider.GetEntityValidator(internalValidateEntity);

                if (internalValidateEntity.EntityValue == null)
                    continue;

                var tempEntityValidationContext = new EntityValidationContext(internalValidateEntity, new ValidationContext(internalValidateEntity.EntityValue, null, new Dictionary<object, object>()));

                var results = customerValidator.Validate(tempEntityValidationContext);

                var oldResult = validateResults.Where(pre => pre.Entry == internalValidateEntity.EntityValue).FirstOrDefault();
                if (oldResult == null)
                    continue;

                validateResults.Remove(oldResult);

                validateResults.Add(results);

                flag = results.IsValid;

                if (!flag)
                    break;
            }

            return flag;
        }

        //protected virtual bool ValidateInputParameter(System.Reflection.ParameterInfo[] parameterInfos, ref ValidationResults validateResults)
        //{
        //    bool flag = true;
        //    foreach (System.Reflection.ParameterInfo parameterInfo in parameterInfos)
        //    {
        //        if (parameterInfo.IsOut)
        //            continue;

        //        if (parameterInfo.Member == null)
        //            continue;

        //        var attributes = parameterInfo.ParameterType.GetCustomAttributes(typeof(ValidationAttribute), false);
        //        if (attributes == null || attributes.Length == 0)
        //            continue;

        //        Validator customerValidator = ValidationFactory.CreateValidator(parameterInfo.ParameterType);

        //        var results = customerValidator.Validate(parameterInfo);

        //        if (results.Count > 0)
        //        {
        //            flag = results.IsValid;
        //        }

        //        if (!flag)
        //        {
        //            validateResults.AddAllResults(results);
        //            break;
        //        }
        //    }

        //    return flag;
        //}

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示当前拦截行为被调用时，是否真的需要执行
        /// 某些操作。
        /// </summary>
        public bool WillExecute
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}
