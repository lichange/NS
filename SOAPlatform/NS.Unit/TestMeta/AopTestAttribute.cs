using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;

namespace NS.Unit.TestMeta
{
    public class AopTestAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return null;
        }
    }
}
