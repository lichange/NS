//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel.Web;
//using System.Text;

//using Microsoft.Practices.Unity;
//using NS.Framework.Exceptions;
//using System.ServiceModel;
//using System.ServiceModel.Description;
//using NS.Framework.Utility.Collections;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// 服务发布器-动态发布WCF服务
//    /// </summary>
//    internal class RestfulServicePublisher : IServicePublisher
//    {
//        private IUnityContainer _container;
//        private IServiceManager serviceManager;

//        /// <summary>
//        /// 同步flag
//        /// </summary>
//        private static readonly object lock_flag = new object();

//        /// <summary>
//        /// Servicehost字典,负责存储所有的宿主信息
//        /// </summary>
//        private ThreadSafeDictionary<Type, WebServiceHost> serviceHostDictionary = new ThreadSafeDictionary<Type, WebServiceHost>();

//        public RestfulServicePublisher(IUnityContainer container)
//        {
//            this._container = container;
//            this.serviceManager = this._container.Resolve<IServiceManager>();
//        }

//        #region IServicePublisher 成员

//        public bool Publish(string serviceImplementName)
//        {
//            if (this.serviceManager == null)
//                throw new NotFoundException("尚未初始化:IServiceManager");

//            var serviceMetaData = serviceManager.GetServiceMetaData(serviceImplementName);

//            return this.Publish(serviceMetaData);
//        }

//        public bool Publish(params string[] serviceContractNames)
//        {
//            bool flag = false;
//            if (serviceContractNames == null)
//                return flag;

//            foreach (var serviceContractName in serviceContractNames)
//            {
//                flag = this.Publish(serviceContractName);
//            }

//            return flag;
//        }

//        public bool Publish(string serviceName, string binding, string address)
//        {
//            try
//            {
//                if (this.serviceManager == null)
//                    throw new NotFoundException("尚未初始化:IServiceManager");

//                var description =
//                    this.serviceManager.GetServiceMetaData(serviceName);

//                if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                    return false;

//                WebServiceHost tempHost = ServicePublishHelper.PublishHelper.CreateWebServiceHost(description.ImplementType);

//                if (this.serviceHostDictionary.ContainsKey(description.ImplementType))
//                {
//                    if (tempHost.State != CommunicationState.Opened || tempHost.State == CommunicationState.Opening)
//                    {
//                        tempHost.Open();

//                        //this.publishManagerServiceDictionary[description.ImplementType].Url =
//                        //    tempHost.BaseAddresses[0].AbsoluteUri;
//                        description.IsOpen = true;
//                        description.OutServiceURL = tempHost.BaseAddresses[0].AbsoluteUri;
//                    }
//                }
//                else
//                {
//                    System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);

//                    Uri endPointAddress = null;
//                    if (!string.IsNullOrEmpty(address))
//                    {
//                        endPointAddress = new Uri(address);
//                    }
//                    else
//                    {
//                        endPointAddress = new Uri(string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(description.Binding), ServicePublishHelper.PublishHelper.GetHostName(), description.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : description.Port, description.ServiceContract));
//                    }

//                    if (!string.IsNullOrEmpty(binding))
//                    {
//                        tempHost.AddServiceEndpoint(description.ServiceContract, ServicePublishHelper.PublishHelper.GetBinding(tempBinding), endPointAddress);
//                    }
//                    else
//                    {
//                        tempHost.AddServiceEndpoint(description.ServiceContract, ServicePublishHelper.PublishHelper.GetBinding(description.Binding), endPointAddress);
//                    }

//                    //服务行为
//                    ServiceDebugBehavior serviceDebugBehaviour =
//                                    tempHost.Description.Behaviors.Find<ServiceDebugBehavior>();
//                    serviceDebugBehaviour.IncludeExceptionDetailInFaults = NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.IsEnableDebug;

//                    //判断是否以及打开连接，如果尚未打开，就打开侦听端口
//                    if (tempHost.State != CommunicationState.Opening)
//                        tempHost.Open();

//                    description.OutServiceURL = endPointAddress.AbsoluteUri;

//                    //添加到Host容器中
//                    this.serviceHostDictionary.Add(description.ImplementType, tempHost);

//                    //添加到服务发布缓存中
//                    lock (lock_flag)
//                    {
//                        //if (!this.publishManagerServiceDictionary.ContainsKey(description.ImplementType))
//                        //{
//                        //    //该种方式，后期提供...
//                        //    var managerService = new ManagedServiceDescription();
//                        //    managerService.Url = description.OutServiceURL;
//                        //    managerService.Name = description.ServiceContract.FullName;
//                        //    managerService.DisplayName = description.ImplementType.FullName;

