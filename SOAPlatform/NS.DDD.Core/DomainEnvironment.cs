using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NS.DDD.Core.EventBus;
using NS.DDD.Core.UnitOfWork;
using NS.Framework.Utility;

//using IWS.Data;
//using IWS.Data.Impl;

namespace NS.DDD.Core
{
    /// <summary>
    /// 领域上下文环境
    /// </summary>
    public  class DomainEnvironment
    {
        /// <summary>
        /// 单例模式
        /// </summary>
        private static DomainEnvironment instance;
        private static object lock_flag = new object();

        public static DomainEnvironment Instance
        {
            get
            {
                if(instance==null)
                    lock (lock_flag)
                    {
                        if (instance==null)
                            instance = new DomainEnvironment();
                    }

                return instance;
            }
        }

        /// <summary>
        /// 同步执行的事件总线
        /// </summary>
        public IEventBus ImmediateEventBus { get; set; }

        /// <summary>
        /// 工作单元执行成功后的同步执行的事件总线
        /// </summary>
        public IEventBus PostCommitEventBus { get; set; }

        public Func<IUnitOfWork> UnitOfWorkFactory { get; set; }

        private DomainEnvironment()
        {
            ImmediateEventBus = new DefaultEventBus();
            PostCommitEventBus = new DefaultEventBus();
        }

        public static void Configure(Action<DomainEnvironment> action)
        {
            Check.NotNull(action, "action");
            action(Instance);
        }

        public DomainEnvironment RegisterEventHandlers(params Assembly[] assembliesToScan)
        {
            return RegisterEventHandlers(assembliesToScan as IEnumerable<Assembly>);
        }

        public DomainEnvironment RegisterEventHandlers(IEnumerable<Assembly> assembliesToScan)
        {
            Check.NotNull(assembliesToScan, "assembliesToScan");

            var immediateEventBus = ImmediateEventBus;
            var postCommitEventBus = PostCommitEventBus;

            if (immediateEventBus == null)
                throw new InvalidOperationException("Please register immediate event bus to the TaroEnvironment first.");

            if (postCommitEventBus == null)
                throw new InvalidOperationException("Please register post commit event bus to the TaroEvnironment first.");

            //immediateEventBus.RegisterHandlers(assembliesToScan);
            //postCommitEventBus.RegisterHandlers(assembliesToScan);

            return this;
        }

        public DomainEnvironment RegisterUnitOfWorkFactory(Func<IUnitOfWork> factory)
        {
            Check.NotNull(factory, "factory");
            UnitOfWorkFactory = factory;
            return this;
        }

        /// <summary>
        /// 持久化服务
        /// </summary>
        //public IPersistenceDAL CreatePersistence()
        //{
        //    return PersistenceFactory.CreatePersistence();
        //}
    }
}
