///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System;
//    using System.ServiceModel;

//    /// <summary>
//    /// WCF服务发布管理器
//    /// </summary>
//    public class ServerManager
//    {
//        private static ServiceHost host;

//        internal static bool TryRestartService(string typeName)
//        {
//            if (host != null && typeName == host.Description.ServiceType.FullName)
//            {
//                host.Close();
//                StartService(host.Description.ServiceType);
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// 开启服务
//        /// </summary>
//        /// <param name="serviceType"></param>
//        public static void StartService(Type serviceType)
//        {
//            try
//            {
//                host = WcfServiceHostFactory.CreateServiceHost(serviceType);
//                host.Open();
//                //ConfigUpdateManager.InitServer(); //加入心跳检测
//            }
//            catch (Exception ex)
//            {
//                //LocalLogService.Log(ex.ToString());
//                //记录日志
//                throw;
//            }
//        }
//    }
//}