//                        //    this.publishManagerServiceDictionary.Add(description.ImplementType, managerService);
//                        //}
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }

//            return false;
//        }

//        public bool Publish(string serviceName, string binding)
//        {
//            return this.Publish(serviceName, binding, null);
//        }

//        public bool IsPublish(string serviceName)
//        {
//            return this.Publish(serviceName, null);
//        }

//        public bool Publish(ServiceMetaData serviceDescription)
//        {
//            return this.Publish(serviceDescription.ImplementType, serviceDescription.Binding, serviceDescription.Address);
//        }

//        private bool Publish(Type ImplementType, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress address)
//        {
//            if (this.serviceManager == null)
//                throw new NotFoundException("尚未初始化:IServiceManager");

//            var description =
//                this.serviceManager.GetServiceMetaData(ImplementType.Name);

//            if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                return false;

//            Uri baseUri = null;
//            System.ServiceModel.Channels.Binding tempBinding = new WebHttpBinding();

//            tempBinding.SendTimeout = new TimeSpan(0, 3, 0);

//            //定义端口号-如果时非TCP的情况下，一个端口号可以发布多个服务,但是TCP情况下，不可以
//            var tempPort = (description.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : description.Port);

//            //if (tempBinding != null && tempBinding.GetType() == typeof(System.ServiceModel.NetTcpBinding))
//            //{
//            //    //共享端口号的设置打开，否则无法注册多服务
//            //    ((System.ServiceModel.NetTcpBinding)tempBinding).PortSharingEnabled = true;
//            //}

//            baseUri = new Uri(string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(new BasicHttpBinding()), ServicePublishHelper.PublishHelper.GetHostName(), (description.Port == null ? (NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port + 1).ToString() : description.Port), description.ServiceContract));

//            WebServiceHost tempHost = ServicePublishHelper.PublishHelper.CreateWebServiceHost(description.ImplementType,baseUri);

//            if (this.serviceHostDictionary.ContainsKey(description.ImplementType))
//            {
//                tempHost = this.serviceHostDictionary[description.ImplementType];

//                if (tempHost.State != CommunicationState.Opened || tempHost.State == CommunicationState.Opening)
//                {
//                    tempHost.Open();

//                    //this.publishManagerServiceDictionary[description.ImplementType].Url =
//                    //    tempHost.BaseAddresses[0].AbsoluteUri;
//                    description.IsOpen = true;
//                    description.OutServiceURL = tempHost.BaseAddresses[0].AbsoluteUri;
//                }
//            }
//            else
//            {
//                ServiceEndpoint serviceEndpoint = null;
//                Uri endPointAddress = null;
//                if (address != null && address.Uri != null && !string.IsNullOrEmpty(address.Uri.ToString()))
//                {
//                    endPointAddress = address.Uri;
//                }
//                else
//                {
//                    endPointAddress = new Uri(string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), ServicePublishHelper.PublishHelper.GetHostName(), (description.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : description.Port), description.ServiceContract));
//                }

//                if (binding != null)
//                {
//                  serviceEndpoint=  tempHost.AddServiceEndpoint(description.ServiceContract, ServicePublishHelper.PublishHelper.GetBinding(binding), endPointAddress);
//                }
//                else
//                {
//                   serviceEndpoint= tempHost.AddServiceEndpoint(description.ServiceContract, ServicePublishHelper.PublishHelper.GetBinding(tempBinding), endPointAddress);
//                }

//                WebHttpBehavior webHttpBehavior = tempHost.Description.Behaviors.Find<WebHttpBehavior>();

//                if(webHttpBehavior==null)
//                {
//                    webHttpBehavior=new WebHttpBehavior();
//                    webHttpBehavior.FaultExceptionEnabled = NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.IsEnableDebug;
//                    webHttpBehavior.AutomaticFormatSelectionEnabled = true;
//                    serviceEndpoint.Behaviors.Add(webHttpBehavior);

//                }
                
//                //服务行为
//                //ServiceDebugBehavior serviceDebugBehaviour =
//                //                tempHost.Description.Behaviors.Find<ServiceDebugBehavior>();
//                //serviceDebugBehaviour.IncludeExceptionDetailInFaults = NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.IsEnableDebug;

//                //发布元数据
//                ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior()
//                {
//                    HttpGetEnabled = true
//                };

