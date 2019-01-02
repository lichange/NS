//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Practices.Unity;

//using NS.Framework.Utility.Collections;
//using Unity;

//namespace NS.Framework.Service.Implement
//{
//    internal class ServiceManager : IServiceManager
//    {
//        private IUnityContainer _container;
//        //private IServicePublisher servicePublisher;
//        private ThreadSafeDictionary<string, ServiceMetaData> tempServiceDictionary = new ThreadSafeDictionary<string, ServiceMetaData>();
//        private ThreadSafeDictionary<Type, ServiceMetaData> tempServiceTypeDictionary = new ThreadSafeDictionary<Type, ServiceMetaData>();

//        public ServiceManager(IUnityContainer container)
//        {
//            this._container = container;
//        }

//        #region IServiceManager 成员

//        public void InitializationServiceManager()
//        {
//            var serviceDiscovery = _container.Resolve<IServiceDiscovery>();

//            if (serviceDiscovery == null)
//                throw new Exceptions.NotFoundException("未初始化组件:IServiceDiscovery");

//            var serviceDiscoveryPath = string.IsNullOrEmpty(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServerDiscoveryPath)?
//                System.AppDomain.CurrentDomain.BaseDirectory : NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServerDiscoveryPath;

//            var serviceMetaDatas = serviceDiscovery.GetServiceMetaDatas(serviceDiscoveryPath, NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.AssemblyFilter);

//            foreach (var serviceMetaData in serviceMetaDatas)
//            {
//                if (serviceMetaData == null || serviceMetaData.ImplementType == null)
//                    continue;

//                if (!tempServiceDictionary.ContainsKey(serviceMetaData.ImplementType.Name))
//                    tempServiceDictionary.Add(serviceMetaData.ImplementType.Name, serviceMetaData);

//                if (!tempServiceTypeDictionary.ContainsKey(serviceMetaData.ImplementType))
//                    tempServiceTypeDictionary.Add(serviceMetaData.ImplementType, serviceMetaData);
//            }

//            var servicePublisher = _container.Resolve<IServicePublisher>();

//            if (servicePublisher == null)
//                return;

//            servicePublisher.Publish(tempServiceDictionary.Values.ToArray());
//        }

//        public bool StartService(string serviceName)
//        {
//            if (!tempServiceDictionary.ContainsKey(serviceName))
//                return false;

//            try
//            {
//                var servicePublisher = _container.Resolve<IServicePublisher>();

//                if (servicePublisher == null)
//                    return false;

//                return servicePublisher.Publish(tempServiceDictionary[serviceName]);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool StartService(Type implementType)
//        {
//            if (!tempServiceTypeDictionary.ContainsKey(implementType))
//                return false;

//            try
//            {
//                var servicePublisher = _container.Resolve<IServicePublisher>();

//                if (servicePublisher == null)
//                    return false;

//                return servicePublisher.Publish(tempServiceTypeDictionary[implementType]);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool CloseService(string serviceName)
//        {
//            if (!tempServiceDictionary.ContainsKey(serviceName))
//                return false;

//            try
//            {
//                var servicePublisher = _container.Resolve<IServicePublisher>();

//                if (servicePublisher == null)
//                    return false;

//                return servicePublisher.UnPublish(tempServiceDictionary[serviceName]);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool CloseService(Type implementType)
//        {
//            if (!tempServiceTypeDictionary.ContainsKey(implementType))
//                return false;

//            try
//            {
//                var servicePublisher = _container.Resolve<IServicePublisher>();

//                if (servicePublisher == null)
//                    return false;

//                return servicePublisher.UnPublish(tempServiceTypeDictionary[implementType]);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool UpdateService(Type implementType, ServiceMetaData newImplementType)
//        {
//            try
//            {
//                var closeFlag = this.CloseService(implementType);

//                if (closeFlag)
//                {
//                    //移出
//                    if (this.tempServiceDictionary.ContainsKey(implementType.Name))
//                        this.tempServiceDictionary.Remove(implementType.Name);
//                    if (this.tempServiceTypeDictionary.ContainsKey(implementType))
//                        this.tempServiceTypeDictionary.Remove(implementType);

