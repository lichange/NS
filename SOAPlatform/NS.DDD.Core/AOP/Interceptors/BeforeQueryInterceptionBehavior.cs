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
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;

namespace NS.DDD.Core.AOP.Interceptors
{
    /// <summary>
    /// 查询前统一处理的对象拦截器。-支持扩展--目前 仅仅是处理当前执行的查询操作之前执行的动作
    /// </summary>
    public class BeforeQueryInterceptionBehavior : IInterceptionBehavior
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

            var targetType = tempMethod.DeclaringType;

            var targetName = input.Target.GetType().Name.TrimStart('I').Replace("Querier", "").Replace("Repository", "").Replace("AppService", "");
            //TODO...

            //执行业务规则
            IList<object> bussinessRules = RuleHandlerContainer.GetRuleHandlers(targetType, tempMethod.Name, targetName);

            if (bussinessRules.Count == 0)
                return getNext().Invoke(input, getNext);

            try
            {
                bool flag = (bool)RuleHandlerInvoker.Invoke(bussinessRules[0], null);
                if (flag)
                    return getNext().Invoke(input, getNext);
                else
                    return new VirtualMethodReturn(input, new Exception(string.Format("无法执行该操作,错误原因:{0}", "未知")));
            }
            catch (Exception ex)
            {
                return new VirtualMethodReturn(input, new Exception(string.Format("无法执行该操作,错误原因:{0}", ex.Message)));
            }
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