//                metadataBehavior.HttpGetUrl = baseUri;
//                tempHost.Description.Behaviors.Add(metadataBehavior);

//                //设置服务的超时和其他的基本参数信息

//                //判断是否以及打开连接，如果尚未打开，就打开侦听端口
//                if (tempHost.State != CommunicationState.Opening)
//                    tempHost.Open();

//                description.OutServiceURL = endPointAddress.AbsoluteUri;

//                //添加到Host容器中
//                this.serviceHostDictionary.Add(description.ImplementType, tempHost);

//                //添加到服务发布缓存中
//                lock (lock_flag)
//                {
//                    //if (!this.publishManagerServiceDictionary.ContainsKey(description.ImplementType))
//                    //{
//                    //    //该种方式，后期提供...
//                    //    var managerService = new ManagedServiceDescription();
//                    //    managerService.Url = description.OutServiceURL;
//                    //    managerService.Name = description.ServiceContract.FullName;
//                    //    managerService.DisplayName = description.ImplementType.FullName;

//                    //    this.publishManagerServiceDictionary.Add(description.ImplementType, managerService);
//                    //}
//                }
//            }

//            return true;
//        }

//        public bool Publish(params ServiceMetaData[] serviceDescriptions)
//        {
//            bool flag = false;
//            if (serviceDescriptions == null)
//                return flag;

//            foreach (var serviceDescription in serviceDescriptions)
//            {
//                flag = this.Publish(serviceDescription);
//            }

//            return flag;
//        }

//        public ServiceMetaData GetServiceMetaData(string serviceName)
//        {
//            if (this.serviceManager == null)
//                throw new NotFoundException("尚未初始化:IServiceManager");

//            return this.serviceManager.GetServiceMetaData(serviceName);
//        }

//        public IList<ServiceMetaData> GetServiceMetaDatas()
//        {
//            return this.serviceManager.GetAllServiceMetaDatas();
//        }

//        #endregion

//        #region 停止发布

//        public bool UnPublish(string serviceImplementName)
//        {
//            if (this.serviceManager == null)
//                throw new NotFoundException("尚未初始化:IServiceManager");

//            var description =
//                this.serviceManager.GetServiceMetaData(serviceImplementName);

//            if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                return false;

//            if (!this.serviceHostDictionary.ContainsKey(description.ImplementType))
//                return false;

//            try
//            {
//                this.serviceHostDictionary[description.ImplementType].Close();
//            }
//            catch
//            {
//                return false;
//            }
//            return true;
//        }

//        public bool UnPublish(params string[] serviceImplementNames)
//        {
//            if (this.serviceManager == null)
//                throw new NotFoundException("尚未初始化:IServiceManager");

//            try
//            {
//                foreach (var serviceImplementName in serviceImplementNames)
//                {
//                    var description =
//                 this.serviceManager.GetServiceMetaData(serviceImplementName);

//                    if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                        continue;

//                    if (!this.serviceHostDictionary.ContainsKey(description.ImplementType))
//                        continue;

//                    this.serviceHostDictionary[description.ImplementType].Close();
//                }
//            }
//            catch
//            {
//                return false;
//            }

//            return true;
//        }

//        public bool UnPublish(ServiceMetaData description)
//        {
//            if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                return false;

//            if (!this.serviceHostDictionary.ContainsKey(description.ImplementType))
//                return false;

//            try
//            {
//                this.serviceHostDictionary[description.ImplementType].Close();
//            }
//            catch
//            {
//                return false;
//            }
//            return true;
//        }

//        public bool UnPublish(params ServiceMetaData[] serviceDescriptions)
//        {
//            try
//            {
//                foreach (var description in serviceDescriptions)
//                {
//                    if (description == null || description.ServiceContract == null || description.ImplementType == null)
//                        continue;

//                    if (!this.serviceHostDictionary.ContainsKey(description.ImplementType))
//                        continue;

//                    this.serviceHostDictionary[description.ImplementType].Close();
//                }
//            }
//            catch
//            {
//                return false;
//            }

//            return true;
//        }

//        #endregion

//        #region 平台启动或关闭时的处理
//        internal void OnStartPlatform(object sender, EventArgs e)
//        {
//            serviceHostDictionary.ClearAll();
//        }

//        internal void OnStopPlatform(object sender, EventArgs e)
//        {
//            serviceHostDictionary.ClearAll();
//        }
//        #endregion
//    }
//}
