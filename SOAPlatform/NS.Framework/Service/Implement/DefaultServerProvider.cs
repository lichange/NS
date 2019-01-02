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
//using NS.Framework.Attributes;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// 服务发布器-动态发布WCF服务
//    /// </summary>
//    [Export(typeof(IDefaultServerProvider))]
//    public class DefaultServerProvider : IDefaultServerProvider
//    {
//        public string GetServerUri<T>(ServiceMetaData serviceMetaData) where T : class
//        {
//            var keyValueItem = NS.Framework.Config.PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key == typeof(T).FullName).FirstOrDefault();
//            if (keyValueItem == null)
//                return this.GetDefaultServerUrl(serviceMetaData);
//            else
//            return this.GetServerUrl<T>(keyValueItem,serviceMetaData);
//        }

//        private string GetServerUrl<T>(Config.KeyValueItem keyValueItem, ServiceMetaData serviceMetaData)
//        {
//            System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);

//            var endPointAddress = string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), this.GetNewWCFServiceHostName(keyValueItem.Value), (serviceMetaData.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : serviceMetaData.Port), serviceMetaData.ServiceContract);

//            return endPointAddress;
//        }

//        private string GetDefaultServerUrl(ServiceMetaData serviceMetaData)
//        {
//            System.ServiceModel.Channels.Binding tempBinding = ServicePublishHelper.PublishHelper.GetBinding(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Binding);

//            var endPointAddress = string.Format(ServicePublishHelper.PublishHelper.GetServiceUrlFormat(tempBinding), this.GetWCFServiceHostName(), (serviceMetaData.Port == null ? NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.Port.ToString() : serviceMetaData.Port), serviceMetaData.ServiceContract);

//            return endPointAddress;
//        }

//        private string GetNewWCFServiceHostName(string key)
//        {
//            if (string.IsNullOrEmpty(key))
//                return NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName;
//            else
//                return key;
//        }

//        private string GetWCFServiceHostName()
//        {
//            if (string.IsNullOrEmpty(NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName))
//                return ServicePublishHelper.PublishHelper.GetHostName();
//            else
//                return NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceName;
//        }
//    }
//}
