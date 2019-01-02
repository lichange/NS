using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;

using NHibernate;
using NHibernate.Cfg;

namespace NS.Component.Data.Impl
{
    /// <summary>
    /// 负责配置文件与SessionFactory的映射管理
    /// </summary>
    class NHibernateDataAccessorManager
    {
        /// <summary>
        /// 线程安全的字典,保存和每个数据库配置相关的全部数据访问对象
        /// </summary>
        private readonly static Hashtable dict = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 根据配置信息找到或创建工厂,并提供一个会话对象
        /// </summary>
        /// <param name="dac">数据访问配置类</param>
        /// <returns>返回值为一个回话对象</returns>
        public static ISession GetSession(DataAccessorConfiguration dac)
        {
            lock (dict.SyncRoot)
            {
                if (dict.ContainsKey(dac))
                    return (dict[dac] as HBData).Factory.OpenSession();
                else
                {
                    Configuration cfg = new Configuration();
                    foreach (Assembly asse in dac.MappingsAssemblies)
                        cfg.Configure(dac.ConfigFile).AddAssembly(asse);
                    if (dac.NamingStrategy != null)
                        cfg.SetNamingStrategy(dac.NamingStrategy);
                    ISessionFactory factory = cfg.BuildSessionFactory();

                    NS.Component.NHibernate.NHibernateFactory.Initialize(factory);

                    HBData data = new HBData() { Configuration = cfg, Factory = factory };
                    dict.Add(dac, data);
                    return factory.OpenSession();
                }
            }
        }

        /// <summary>
        /// 清理某个数据库连接
        /// </summary>
        /// <param name="dac">传入的参数为构建数据库连接时使用的配置对象</param>
        public static void Clear(DataAccessorConfiguration dac)
        {
            lock (dict.SyncRoot)
            {
                if (dict.ContainsKey(dac))
                {
                    HBData hbdata = dict[dac] as HBData;
                    dict.Remove(dac);
                    hbdata.Factory.Dispose();
                    hbdata = null;
                }
            }
        }

        class HBData
        {
            /// <summary>
            /// NHibernate配置类
            /// </summary>
            public Configuration Configuration { get; set; }
            /// <summary>
            /// NHibernate会话工厂类
            /// </summary>
            public ISessionFactory Factory { get; set; }
        }
    }
}
