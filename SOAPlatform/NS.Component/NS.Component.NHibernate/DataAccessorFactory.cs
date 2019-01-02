using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NS.Component.Data;
using NS.Component.Data.Impl;
using System.IO;

namespace NS.Component.Data
{
    /// <summary>
    /// 数据访问器构造工厂,当应用系统中存在多个数据库配置时,创建多个工厂实例即可
    /// </summary>
    public class DataAccessorFactory
    {
        private DataAccessorConfiguration config;
        private IDataAccessor accessor;
        /// <summary>
        /// 使用配置信息来创建工厂
        /// </summary>
        /// <param name="config">持久化数据访问的配置信息对象</param>
        public DataAccessorFactory(DataAccessorConfiguration config)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.ConfigFile) || (config.MappingsAssemblies == null || config.MappingsAssemblies.Count == 0))
                throw new ArgumentNullException();
            if (!File.Exists(config.ConfigFile))
                throw new ArgumentException();
            this.config = config;
        }

        /// <summary>
        /// 创建数据访问器,每个工厂只针对一个配置文件,构造一个访问器,确保访问器线程安全
        /// </summary>
        /// <returns>持久化数据访问器</returns>
        public IDataAccessor CreateDataAccessor()
        {
            if (accessor != null)
                return accessor;

            var tempKeyItem = NS.Framework.Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode;

            if (tempKeyItem != null || tempKeyItem.ToLower() == "distribute")
                accessor = new WCFNHibernateDataAccessor(config);
            else
                accessor = new NHibernateDataAccessor(config);

            return accessor;
        }

        /// <summary>
        /// 使用应用程序的基目录中的hibernate.cfg.xml来配置Nhibernate
        /// </summary>
        /// <param name="assembly">包含实体类以及映射文件的程序集</param>
        /// <returns>数据访问器接口</returns>
        public static IDataAccessor CreateDataAccessor(System.Reflection.Assembly assembly)
        {
            DataAccessorConfiguration configuration = new DataAccessorConfiguration();
            configuration.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml");
            configuration.MappingsAssemblies.Add(assembly);

            DataAccessorFactory factory = new DataAccessorFactory(configuration);

            return factory.CreateDataAccessor();
        }

        /// <summary>
        /// 通过提供配置文件和映射信息程序集来创建持久化数据访问器
        /// </summary>
        /// <param name="path">配置文件全路径</param>
        /// <param name="assembly">NHibernate映射信息程序集</param>
        /// <returns>数据访问器接口</returns>
        public static IDataAccessor CreateDataAccessor(string path, System.Reflection.Assembly assembly)
        {
            DataAccessorConfiguration configuration = new DataAccessorConfiguration();
            configuration.ConfigFile = path;
            configuration.MappingsAssemblies.Add(assembly);
            DataAccessorFactory factory = new DataAccessorFactory(configuration);
            return factory.CreateDataAccessor();
        }
    }
}
