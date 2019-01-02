using NS.Component.Data.Impl;
using NS.Framework.Utility.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

using NHibernate;
using NHibernate.Cfg;
using System.Threading;
//using System.ServiceModel;
using NS.Component.Data;


namespace NS.Component.NHibernate
{
    public static class NHibernateFactory
    {
        private static ISessionFactory _sessionFactory;
        private static Data.DataAccessorConfiguration _config;

        public static void Initialize(DataAccessorConfiguration config) 
        {
            _config = config;
        }

     public static void Initialize()
        {
            DataAccessorConfiguration config = new DataAccessorConfiguration(); // TODO: 初始化为适当的值
            config.MappingsAssemblies = new List<System.Reflection.Assembly>();
            config.MappingsAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
            config.MappingsAssemblies.AddRange(NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.GetMappingAssemblys);
            var configFileName = string.Format("{0}-{1}", "car", NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConfigFile);
            //var configFileName = config.ConfigFile;


            if (NS.Framework.Config.PlatformConfig.ServerConfig.ConfigFilePath.EndsWith("bin"))
                config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", configFileName);
            else
                config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, configFileName);
            _config = config;
        }

        public static void Initialize(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public static ISession OpenSession()
        {
 
            if (_sessionFactory != null)
               return  _sessionFactory.OpenSession();
            else
            {
                Configuration cfg = new Configuration();
                if (_config == null) Initialize();
                foreach (Assembly asse in _config.MappingsAssemblies)
                    cfg.Configure(_config.ConfigFile).AddAssembly(asse);
                if (_config.NamingStrategy != null)
                    cfg.SetNamingStrategy(_config.NamingStrategy);
                ISessionFactory factory = cfg.BuildSessionFactory();
                NS.Component.NHibernate.NHibernateFactory.Initialize(factory);
                return factory.OpenSession();
            }

        }



    }
}