//                    //添加服务即可
//                    var addFlag = this.AddService(newImplementType);

//                    if (addFlag)
//                        this.StartService(newImplementType.ImplementType);
//                }

//                return false;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool UpdateService(string serviceName, ServiceMetaData newServiceName)
//        {
//            try
//            {
//                var closeFlag = this.CloseService(serviceName);

//                if (closeFlag)
//                {
//                    //移出
//                    if (this.tempServiceDictionary.ContainsKey(serviceName))
//                        this.tempServiceDictionary.Remove(serviceName);

//                    var types = this.tempServiceTypeDictionary.Keys.Where(pre => pre.Name == serviceName).ToList();

//                    if (types.Count > 0)
//                    {
//                        if (this.tempServiceTypeDictionary.ContainsKey(types[0]))
//                            this.tempServiceTypeDictionary.Remove(types[0]);
//                    }

//                    //添加服务即可
//                    var addFlag = this.AddService(newServiceName);

//                    if (addFlag)
//                        this.StartService(newServiceName.ImplementType);
//                }

//                return false;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public bool AddService(ServiceMetaData metaData)
//        {
//            if (metaData == null || metaData.ImplementType == null)
//                return false;

//            if (!tempServiceDictionary.ContainsKey(metaData.ImplementType.Name))
//                tempServiceDictionary.Add(metaData.ImplementType.Name, metaData);

//            if (!tempServiceTypeDictionary.ContainsKey(metaData.ImplementType))
//                tempServiceTypeDictionary.Add(metaData.ImplementType, metaData);

//            return true;
//        }

//        public ServiceMetaData GetServiceMetaData(string serviceName)
//        {
//            if (string.IsNullOrEmpty(serviceName))
//                return null;

//            if (this.tempServiceDictionary.ContainsKey(serviceName))
//                return this.tempServiceDictionary[serviceName];

//            return null;
//        }

//        public IList<ServiceMetaData> GetServiceMetaDatas(params string[] serviceNames)
//        {
//            IList<ServiceMetaData> serviceMetaDatas = new List<ServiceMetaData>();

//            if (serviceNames == null || serviceNames.Length == 0)
//                return serviceMetaDatas;

//            foreach (var serviceName in serviceNames)
//            {
//                if (string.IsNullOrEmpty(serviceName))
//                    continue;

//                var tempserviceMetaData = this.GetServiceMetaData(serviceName);

//                if (tempserviceMetaData != null)
//                    serviceMetaDatas.Add(tempserviceMetaData);
//            }

//            return serviceMetaDatas;
//        }

//        public ServiceMetaData GetServiceMetaData(Type implementType)
//        {
//            if (this.tempServiceTypeDictionary.ContainsKey(implementType))
//                return this.tempServiceTypeDictionary[implementType];

//            return null;
//        }

//        public IList<ServiceMetaData> GetServiceMetaDatas(params Type[] serviceTypes)
//        {
//            IList<ServiceMetaData> serviceMetaDatas = new List<ServiceMetaData>();

//            if (serviceTypes == null || serviceTypes.Length == 0)
//                return serviceMetaDatas;

//            foreach (var serviceType in serviceTypes)
//            {
//                if (serviceType == null)
//                    continue;

//                var tempserviceMetaData = this.GetServiceMetaData(serviceType);

//                if (tempserviceMetaData != null)
//                    serviceMetaDatas.Add(tempserviceMetaData);
//            }

//            return serviceMetaDatas;
//        }

//        #endregion

//        #region IServiceManager 成员

//        public IList<ServiceMetaData> GetAllServiceMetaDatas()
//        {
//            return this.tempServiceDictionary.Values.ToList();
//        }

//        #endregion

//        #region 平台启动或关闭时的处理
//        internal void OnStartPlatform(object sender, EventArgs e)
//        {
//            tempServiceDictionary.ClearAll();
//            tempServiceTypeDictionary.ClearAll();

//            //初始化服务管理器
//            InitializationServiceManager();
//        }

//        internal void OnStopPlatform(object sender, EventArgs e)
//        {
//            tempServiceDictionary.ClearAll();
//            tempServiceTypeDictionary.ClearAll();
//        }
//        #endregion
//    }
//}
