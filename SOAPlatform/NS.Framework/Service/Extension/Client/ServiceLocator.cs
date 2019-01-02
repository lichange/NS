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
//    using System.Threading;

//    /// <summary>
//    /// WCF服务定位器
//    /// </summary>
//    public class WcfServiceLocator
//    {
//        private static bool enableRemoteLogService = true;
//        private static Timer logServiceCheckTimer;

//        private readonly static TimeSpan NORMAL_CHECK_SPAN = TimeSpan.FromSeconds(1);
//        private readonly static TimeSpan EXCEPTION_CHECK_SPAN = TimeSpan.FromSeconds(10);

//        public static T Create<T>() where T : class
//        {
//            return (T)(new ServiceRealProxy<T>().GetTransparentProxy());
//        }
//    }
//}
