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
//    using System.ServiceModel.Dispatcher;

//    /// <summary>
//    /// 服务附加行为
//    /// </summary>
//    internal class UnityServiceBehavior : IServiceBehavior
//    {
//        public UnityInstanceProvider InstanceProvider { get; private set; }

//        public UnityServiceBehavior()
//        {
//            InstanceProvider = new UnityInstanceProvider();
//        }

//        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
//        {
//            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
//            {
//                ChannelDispatcher cd = cdb as ChannelDispatcher;
//                if (cd != null)
//                {
//                    foreach (EndpointDispatcher ed in cd.Endpoints)
//                    {
//                        InstanceProvider.ServiceType = serviceDescription.ServiceType;
//                        ed.DispatchRuntime.InstanceProvider = InstanceProvider;
//                    }
//                }
//            }
//        }

//        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

//        public void AddBindingParameters(
//            ServiceDescription serviceDescription,
//            ServiceHostBase serviceHostBase,
//            Collection<ServiceEndpoint> endpoints,
//            BindingParameterCollection bindingParameters)
//        {
//        }
//    }
//}

