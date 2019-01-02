///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System;
//    using System.ServiceModel;
//    using System.ServiceModel.Channels;
//    using System.ServiceModel.Dispatcher;
//    using Microsoft.Practices.Unity;
//    //using Microsoft.Practices.Unity.Configuration;

//    /// <summary>
//    /// 服务实例提供器-这里采用Unity实现
//    /// </summary>
//    internal class UnityInstanceProvider : IInstanceProvider
//    {
//        private static UnityContainer Container { set; get; }
//        public Type ServiceType { set; get; }

//        static UnityInstanceProvider()
//        {
//            Container = new UnityContainer();
//            //Container.LoadConfiguration();
//        }

//        public object GetInstance(InstanceContext instanceContext, Message message)
//        {
//            return Container.Resolve(ServiceType);
//        }

//        public object GetInstance(InstanceContext instanceContext)
//        {
//            return GetInstance(instanceContext, null);
//        }

//        public void ReleaseInstance(InstanceContext instanceContext, object instance)
//        {
//        }
//    }
//}
