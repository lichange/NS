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
//    /// 拦截器-负责WCF服务调用AOP-负责在服务方法执行前后执行的操作
//    /// </summary>
//    internal class ClientMessageInspector : IClientMessageInspector
//    {
//        /// <summary>
//        /// 记录当前客户端调用服务的版本
//        /// </summary>
//        private static Dictionary<string, string> contractVersionCache = new Dictionary<string, string>();
//        private static object locker = new object();

//        public void AfterReceiveReply(ref Message reply, object correlationState)
//        {
//#if DEBUG
//            var message = reply.ToString();
//            Console.WriteLine("Client got message：" + message);
//#endif
//            ClientApplicationContext.Current = reply.GetApplicationContext<ClientApplicationContext>();    

//            try
//            {
//               //日志组件
//            }
//            catch (Exception ex)
//            {
//                //日志组件
//            }
//        }

//        public object BeforeSendRequest(ref Message request, IClientChannel channel)
//        {
//#if DEBUG
//            var message = request.ToString();
//            Console.WriteLine("Client send message：" + message);
//#endif
//            try
//            {
//                var channelType = channel.GetType();

//                var serverContext = new ServerApplicationContext();
//                serverContext.RequestIdentity = Guid.NewGuid().ToString();
//                //serverContext.ClientMachineName = WcfLogProvider.MachineName;

//                if (!contractVersionCache.ContainsKey(channelType.FullName))
//                {
//                    lock (locker)
//                    {
//                        if (!contractVersionCache.ContainsKey(channelType.FullName))
//                        {
//                            contractVersionCache.Add(channelType.FullName, channelType.Assembly.GetName().Version.ToString());
//                        }
//                    }
//                }
//                serverContext.ClientVersion = contractVersionCache[channelType.FullName];
//                request.SetApplicationContext(serverContext);

//                var clientContext = new ClientApplicationContext();
//                clientContext.RequestIdentity = serverContext.RequestIdentity;
//                ClientApplicationContext.Current = clientContext;

//                //var logSetting = WcfLogSettingManager.Current(channelType);
//                //if (logSetting.Enabled && logSetting.MessageInfoSetting.Enabled)
//                //{
//                //    var direct = logSetting.MessageInfoSetting.MessageDirection;
//                //    if (direct == MessageDirection.Both
//                //        || direct == MessageDirection.Send)
//                //    {
//                //        var log = WcfLogProvider.GetClientMessageInfo(
//                //        channelType.FullName,
//                //        ClientApplicationContext.Current.RequestIdentity,
//                //        "ClientMessageInspector.BeforeSendRequest",
//                //        MessageDirection.Send,
//                //        request.ToString());
//                //        WcfServiceLocator.GetLogService().LogWithoutException(log);
//                //    }
//                //}

//                return channelType;
//            }
//            catch (Exception ex)
//            {
//                //LocalLogService.Log(ex.ToString());
//            }
//            return channel.GetType();
//        }
//    }
//}
