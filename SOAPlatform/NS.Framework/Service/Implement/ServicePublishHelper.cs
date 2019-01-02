//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.ServiceModel.Activation;
//using System.ServiceModel.Web;
//using System.Text;
//using System.ServiceModel;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// 服务动态发布辅助类
//    /// </summary>
//    internal class ServicePublishHelper
//    {
//        private string hostName = "localhost";

//        #region 单例

//        private static readonly object flag = new object();
//        private static ServicePublishHelper iPublishHelper;

//        public static ServicePublishHelper PublishHelper
//        {
//            get
//            {
//                if (iPublishHelper == null)
//                {
//                    lock (flag)
//                    {
//                        if (iPublishHelper == null)
//                        {
//                            iPublishHelper = new ServicePublishHelper();
//                        }
//                    }
//                }
//                return iPublishHelper;
//            }
//        }

//        #endregion

//        #region 根据绑定协议返回具体的地址绑定字符串格式

//        public string GetServiceUrlFormat(System.ServiceModel.Channels.Binding binding)
//        {
//            string returnFormatValue = string.Empty;

//            if (binding == null)
//            {
//                returnFormatValue = "http://{0}:{1}/{2}";
//                return returnFormatValue;
//            }

//            switch (binding.Name.ToLower())
//            {
//                case "webhttpbinding":
//                    returnFormatValue = "http://{0}:{1}/{2}";
//                    break;
//                case "wshttpbinding":
//                    returnFormatValue = "http://{0}:{1}/{2}";
//                    break;
//                case "basichttpbinding":
//                    returnFormatValue = "http://{0}:{1}/{2}";
//                    break;
//                case "nettcpbinding":
//                    returnFormatValue = "net.tcp://{0}:{1}/{2}";
//                    break;
//                case "netmsmqbinding":
//                    returnFormatValue = "net.msmq://{0}:{1}/{2}";
//                    break;
//                case "netnamedpipebinding":
//                    returnFormatValue = "net.pipe://{0}/{1}";
//                    break;
//                case "netpeertcpbinding":
//                    returnFormatValue = "net.p2p://{0}:{1}/{2}";
//                    break;
//            }

//            return returnFormatValue;
//        }

//        #endregion

//        public int RandomPort()
//        {
//            int start = 4000, end = 10000;

//            Random random = new Random();
//            int tempRandom = random.Next(start, end);

//            return tempRandom;
//        }

//        #region 获取Host的主机名 -后续可扩展为从配置文件中读取

//        public string SetHostName(string tempHostName)
//        {
//            this.hostName = tempHostName;

//            return this.hostName;
//        }

//        public string GetHostName()
//        {
//            return this.hostName;
//        }

//        #endregion

//        #region 获取动态发布服务的绑定协议
//        public System.ServiceModel.Channels.Binding GetBinding(System.ServiceModel.Channels.Binding binding)
//        {
//            if (binding == null)
//            {
//                return new BasicHttpBinding();
//            }

//            #region 启用WCF认证

//            #endregion

//            return binding;
//        }

//        public System.ServiceModel.Channels.Binding GetBinding(string binding)
//        {
//            System.ServiceModel.Channels.Binding tempBinding = null;

//            if (string.IsNullOrEmpty(binding))
//            {
//                return new BasicHttpBinding();
//            }

//            switch (binding.ToLower())
//            {
//                case "basichttpbinding":
//                    var newBasicHttpBinding = new BasicHttpBinding();
//                    newBasicHttpBinding.MaxReceivedMessageSize = Int32.MaxValue;
//                    newBasicHttpBinding.MaxBufferPoolSize = Int32.MaxValue;
//                    newBasicHttpBinding.MaxBufferSize = Int32.MaxValue;
//                    newBasicHttpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
//                    newBasicHttpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
//                    newBasicHttpBinding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
//                    tempBinding = newBasicHttpBinding;
//                    break;
//                case "wshttpbinding":
//                    var newWSHttpBinding = new WSHttpBinding();

//                    newWSHttpBinding.MaxReceivedMessageSize = Int32.MaxValue;
//                    newWSHttpBinding.MaxBufferPoolSize = Int32.MaxValue;

//                    newWSHttpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
//                    newWSHttpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
//                    newWSHttpBinding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
//                    newWSHttpBinding.Security.Mode = SecurityMode.None;
//                    tempBinding = newWSHttpBinding;
//                    break;
//                case "webhttpbinding":
//                    var newWebHttpBinding = new WebHttpBinding();

