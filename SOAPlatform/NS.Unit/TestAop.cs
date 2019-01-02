using NS.Framework.AOP.Interceptors;
using NS.Unit.TestMeta;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;

namespace NS.Unit
{
    public class TestAop
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_LogInterceptionBehavior()
        {
            UnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.RegisterType<ITalk, PeopleTalk>(
                new InjectionConstructor("AOP", 18),
                new Interceptor<InterfaceInterceptor>(),
                new InterceptionBehavior<LogInterceptionBehavior>());
            ITalk talker = container.Resolve<ITalk>();
            bool isTalked = talker.talk("Test！");

            Assert.IsTrue(isTalked);
        }

    }
}
