//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Unity;

//namespace NS.Framework.Service.Implement
//{
//    internal class WCFPlatform : MarshalByRefObject, IWCFPlatform
//    {
//        private IUnityContainer pfuc;

//        /// <summary>
//        /// 请注意,构造函数中对于子模块注册顺序是无约定的,对于子模块的调用,请在OnStarting方法中调用,
//        /// 否则可能出现模块尚未被初始化成功就被其他模块的构造函数调用的错误
//        /// </summary>
//        /// <param name="pfuc"></param>
//        public WCFPlatform(IUnityContainer pfuc)
//        {
//            this.pfuc = pfuc;

//            // 平台的配置管理类
//            //PlatformConfiguration pfcfg = new PlatformConfiguration(pfuc);
//            //this.StartPlatformEvent += pfcfg.OnStartPlatform;
//            //this.StopPlatformEvent += pfcfg.OnStopPlatform;
//            //pfuc.RegisterInstance(typeof(PlatformConfiguration), pfcfg);
//            //pfuc.RegisterInstance(typeof(IHapConfiguration), pfcfg);

//            // 平台的命名管理类
//            //pfuc.RegisterInstance(typeof(PlatformNameManager), new PlatformNameManager());

//            // 平台的服务管理类
//            ServiceManager pfServiceManager = new ServiceManager(pfuc);
//            this.StartPlatformEvent += pfServiceManager.OnStartPlatform;
//            this.StopPlatformEvent += pfServiceManager.OnStopPlatform;
//            pfuc.RegisterInstance(typeof(IServiceManager), pfServiceManager);

//            //// 平台的服务发现类           
//            ServiceDiscovery pfServiceDiscovery = new ServiceDiscovery();
//            this.StartPlatformEvent += pfServiceDiscovery.OnStartPlatform;
//            this.StopPlatformEvent += pfServiceDiscovery.OnStopPlatform;
//            pfuc.RegisterInstance(typeof(IServiceDiscovery), pfServiceDiscovery);

//            // 平台的服务动态发布类--根据配置的策略来做
//            var publishType = NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.PublishType;
//            switch (publishType.ToLower())
//            {
//                case "wcfservice":
//                    DefaultServicePublisher pfServicePublisher = new DefaultServicePublisher(pfuc);
//                    this.StartPlatformEvent += pfServicePublisher.OnStartPlatform;
//                    this.StopPlatformEvent += pfServicePublisher.OnStopPlatform;
//                    pfuc.RegisterInstance(typeof(IServicePublisher), pfServicePublisher);
//                    break;
//                case "restful":
//                    RestfulServicePublisher restfulServicePublisher = new RestfulServicePublisher(pfuc);
//                    this.StartPlatformEvent += restfulServicePublisher.OnStartPlatform;
//                    this.StopPlatformEvent += restfulServicePublisher.OnStopPlatform;
//                    pfuc.RegisterInstance(typeof(IServicePublisher), restfulServicePublisher);
//                    break;
//            }
//        }

//        #region IWCFPlatform 成员

//        public IServiceManager GetServiceManager()
//        {
//            return this.pfuc.Resolve<IServiceManager>();
//        }

//        public IServiceDiscovery GetServiceDiscovery()
//        {
//            return this.pfuc.Resolve<IServiceDiscovery>();
//        }

//        public IServicePublisher GetServicePublisher()
//        {
//            return this.pfuc.Resolve<IServicePublisher>();
//        }

//        public void Start()
//        {
//            if (StartPlatformEvent != null)
//                StartPlatformEvent(this, new EventArgs());
//        }

//        public void Stop()
//        {
//            if (StopPlatformEvent != null)
//                StopPlatformEvent(this, new EventArgs());
//        }

//        public event EventHandler StartPlatformEvent;

//        public event EventHandler StopPlatformEvent;

//        #endregion
//    }
//}
