//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NS.Framework.Exceptions;
//using NS.Framework.Utility.Collections;
//using NS.Framework.IOC;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// 服务代理创建容器--同步创建服务代理-不适合silverlight
//    /// </summary>
//    public class ServiceProxyContainer : IServiceProxyContainer
//    {
//        private ThreadSafeDictionary<string, object> channelFactories = new ThreadSafeDictionary<string, object>();
//        private ThreadSafeDictionary<Type, ServiceMetaData> interfaceServiceDictionary = new ThreadSafeDictionary<Type, ServiceMetaData>();
//        private ThreadSafeDictionary<Type, ServiceMetaData> implementServiceDictionary = new ThreadSafeDictionary<Type, ServiceMetaData>();
//        private IServiceDiscovery serviceDiscovery;

//        /// <summary>
//        /// 客户端创建的所有服务实例集合
//        /// </summary>
//        private ThreadSafeDictionary<Type, IContextChannel> clientContextChannelDictionary = new ThreadSafeDictionary<Type, IContextChannel>();

//        public ServiceProxyContainer()
//        {
//            serviceDiscovery = new ServiceDiscovery();
//            this.RegisterService(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServerDiscoveryPath, null);
//        }

//        #region 创建服务代理

//        public T GetServiceProxy<T>(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress endpoint) where T : class
//        {
//            ChannelFactory<T> channelFactory = null;

//            if (channelFactories.ContainsKey(typeof(T).FullName))
//            {
//                channelFactory = channelFactories[typeof(T).FullName] as ChannelFactory<T>;
//            }

//            if (channelFactory == null)
//            {
//                channelFactory = new ChannelFactory<T>(binding, endpoint);

//                channelFactories[typeof(T).FullName] = channelFactory;
//            }

//            return channelFactory.CreateChannel();
//        }

//        public T GetServiceProxy<T>(string endpointName) where T : class
//        {
//            if (string.IsNullOrEmpty(endpointName))
//            {
//                throw new ArgumentNullException("endpointName");
//            }

//            ChannelFactory<T> channelFactory = null;

//            if (channelFactories.ContainsKey(endpointName))
//            {
//                channelFactory = channelFactories[endpointName] as ChannelFactory<T>;
//            }

//            if (channelFactory == null)
//            {
//                channelFactory = new ChannelFactory<T>(endpointName);

//                channelFactories[endpointName] = channelFactory;
//            }

//            return channelFactory.CreateChannel();
//        }

//        public T GetServiceProxy<T>() where T : class
//        {
//            ServiceMetaData serviceMetaData = null;
//            if (this.interfaceServiceDictionary.ContainsKey(typeof(T)))
//            {
//                serviceMetaData = this.interfaceServiceDictionary[typeof(T)];
//            }
//            else if (this.implementServiceDictionary.ContainsKey(typeof(T)))
//            {
//                serviceMetaData = this.implementServiceDictionary[typeof(T)];
//            }

//            if (serviceMetaData == null)
//                throw new NotFoundException(string.Format("没有找到指定类型:{0}的服务", typeof(T).Name));

//            if (NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode.ToLower() == "local")
//                return ObjectContainer.CreateInstance<T>();

//            System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);

//            //var endPointAddress = string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), this.GetWCFServiceHostName(), (serviceMetaData.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : serviceMetaData.Port), serviceMetaData.ServiceContract);

//            //2014-09-13---新更新内容

//            IDefaultServerProvider tempServerProvider = ObjectContainer.CreateInstance<IDefaultServerProvider>();
//            var endPointAddress = tempServerProvider.GetServerUri<T>(serviceMetaData);

//            ChannelFactory<T> channelFactory = null;

//            if (channelFactories.ContainsKey(typeof(T).FullName))
//            {
//                channelFactory = channelFactories[typeof(T).FullName] as ChannelFactory<T>;
//            }

//            if (channelFactory == null)
//            {
//                if (tempBinding.Name.ToLower() == "webhttpbinding")
//                    channelFactory = new System.ServiceModel.Web.WebChannelFactory<T>(new Uri(endPointAddress));
//                else
//                    channelFactory = new ChannelFactory<T>(tempBinding, endPointAddress);

//                channelFactories[typeof(T).FullName] = channelFactory;
//            }

//            #region 客户端通过messageHeader的方式认证

//            channelFactory.Endpoint.Behaviors.Add(new AttachUserNamePasswordBehavior());

//            #endregion

//            //client.ClientCredentials.ServiceCertificate
//            var tempChannel = channelFactory.CreateChannel();

//            IContextChannel tempContextChannel = tempChannel as IContextChannel;
//            tempContextChannel.OperationTimeout = new TimeSpan(0, 3, 0);

//            if (!clientContextChannelDictionary.ContainsKey(typeof(T)))
//                clientContextChannelDictionary.Add(typeof(T), tempContextChannel);
//            else
//                clientContextChannelDictionary[typeof(T)] = tempContextChannel;

//            return tempChannel;
//        }

//        private string GetWCFServiceHostName()
//        {
//            if (string.IsNullOrEmpty(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName))
//                return ServicePublishHelper.PublishHelper.GetHostName();
//            else
//                return NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName;
//        }

//        #endregion

//        #region 服务注册到容器中

//        public void RegisterService(string filePath)
//        {
//            var serviceMetaDatas = this.serviceDiscovery.GetServiceMetaDatas(filePath);

//            if (serviceMetaDatas == null || serviceMetaDatas.Count == 0)
//                return;

