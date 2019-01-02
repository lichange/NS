//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using RestSharp;
//using RestSharp.Authenticators;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// WCF服务代理
//    /// </summary>
//    public interface IServiceProxyContainer
//    {
//        /// <summary>
//        /// 注册服务到容器中
//        /// </summary>
//        /// <param name="filePath">程序集路径</param>
//        void RegisterService(string filePath);
//        /// <summary>
//        /// 注册服务到容器中
//        /// </summary>
//        /// <param name="dirctoryPath">文件夹路径</param>
//        /// <param name="filter">过滤参数-文件名</param>
//        void RegisterService(string dirctoryPath, params string[] filter);
//        T GetServiceProxy<T>() where T : class;
//        T GetServiceProxy<T>(bool flag) where T : class;
//        /// <summary>
//        /// 根据服务名称-获取服务的类型
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns></returns>
//        Type GetServiceType(string serviceName);

//        /// <summary>
//        /// 释放客户端创建的远程服务代理信息;
//        /// </summary>
//        void ReleaseServiceInstance();
//    }

//    /// <summary>
//    /// WCF服务代理容器
//    /// </summary>
//    public class ServiceContainer
//    {
//        private static readonly IServiceProxyContainer serviceContainer = new Implement.ServiceProxyContainer();

//        #region IServiceProxyContainer 成员

//        /// <summary>
//        /// 注册服务到容器中
//        /// </summary>
//        /// <param name="filePath">程序集路径</param>
//        public static void RegisterService(string filePath)
//        {
//            serviceContainer.RegisterService(filePath);
//        }

//        /// <summary>
//        /// 注册服务到容器中
//        /// </summary>
//        /// <param name="dirctoryPath">文件夹路径</param>
//        /// <param name="filter">过滤参数-文件名</param>
//        public static void RegisterService(string dirctoryPath, params string[] filter)
//        {
//            serviceContainer.RegisterService(dirctoryPath, filter);
//        }

//        /// <summary>
//        /// 根据服务接口类型获取服务代理
//        /// </summary>
//        /// <typeparam name="T">服务接口类型</typeparam>
//        /// <returns>返回服务代理</returns>
//        public static T GetServiceProxy<T>() where T : class
//        {
//            return serviceContainer.GetServiceProxy<T>();
//        }

//        /// <summary>
//        /// 根据服务接口类型获取服务代理
//        /// </summary>
//        /// <typeparam name="T">服务接口类型</typeparam>
//        /// <returns>返回服务代理</returns>
//        public static T GetServiceProxy<T>(bool flag) where T : class
//        {
//            return serviceContainer.GetServiceProxy<T>(flag);
//        }

//        /// <summary>
//        /// 根据服务名称-获取服务的类型
//        /// </summary>
//        /// <param name="serviceName">服务名称</param>
//        /// <returns></returns>
//        public static Type GetServiceType(string serviceName)
//        {
//            return serviceContainer.GetServiceType(serviceName);
//        }

//        public static void ReleaseServiceInstance()
//        {

//            serviceContainer.ReleaseServiceInstance();
//        }

//        #endregion

//        #region RestfulProxy

//        public static RestfulResult GetRestful<T>(RestfulContext context) where T : class, new()
//        {
//            var tempResult = new RestfulResult();
//            var client = new RestClient(context.Server);
//            client.Authenticator = new HttpBasicAuthenticator(context.UserName, context.Password);

//            var request = new RestRequest(context.RestfulAddress, context.Method);
//            var response = client.Execute<T>(request);

//            tempResult.Data = response.Data;
//            tempResult.ErrorMessage = response.ErrorMessage;
//            tempResult.Exception = response.ErrorException;
//            tempResult.Details = response;
//            return tempResult;
//        }

//        #endregion
//    }

//    [Serializable]
//    public class RestfulResult
//    {
//        public object Data
//        {
//            get;
//            set;
//        }

//        public string ErrorMessage
//        {
//            get;
//            set;
//        }

//        public Exception Exception
//        {
//            get;
//            set;
//        }

//        public IRestResponse Details
//        {
//            get;
//            set;
//        }
//    }

//    [Serializable]
//    public class RestfulContext
//    {
//        public string Server
//        {
//            get;
//            set;
//        }

//        public string RestfulAddress
//        {
//            get;
//            set;
//        }

//        public string UserName
//        {
//            get;
//            set;
//        }

//        public string Password
//        {
//            get;
//            set;
//        }

//        public string Token
//        {
//            get;
//            set;
//        }

//        public Method Method
//        {
//            get;
//            set;
//        }

//        public object Parameters
//        {
//            get;
//            set;
//        }
//    }

//}
