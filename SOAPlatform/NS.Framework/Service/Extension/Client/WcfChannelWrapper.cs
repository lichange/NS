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
//    /// 通信通道管理器
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    internal sealed class WcfChannelWrapper<T> : IDisposable where T : class
//    {
//        private readonly T channel;

//        public WcfChannelWrapper(T channel)
//        {
//            this.channel = channel;
//        }

//        public T Channel { get { return channel; } }

//        #region IDisposable Members

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        private bool disposed;

//        private void Dispose(bool disposing)
//        {
//            if (disposed) return;
//            if (disposing)
//            {
//                var commObj = channel as ICommunicationObject;
//                if (commObj != null)
//                {
//                    try
//                    {
//                        commObj.Close();
//                    }
//                    catch (CommunicationException)
//                    {
//                        commObj.Abort();
//                    }
//                    catch (TimeoutException)
//                    {
//                        commObj.Abort();
//                    }
//                    catch (Exception)
//                    {
//                        commObj.Abort();
//                        throw;
//                    }
//                }

//            }

//            disposed = true;
//        }

//        ~WcfChannelWrapper()
//        {
//            Dispose(false);
//        }

//        #endregion

//    }
//}
