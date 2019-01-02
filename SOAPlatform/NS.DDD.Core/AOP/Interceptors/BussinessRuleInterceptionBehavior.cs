// ***********************************************************************
// Assembly         : NS.DDD.Core
// Author           : Administrator
// Created          : 06-28-2013
//
// Last Modified By : Administrator
// Last Modified On : 06-28-2013
// ***********************************************************************
// <copyright file="BussinessRuleInterceptionBehavior.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using NS.DDD.Core.Validation;
using NS.Framework.Attributes;
using NS.Framework.Exceptions;
using NS.Framework.IOC;
using NS.Framework.Log;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NS.Framework.Utility.Reflection;
using EmitMapper;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// 业务规则对象拦截器。-支持扩展
    /// </summary>
    public class BussinessRuleInterceptionBehavior : IInterceptionBehavior
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
        /// <exception cref="System.ArgumentNullException">
        /// input
        /// or
        /// getNext
        /// </exception>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (getNext == null)
                throw new ArgumentNullException("getNext");

            //执行验证操作；
            var tempMethod = input.MethodBase as System.Reflection.MethodInfo;

            ////当方法没有返回值时 不许做任何的验证操作
            //if (tempMethod.ReturnType == null)
            //    return getNext().Invoke(input, getNext);

            var inputParameterCollection = input.Inputs;
            var arguments = input.Arguments;
            //input.

            //根据反射获取对象的成员信息
            var realArguments = (object[])ReflectHelper.GetInstanceProperty(arguments, "arguments");

            //预处理参数
            //this.PrepareDealArguments(realArguments);

            if (inputParameterCollection.Count == 0)
                return getNext().Invoke(input, getNext);

            IList<ParameterInfo> parameterInfos = new List<ParameterInfo>();

            //进行数据验证
            this.ParseParameter(arguments, ref parameterInfos);

            //this.ParseParameter(inputParameterCollection, ref parameterInfos);

            #region 持久化规则验证

            //进行业务逻辑验证
            if (parameterInfos.Count == 0)
                return getNext().Invoke(input, getNext);

            var targetName = input.Target.GetType().Name.TrimStart('I').Replace("Repository", "").Replace("AppService", "");
            //TODO...
            //执行业务规则
            IList<object> bussinessRules = RuleHandlerContainer.GetRuleHandlers(parameterInfos[0].ParameterType, tempMethod.Name,targetName);

            if(bussinessRules.Count==0)
                return getNext().Invoke(input, getNext);

            #endregion

            bool flag = (bool)RuleHandlerInvoker.Invoke(bussinessRules[0], realArguments);
            if (flag)
                return getNext().Invoke(input, getNext);
            else
                return new VirtualMethodReturn(input, new Exception(string.Format("无法执行该操作,错误原因:{0}", "无法删除")));
        }

        private void PrepareDealArguments(object[] realArguments)
        {
            if (realArguments == null || realArguments.Length == 0)
                return;

            var count = realArguments.Length;

            for (int i = 0; i < count; i++)
            {
                if (realArguments[i] == null)
                    continue;

                if (realArguments[i].GetType().GetInterfaces().Where(Predicate => Predicate.Name == (typeof(IAggregateRoot)).Name).Count() == 0)
                    continue;
                var tempInstance = (IAggregateRoot)Activator.CreateInstance(realArguments[i].GetType());
                var objInstance = realArguments[i] as IAggregateRoot;
                 ObjectMapperManager.DefaultInstance.GetMapper<IAggregateRoot, IAggregateRoot>()
                              .Map(objInstance, tempInstance);

                 realArguments[i] = tempInstance;
            }
        }

        /// <summary>
        /// Parses the parameter.
        /// </summary>
        /// <param name="inputParameterCollection">The input parameter collection.</param>
        /// <param name="parameterInfos">The parameter infos.</param>
        private void ParseParameter(IParameterCollection inputParameterCollection, ref IList<ParameterInfo> parameterInfos)
        {
            for (int i = 0; i < inputParameterCollection.Count; i++)
            {
                ParameterInfo tempParameterInfo = inputParameterCollection.GetParameterInfo(i);

                if (tempParameterInfo == null)
                    continue;

                parameterInfos.Add(tempParameterInfo);
            }
        }

        /// <summary>
        /// 获取一个<see cref="Boolean" />值，该值表示当前拦截行为被调用时，是否真的需要执行
        /// 某些操作。
        /// </summary>
        /// <value><c>true</c> if [will execute]; otherwise, <c>false</c>.</value>
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
