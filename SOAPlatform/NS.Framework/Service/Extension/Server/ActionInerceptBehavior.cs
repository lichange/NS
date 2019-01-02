///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System.Collections.ObjectModel;
//    using System.ServiceModel;
//    using System.ServiceModel.Channels;
//    using System.ServiceModel.Description;

//    /// <summary>
//    /// 自定义扩张WCF行为---方法调用拦截器
//    /// </summary>
//    internal class ActionInterceptBehavior : IServiceBehavior
//    {
//        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase host)
//        {
//            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
//            {
//                foreach (OperationDescription operation in endpoint.Contract.Operations)
//                {
//                    operation.Behaviors.Add(new OperationBehavior());
//                }
//            }
//        }
//        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
//        { }

//        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        { }
//    }
//}
