///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System.ServiceModel;
//    using System.ServiceModel.Channels;
//    using System.ServiceModel.Description;
//    using System.ServiceModel.Dispatcher;

//    /// <summary>
//    /// 服务出现错误的附加处理行为
//    /// </summary>
//    internal class ServiceErrorBehavior : IServiceBehavior
//    {
//        public void Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
//        {
//        }

//        public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
//        {
//        }

//        public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
//        {
//            IErrorHandler errorHandler = new ErrorHandler();

//            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
//            {
//                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
//                channelDispatcher.ErrorHandlers.Add(errorHandler);
//            }
//        }
//    }        
//}