//                    newWebHttpBinding.MaxReceivedMessageSize = Int32.MaxValue;
//                    newWebHttpBinding.MaxBufferPoolSize = Int32.MaxValue;
//                    newWebHttpBinding.MaxBufferSize = Int32.MaxValue;
//                    newWebHttpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
//                    newWebHttpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
//                    newWebHttpBinding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
//                    newWebHttpBinding.Security.Mode = WebHttpSecurityMode.None;
//                    newWebHttpBinding.WriteEncoding = System.Text.Encoding.UTF8;

//                    tempBinding = newWebHttpBinding;
//                    break;
//                case "nettcpbinding":
//                    var newNetTcpBinding = new NetTcpBinding();

//                    newNetTcpBinding.MaxReceivedMessageSize = Int32.MaxValue;
//                    newNetTcpBinding.MaxBufferPoolSize = Int32.MaxValue;
//                    newNetTcpBinding.MaxBufferSize = Int32.MaxValue;
//                    newNetTcpBinding.ReaderQuotas.MaxStringContentLength = Int32.MaxValue;
//                    newNetTcpBinding.ReaderQuotas.MaxArrayLength = Int32.MaxValue;
//                    newNetTcpBinding.ReaderQuotas.MaxBytesPerRead = Int32.MaxValue;
//                    newNetTcpBinding.Security.Mode = SecurityMode.None;
//                    tempBinding = newNetTcpBinding;
//                    break;
//                case "netmsmqbinding":
//                    var newNetMsmqBinding = new NetMsmqBinding();
//                    newNetMsmqBinding.Security.Mode = NetMsmqSecurityMode.None;
//                    tempBinding = newNetMsmqBinding;
//                    break;
//                case "netnamedpipebinding":
//                    tempBinding = new NetNamedPipeBinding();
//                    break;
//                case "netpeertcpbinding":
//                    tempBinding = new NetPeerTcpBinding();
//                    break;
//            }

//            //return ServiceSecurityHelper.SecurityHelper.SetX509SecurityMode(tempBinding);

//            return tempBinding;
//        }
//        #endregion

//        #region 动态创建ServiceHost
//        public System.ServiceModel.ServiceHost CreateServiceHost(Type implementType)
//        {
//            lock (flag)
//            {
//                ServiceHost serviceHost = new ServiceHost(implementType);
//                return serviceHost;
//            }
//        }

//        public System.ServiceModel.ServiceHost CreateServiceHost(ServiceMetaData metaData)
//        {
//            lock (flag)
//            {
//                ServiceHost serviceHost = new ServiceHost(metaData.ImplementType);

//                return serviceHost;
//            }
//        }

//        public System.ServiceModel.ServiceHost CreateServiceHost(Type implementType, params Uri[] uris)
//        {
//            lock (flag)
//            {
//                ServiceHost serviceHost = new ServiceHost(implementType, uris);

//                return serviceHost;
//            }
//        }

//        #region Ajax-js-Wcf
//        public System.ServiceModel.ServiceHost CreateWebScriptServiceHost(Type implementType)
//        {
//            lock (flag)
//            {
//                WebScriptServiceHostFactory factory = new WebScriptServiceHostFactory();
//                return (System.ServiceModel.ServiceHost)factory.CreateServiceHost(implementType.FullName, null);
//            }
//        }

//        public System.ServiceModel.ServiceHost CreateWebScriptServiceHost(ServiceMetaData metaData)
//        {
//            lock (flag)
//            {
//                WebScriptServiceHostFactory factory = new WebScriptServiceHostFactory();
//                return (System.ServiceModel.ServiceHost)factory.CreateServiceHost(metaData.ImplementType.FullName, null);
//            }
//        }

//        public System.ServiceModel.ServiceHost CreateWebScriptServiceHost(Type implementType, params Uri[] uris)
//        {
//            lock (flag)
//            {
//                WebScriptServiceHostFactory factory = new WebScriptServiceHostFactory();
//                return (System.ServiceModel.ServiceHost)factory.CreateServiceHost(implementType.FullName, uris);
//            }
//        }

//        #endregion

//        #region Rest
//        public WebServiceHost CreateWebServiceHost(Type implementType)
//        {
//            lock (flag)
//            {
//                WebServiceHost serviceHost = new WebServiceHost(implementType);

//                return serviceHost;
//            }
//        }

//        public WebServiceHost CreateWebServiceHost(ServiceMetaData metaData)
//        {
//            lock (flag)
//            {
//                WebServiceHost serviceHost = new WebServiceHost(metaData.ImplementType);

//                return serviceHost;
//            }
//        }

//        public WebServiceHost CreateWebServiceHost(Type implementType, params Uri[] uris)
//        {
//            lock (flag)
//            {
//                WebServiceHost serviceHost = new WebServiceHost(implementType, uris);
//                return serviceHost;
//            }
//        }
//        #endregion

//        #endregion
//    }
//}
