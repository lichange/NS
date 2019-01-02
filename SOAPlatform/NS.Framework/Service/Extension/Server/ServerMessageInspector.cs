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
//    using System.Collections.Generic;
//    using System.ServiceModel;
//    using System.ServiceModel.Channels;
//    using System.ServiceModel.Dispatcher;

//    /// <summary>
//    /// 服务器端消息拦截器-负责拦截WCF收到客户端请求前后的附加处理
//    /// </summary>
//    internal class ServerMessageInspector : IDispatchMessageInspector
//    {
//        private static Dictionary<string, string> serviceVersionCache = new Dictionary<string, string>();
//        private static object locker = new object();

//        /// <summary>
//        /// 接收到客户端请求-执行方法调用之前
//        /// </summary>
//        /// <param name="request"></param>
//        /// <param name="channel"></param>
//        /// <param name="instanceContext"></param>
//        /// <returns></returns>
//        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
//        {
//#if DEBUG
//            var message = request.ToString();
//            Console.WriteLine("Server got message：" + message);
//#endif
//            ServerApplicationContext.Current = request.GetApplicationContext<ServerApplicationContext>();
          
//            try
//            {
//                //记录日志
//                //var setting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType);
//                //var logSetting = setting.WcfLogSetting;
//                //if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
//                //{
//                //    var direct = logSetting.MessageInfoSetting.MessageDirection;
//                //    if (direct == MessageDirection.Both || direct == MessageDirection.Receive)
//                //    {
//                //        var log = WcfLogProvider.GetServerMessageInfo(
//                //            "ServerMessageInspector.AfterReceiveRequest",
//                //            MessageDirection.Receive,
//                //            request.ToString());
//                //        WcfServiceLocator.GetLogService().LogWithoutException(log);
//                //    }
//                //}

//                //调用服务的身份验证
//                //var securitySetting = setting.WcfSecuritySetting;
//                //if (securitySetting.PasswordCheck.Enable)
//                //{
//                //    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Request)
//                //    {
//                //        if (ServerApplicationContext.Current.Password != securitySetting.PasswordCheck.Password)
//                //            throw new WcfSecurityException("PasswordCheck failed in request!");
//                //    }
//                //}
//            }
//            catch (Exception ex)
//            {
//                //LocalLogService.Log(ex.ToString());
//                if (ex is WcfSecurityException) throw;
//            }
//            return null;
//        }

//        /// <summary>
//        /// 发送服务处理的结果之前做的处理
//        /// </summary>
//        /// <param name="reply"></param>
//        /// <param name="correlationState"></param>
//        public void BeforeSendReply(ref Message reply, object correlationState)
//        {
//            try
//            {
//                var clientContext = new ClientApplicationContext();
//                //clientContext.ServerMachineName = WcfLogProvider.MachineName;
//                clientContext.RequestIdentity = ServerApplicationContext.Current.RequestIdentity;
//                clientContext.ServerExceptionID = ServerApplicationContext.Current.ServerExceptionID ?? "";

//                var serviceType = OperationContext.Current.GetCurrentServiceDescription().ServiceType;
//                if (!serviceVersionCache.ContainsKey(serviceType.FullName))
//                {
//                    lock (locker)
//                    {
//                        if (!serviceVersionCache.ContainsKey(serviceType.FullName))
//                        {
//                            serviceVersionCache.Add(serviceType.FullName, serviceType.Assembly.GetName().Version.ToString());
//                        }
//                    }
//                }
//                clientContext.ServerVersion = serviceVersionCache[serviceType.FullName];

//                reply.SetApplicationContext(clientContext);

//#if DEBUG
//                var message = reply.ToString();
//                Console.WriteLine("Server send message：" + message);
//#endif

//                //var setting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType);

//                //var securitySetting = setting.WcfSecuritySetting;
//                //if (securitySetting.PasswordCheck.Enable)
//                //{
//                //    if (securitySetting.PasswordCheck.Direction == OperationDirection.Both || securitySetting.PasswordCheck.Direction == OperationDirection.Reply)
//                //    {
//                //        clientContext.Password = securitySetting.PasswordCheck.Password;
//                //    }
//                //}

//            //    var logSetting = setting.WcfLogSetting;
//            //    if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
//            //    {
//            //        var direct = logSetting.MessageInfoSetting.MessageDirection;
//            //        if (direct == MessageDirection.Both ||
//            //            direct == MessageDirection.Send)
//            //        {
//            //            var log = WcfLogProvider.GetServerMessageInfo(
//            //                "ServerMessageInspector.BeforeSendReply",
//            //                MessageDirection.Send,
//            //                reply.ToString());
//            //            WcfServiceLocator.GetLogService().LogWithoutException(log);
//            //        }
//            //    }

//            }
//            catch (Exception ex)
//            {
//                //LocalLogService.Log(ex.ToString());
//            }
//        }
//    }
//}
