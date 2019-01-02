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
//    using System.Diagnostics;
//    using System.Runtime.Remoting.Messaging;
//    using System.Runtime.Remoting.Proxies;
//    using System.ServiceModel;

//    /// <summary>
//    /// WCF服务的真实代理
//    /// </summary>
//    /// <typeparam name="T">服务类型</typeparam>
//    internal class ServiceRealProxy<T> : RealProxy where T : class
//    {
//        public ServiceRealProxy()
//            : base(typeof(T))
//        {
//        }

//        /// <summary>
//        /// 重写真实代理方法
//        /// </summary>
//        /// <param name="msg">消息对象</param>
//        /// <returns></returns>
//        public override IMessage Invoke(IMessage msg)
//        {
//            using (var client = WcfServiceClientFactory.CreateServiceClient<T>())
//            {
//                var channel = client.Channel;
//                IMethodCallMessage methodCall = (IMethodCallMessage)msg;
//                IMethodReturnMessage methodReturn = null;
//                object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
//                methodCall.Args.CopyTo(copiedArgs, 0);

//                bool isSuccessuful = false;

//                //计时器
//                var stopwatch = Stopwatch.StartNew();

//                try
//                {
//#if DEBUG
//                    Console.WriteLine("Begin to invoke：" + methodCall.MethodName);
//#endif
//                    object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);

//#if DEBUG
//                    Console.WriteLine("End invoke:" + methodCall.MethodName);
//#endif
//                    methodReturn = new ReturnMessage(returnValue,
//                                                    copiedArgs,
//                                                    copiedArgs.Length,
//                                                    methodCall.LogicalCallContext,
//                                                    methodCall);
//                    isSuccessuful = true;
//                }
//                catch (Exception ex)
//                {
//                    var exception = ex;
//                    var excepionID = ClientApplicationContext.Current.ServerExceptionID ?? "";
//                    if (ex.InnerException != null)
//                    {
//                        exception = ex.InnerException;
//                        exception.HelpLink = "Please check this exception via ID : " + excepionID;
//                    }

//                    methodReturn = new ReturnMessage(exception, methodCall);
                  
//                    //判定是否开始写日志
//                }
//                finally
//                {
//                    //if (LogSetting.Enabled && LogSetting.InvokeInfoSetting.Enabled)
//                    //{
//                    //    var log = WcfLogProvider.GetClientInvokeLog(
//                    //       typeof(T).FullName,
//                    //       "ServiceRealProxy.Invoke",
//                    //       stopwatch.ElapsedMilliseconds,
//                    //       isSuccessuful,
//                    //       methodCall.MethodName,
//                    //       ClientApplicationContext.Current);
//                    //    WcfServiceLocator.GetLogService().LogWithoutException(log);
//                    //}
//                    //写日志
//                }
//                return methodReturn;
//            }
//        }
//    }
//}
