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
//    using System.Diagnostics;
//    using System.Linq;
//    using System.ServiceModel;
//    using System.ServiceModel.Dispatcher;

//    /// <summary>
//    /// WCF服务调用器
//    /// </summary>
//    internal class OperationInvoker : IOperationInvoker
//    {
//        IOperationInvoker invoker;

//        /// <summary>
//        /// 计时器
//        /// </summary>
//        Stopwatch sw;

//        public OperationInvoker(IOperationInvoker oldInvoker)
//        {
//            invoker = oldInvoker;
//        }

//        /// <summary>
//        /// 方法调用的参数
//        /// </summary>
//        /// <returns></returns>
//        public virtual object[] AllocateInputs()
//        {
//            return invoker.AllocateInputs();
//        }

//        /// <summary>
//        /// 方法执行完成后事件处理
//        /// </summary>
//        /// <param name="inputs"></param>
//        /// <param name="returnedValue"></param>
//        /// <param name="outputs"></param>
//        /// <param name="exception"></param>
//        protected void PostInvoke(object[] inputs, object returnedValue, object[] outputs, Exception exception)
//        {
//            try
//            {
//#if DEBUG
//                if (exception == null)
//                    Console.WriteLine("End invoke：" + OperationContext.Current.GetCurrentOperationDescription().SyncMethod.Name);
//                Console.WriteLine("ElapsedMilliseconds：" + sw.ElapsedMilliseconds);
//#endif
//                var results = new List<string>();
//                results.Add(returnedValue.ToString());
//                results.AddRange(outputs.Select(o => o.ToString()).ToList());

//                //var logSetting = WcfSettingManager.CurrentServerSetting(OperationContext.Current.GetCurrentServiceDescription().ServiceType).WcfLogSetting;
//                //if (logSetting.Enabled && logSetting.InvokeInfoSetting.Enabled)
//                //{
//                //    var log = WcfLogProvider.GetServerInvokeInfo(
//                //    "OperationInvoker.PostInvoke",
//                //    sw.ElapsedMilliseconds,
//                //    exception == null ? true : false,
//                //    OperationContext.Current.GetCurrentOperationDescription().SyncMethod.Name,
//                //    ServerApplicationContext.Current,
//                //    inputs.Select(i => i.ToString()).ToList(), results);
//                //    WcfServiceLocator.GetLogService().LogWithoutException(log);
//                //}
//            }
//            catch (Exception ex)
//            {
//                //LocalLogService.Log(ex.ToString());
//            }
//        }

//        /// <summary>
//        /// 方法调用
//        /// </summary>
//        /// <param name="instance"></param>
//        /// <param name="inputs"></param>
//        /// <param name="outputs"></param>
//        /// <returns></returns>
//        public object Invoke(object instance, object[] inputs, out object[] outputs)
//        {
//            object returnedValue = null;
//            Exception exception = null;

//            try
//            {
//                sw = Stopwatch.StartNew();
//                returnedValue = invoker.Invoke(instance, inputs, out outputs);
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                throw;
//            }

//            PostInvoke(inputs, returnedValue, outputs, exception);

//            return returnedValue;
//        }

//        /// <summary>
//        /// 异步调用-开始调用
//        /// </summary>
//        /// <param name="instance"></param>
//        /// <param name="inputs"></param>
//        /// <param name="callback"></param>
//        /// <param name="state"></param>
//        /// <returns></returns>
//        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
//        {
//            sw = Stopwatch.StartNew();
//            return invoker.InvokeBegin(instance, inputs, callback, inputs);
//        }

//        /// <summary>
//        /// 异步调用-调用完成
//        /// </summary>
//        /// <param name="instance"></param>
//        /// <param name="outputs"></param>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
//        {
//            object returnedValue = null;
//            Exception exception = null;

//            try
//            {
//                returnedValue = invoker.InvokeEnd(instance, out outputs, result);
//            }
//            catch (Exception ex)
//            {
//                exception = ex;
//                throw;
//            }

//            PostInvoke(result.AsyncState as object[], returnedValue, outputs, exception);

//            return returnedValue;
//        }

//        /// <summary>
//        /// 是否同步调用
//        /// </summary>
//        public bool IsSynchronous
//        {
//            get
//            {
//                return invoker.IsSynchronous;
//            }
//        }
//    }
//}
