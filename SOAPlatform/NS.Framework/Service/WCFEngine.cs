//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Practices.Unity;
//using System.Threading;

//using NS.Framework.Service.Implement;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// 平台引擎,负责提供线程安全的平台启动,关闭和重启服务,但不允许跨应用程序域访问
//    /// 平台为应用提供的接口中,部分会出现跨应用程序域访问的要求,那时需要做好应用程序域安全
//    /// </summary>
//    public class WCFEngine : IDisposable
//    {
//        #region 平台单一实例

//        /// <summary>
//        /// 静态单例
//        /// </summary>
//        private static WCFEngine instance;

//        /// <summary>
//        /// 线程安全的锁
//        /// </summary>
//        public static Object syncLock = new Object();


//        /// <summary>
//        /// 平台引擎单一实例
//        /// </summary>
//        public static WCFEngine Instance
//        {
//            get
//            {
//                lock (syncLock)
//                {
//                    if (instance == null || instance.IsDisposed) instance = new WCFEngine();
//                    return instance;
//                }
//            }
//        }

//        #endregion

//        #region IDisposable 成员

//        public void Dispose()
//        {
//            lock (syncLock)
//            {
//                Dispose(true);
//                GC.SuppressFinalize(this);
//            }
//        }

//        /// <summary>
//        /// 标志本对象是否被释放
//        /// </summary>
//        private bool disposed = false;

//        /// <summary>
//        /// 标志本对象是否被释放
//        /// </summary>
//        public bool IsDisposed
//        {
//            get
//            {
//                return disposed;
//            }
//        }

//        private void Dispose(bool disposing)
//        {
//            if (!this.disposed)
//            {
//                // 如果是手工释放,则释放托管资源
//                if (disposing)
//                {
//                    // 如果容器不释放所包含的对象,则需手工释放
//                    pfuc.Dispose();                    
//                }
//                // 释放非托管资源
//                { }

//                disposed = true;
//            }
//        }

//        #endregion

//        #region 构造与析构函数

//        private WCFEngine()
//        {
//            pfuc = new UnityContainer();
//            IsEngineStarted = false;
//            platform = new WCFPlatform(pfuc);
//            pfuc.RegisterInstance(typeof(IWCFPlatform), platform);
//        }

//        ~WCFEngine()
//        {
//            Dispose(false);
//        }

//        #endregion

//        #region 私有字段与方法
       
//        /// <summary>
//        /// Unity容器
//        /// </summary>
//        private IUnityContainer pfuc;

//        /// <summary>
//        /// false表示未启动, true表示启动
//        /// </summary>
//        private bool IsEngineStarted;

//        /// <summary>
//        /// 启动平台服务
//        /// </summary>
//        private void StartEngine()
//        {
//            platform.Start();
//        }

//        /// <summary>
//        /// 平台类实例,伴随引擎的单例的生命周期
//        /// </summary>
//        private IWCFPlatform platform;
        
//        /// <summary>
//        /// 停止平台服务
//        /// </summary>
//        private void StopEngine()
//        {
//            platform.Stop();
//        }

//        #endregion

//        #region 公共属性与方法
        
//        /// <summary>
//        /// 确保平台引擎已发动,对Platform的开启和关闭进行线程安全封装,
//        /// 实际上,应用在确保平台启动后,平台还可能随时被关闭,重置,并发出相应通知
//        /// 此时平台会处于拒绝服务状态,IPlatform不再有效,
//        /// 应用可订阅通知或捕获异常,并为用户提供友好的处理
//        /// </summary>
//        /// <returns>返回平台操作接口</returns>
//        public IWCFPlatform MakeSureEngineIsStarted()
//        {           
//            if (IsEngineStarted) return platform;
//            lock (syncLock)
//            {
//                StartEngine();
//                IsEngineStarted = true;
//            }
//            return platform;
//        }

//        /// <summary>
//        /// 停止平台服务,对Platform的开启和关闭进行线程安全封装
//        /// </summary>
//        public void MakeSureEngineIsStopped()
//        {
//            if (!IsEngineStarted) return;
//            lock (syncLock)
//            {
//                StopEngine();
//                IsEngineStarted = false;
//            }
//        }

//        /// <summary>
//        /// 重启平台服务,对Platform的开启和关闭进行线程安全封装
//        /// </summary>
//        public void ResetEngine()
//        {
//            lock (syncLock)
//            {
//                if (IsEngineStarted) StopEngine();
//                StartEngine();
//                IsEngineStarted = true;
//            }
//        }

//        #endregion
//    }
//}
