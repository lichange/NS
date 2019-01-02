using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.PolicyInjection.Pipeline;

namespace NS.Framework.AOP.Interceptors
{
    /// <summary>
    /// 方法调用转换为消息的拦截器
    /// </summary>
    public class MethodCallToMessageInterceptionBehavior : BaseInterceptionBehavior
    {
        public override bool IsIntercption(IMethodInvocation methodInvocation)
        {
            return true;
        }

        protected override void BeforeInvoke(IMethodInvocation methodInvocation)
        {
            
        }

        protected override void AfterInvoke(IMethodInvocation methodInvocation)
        {
            
        }

        protected override IMethodReturn ExcuteInvoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            return base.ExcuteInvoke(input, getNext);
        }
    }
}
