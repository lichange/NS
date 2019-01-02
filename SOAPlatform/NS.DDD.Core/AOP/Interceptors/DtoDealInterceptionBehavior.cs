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

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// DTO统一处理的对象拦截器。-支持扩展--可能不仅仅是执行数据的预处理，还需要进行相应的数据验证
    /// </summary>
    public class DtoDealInterceptionBehavior : IInterceptionBehavior
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

            //DTO预处理-自动截取空字符串
            this.PreDealDto(realArguments);

            //IList<EntityValidationResult> tempResults = new List<EntityValidationResult>();
            //foreach (var realArgument in realArguments)
            //{
            //    var entityValidationResult = new EntityValidationResult(realArgument as IAggregateRoot, new List<EntityValidationError>());
            //    tempResults.Add(entityValidationResult);
            //}

            //var flag = true;

            //进行数据验证
            //flag = this.ValidateInputParameter(realArguments, ref tempResults);
            //if (parameterInfos != null && parameterInfos.Length > 0)
            //{
            //    //验证所有的参数，判定参数类型，是否需要验证
            //    flag = this.ValidateInputParameter(parameterInfos, ref results);
            //}

            //进行业务逻辑验证
            var outMessage = string.Empty;
            //flag = this.ValidateInputParameter(tempMethod,inputParameterCollection, out outMessage);
            //if (!flag)
            //{
            //    return new VirtualMethodReturn(input, new ValidationException(tempResults[0].ValidationErrors.First().ErrorMessage));
            //}

            return getNext().Invoke(input, getNext);
        }

        private void PreDealDto(object[] realArguments)
        {
            foreach (var realArgument in realArguments)
            {
                if (realArgument == null)
                    continue;

                if ((realArgument != null && realArgument.GetType().IsValueType) || !realArgument.GetType().IsClass || realArgument.GetType() == typeof(string))
                    continue;

                if (realArgument is IEnumerable<object>)
                {
                    var tempArguments = realArgument as IEnumerable<object>;

                    foreach (var tempArgument in tempArguments)
                    {
                        if (tempArgument == null)
                            continue;

                        this.PreDealDto(new object[] { tempArgument });
                    }

                    break;
                }

                var tempPropertys = realArgument.GetType().GetProperties();

                if (tempPropertys == null || tempPropertys.Length == 0)
                    continue;

                foreach (var tempProperty in tempPropertys)
                {
                    if (tempProperty != null && tempProperty.GetType().IsValueType)
                        continue;

                    var oldValue = tempProperty.GetValue(realArgument, null);

                    if (oldValue == null || !(oldValue is string))
                        continue;

                    var newValue = oldValue.ToString().Trim();

                    tempProperty.SetValue(realArgument, oldValue,null);
                }
            }
        }

        protected virtual bool ValidateInputParameter(object[] parameterInfos, ref IList<EntityValidationResult> validateResults)
        {
            bool flag = true;
            foreach (var parameterInfo in parameterInfos)
            {
                if (parameterInfo == null)
                    continue;

                var tempValidationProvider = ValidationFactory.Create();

                var internalValidateEntity = new InternalValidateEntity(parameterInfo as IAggregateRoot);

                var customerValidator = tempValidationProvider.GetEntityValidator(internalValidateEntity);

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