//            foreach (var serviceMetaData in serviceMetaDatas)
//            {
//                if (!this.interfaceServiceDictionary.ContainsKey(serviceMetaData.ServiceContract))
//                    this.interfaceServiceDictionary.Add(serviceMetaData.ServiceContract, serviceMetaData);

//                if (!this.implementServiceDictionary.ContainsKey(serviceMetaData.ImplementType))
//                    this.implementServiceDictionary.Add(serviceMetaData.ImplementType, serviceMetaData);
//            }
//        }

//        public void RegisterService(string dirctoryPath, params string[] filter)
//        {
//            var serviceMetaDatas = this.serviceDiscovery.GetServiceMetaDatas(dirctoryPath, NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.AssemblyFilter, filter);

//            if (serviceMetaDatas == null || serviceMetaDatas.Count == 0)
//                return;

//            foreach (var serviceMetaData in serviceMetaDatas)
//            {
//                if (!this.interfaceServiceDictionary.ContainsKey(serviceMetaData.ServiceContract))
//                    this.interfaceServiceDictionary.Add(serviceMetaData.ServiceContract, serviceMetaData);

//                if (!this.implementServiceDictionary.ContainsKey(serviceMetaData.ImplementType))
//                    this.implementServiceDictionary.Add(serviceMetaData.ImplementType, serviceMetaData);
//            }
//        }

//        #endregion

//        /// <summary>
//        /// 根据服务名称-获取服务的类型
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns></returns>
//        public Type GetServiceType(string serviceName)
//        {
//            if (string.IsNullOrEmpty(serviceName))
//                return null;

//            var tempMetaData = this.implementServiceDictionary.Values.Where(pre => pre.ImplementType.FullName == serviceName).FirstOrDefault();

//            return tempMetaData == null ? null : tempMetaData.ServiceContract;
//        }

//        public T GetServiceProxy<T>(bool flag) where T : class
//        {
//            ServiceMetaData serviceMetaData = null;
//            if (this.interfaceServiceDictionary.ContainsKey(typeof(T)))
//            {
//                serviceMetaData = this.interfaceServiceDictionary[typeof(T)];
//            }
//            else if (this.implementServiceDictionary.ContainsKey(typeof(T)))
//            {
//                serviceMetaData = this.implementServiceDictionary[typeof(T)];
//            }

//            if (serviceMetaData == null)
//                throw new NotFoundException(string.Format("没有找到指定类型:{0}的服务", typeof(T).Name));

//            if (NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode.ToLower() == "local" && !flag)
//                return ObjectContainer.CreateInstance<T>();

//            System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);

//            //var endPointAddress = string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), this.GetWCFServiceHostName(), (serviceMetaData.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : serviceMetaData.Port), serviceMetaData.ServiceContract);

//            //2014-09-13---新更新内容

//            IDefaultServerProvider tempServerProvider = ObjectContainer.CreateInstance<IDefaultServerProvider>();
//            var endPointAddress = tempServerProvider.GetServerUri<T>(serviceMetaData);

//            ChannelFactory<T> channelFactory = null;

//            if (channelFactories.ContainsKey(typeof(T).FullName))
//            {
//                channelFactory = channelFactories[typeof(T).FullName] as ChannelFactory<T>;
//            }

//            if (channelFactory == null)
//            {
//                if (tempBinding.Name.ToLower() == "webhttpbinding")
//                    channelFactory = new System.ServiceModel.Web.WebChannelFactory<T>(new Uri(endPointAddress));
//                else
//                    channelFactory = new ChannelFactory<T>(tempBinding, endPointAddress);

//                channelFactories[typeof(T).FullName] = channelFactory;
//            }

//            var tempChannel = channelFactory.CreateChannel();
//            IContextChannel tempContextChannel = tempChannel as IContextChannel;
//            tempContextChannel.OperationTimeout = new TimeSpan(0, 3, 0);

//            if (!clientContextChannelDictionary.ContainsKey(typeof(T)))
//                clientContextChannelDictionary.Add(typeof(T), tempContextChannel);
//            else
//                clientContextChannelDictionary[typeof(T)] = tempContextChannel;

//            return tempChannel;
//        }

//        public void ReleaseServiceInstance()
//        {
//            ObjectContainer._logProvider.Warn("平台关闭  ReleaseServiceInstance");
//            if (channelFactories.Count == 0)
//                return;

//            foreach (var itemFactory in channelFactories)
//            {
//                if (itemFactory.Value != null && itemFactory.Value is System.ServiceModel.Channels.IChannelFactory)
//                {
//                    if (((System.ServiceModel.Channels.IChannelFactory)itemFactory.Value).State != CommunicationState.Faulted)
//                        ((System.ServiceModel.Channels.IChannelFactory)itemFactory.Value).Close();
//                    else
//                        ((System.ServiceModel.Channels.IChannelFactory)itemFactory.Value).Abort();
//                }
//            }

//            channelFactories.ClearAll();
//            channelFactories = null;

//            if (clientContextChannelDictionary.Count == 0)
//                return;

//            foreach (var item in clientContextChannelDictionary)
//            {
//                if (item.Value == null)
//                    continue;

//                if (item.Value.State != CommunicationState.Faulted)
//                {
//                    item.Value.Close();
//                }
//                else
//                {
//                    item.Value.Abort();
//                }
//            }

//            clientContextChannelDictionary.ClearAll();
//            clientContextChannelDictionary = null;
//        }
//    }
//}
