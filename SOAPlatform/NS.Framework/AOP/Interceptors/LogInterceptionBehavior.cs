using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Practices.Unity.InterceptionExtension;

using NS.Framework.IOC;
using NS.Framework.Log;
using NS.Framework.Attributes;
using NS.Framework.Utility.Reflection;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace NS.Framework.AOP.Interceptors
{
    /// <summary>
    /// 用于统一记录日志的方法拦截器。-支持扩展
    /// </summary>
    public class LogInterceptionBehavior : IInterceptionBehavior
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

            var tempLogAttributes = tempMethod.GetCustomAttributes(typeof(LogAttribute));

            if (tempLogAttributes.Count() == 0)
                return getNext().Invoke(input, getNext);

            //根据反射获取对象的成员信息
            var realArguments = (object[])ReflectHelper.GetInstanceProperty(inputParameterCollection, "arguments");

            if (inputParameterCollection.Count == 0)
                return getNext().Invoke(input, getNext);

            //计时器开始 监视方法执行时间---Debug模式下启用

            //开始监视
            //计时器
#if DEBUG
            var stopwatch = Stopwatch.StartNew();
            var tempReturnResult = getNext().Invoke(input, getNext);
            stopwatch.Stop();
            var totalMillisencodes = stopwatch.Elapsed.TotalMilliseconds;
#else
            var tempReturnResult = getNext().Invoke(input, getNext);
#endif
            //生成日志对象
            var logInfo = string.Format("TODO....");
            //输出到log---采用异步方式
            ILogProvider logProvider = ObjectContainer.CreateInstance<ILogProvider>();
            logProvider.Debug(logInfo);
            return tempReturnResult;
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
