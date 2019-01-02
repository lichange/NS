//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Practices.Unity;
//using NS.Framework.Exceptions;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using System.ServiceModel.Description;
//using NS.Framework.Utility.Collections;
//using System.ServiceModel.Channels;
//using System.Reflection;

//using NS.Framework.Service.Implement;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// 服务发现器
//    /// </summary>
//    public class ServiceDiscoveryHelper
//    {
//        #region 单例

//        private static readonly object flag = new object();
//        private static ServiceDiscoveryHelper instance;

//        public static ServiceDiscoveryHelper Instance
//        {
//            get
//            {
//                if (instance == null)
//                {
//                    lock (flag)
//                    {
//                        if (instance == null)
//                        {
//                            instance = new ServiceDiscoveryHelper();
//                        }
//                    }
//                }
//                return instance;
//            }

//            #endregion
//        }

//        public IList<ServiceMetaData> GetServiceMetaDatas(string filePath)
//        {
//            IServiceDiscovery discovery = new ServiceDiscovery();

//            return discovery.GetServiceMetaDatas(filePath);
//        }

//        public IList<ServiceMetaData> GetServiceMetaDatas(string directoryPath, string filter, params string[] ignoredfileNames)
//        {
//            IServiceDiscovery discovery = new ServiceDiscovery();

//            return discovery.GetServiceMetaDatas(directoryPath,filter,ignoredfileNames);
//        }
//    }
//}
