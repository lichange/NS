///****************************************************************************************************************
//*                                                                                                               *
//* Copyright (C) 2011 5173.com                                                                                   *
//* This project may be copied only under the terms of the Apache License 2.0.                                    *
//* Please visit the project Home Page http://wcfextension.codeplex.com/ for more detail.                         *
//*                                                                                                               *
//****************************************************************************************************************/

//namespace WcfExtension
//{
//    using System.Runtime.Remoting.Messaging;
//    using System.Runtime.Serialization;

//    /// <summary>
//    /// 服务器端调用上下文
//    /// </summary>
//    [CollectionDataContract(Namespace = "WcfExtension", ItemName = "Context")]
//    public class ServerApplicationContext : ApplicationContext
//    {
//        /// <summary>
//        /// 客户端计算机名称
//        /// </summary>
//        public string ClientMachineName
//        {
//            get
//            {
//                return base["ClientMachineName"];
//            }
//            set
//            {
//                base["ClientMachineName"] = value;
//            }
//        }

//        /// <summary>
//        /// 客户端版本
//        /// </summary>
//        public string ClientVersion
//        {
//            get
//            {
//                return base["ClientVersion"];
//            }
//            set
//            {
//                base["ClientVersion"] = value;
//            }
//        }

//        /// <summary>
//        /// 当前上下文实例
//        /// </summary>
//        public static ServerApplicationContext Current
//        {
//            get
//            {
//                return CallContext.GetData(CallContextKey) as ServerApplicationContext;
//            }
//            set
//            {
//                CallContext.SetData(CallContextKey, value);
//            }
//        }
//    }
//}
