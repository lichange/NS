
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NHibernate.Cfg;

namespace NS.Component.Data
{
    /// <summary>
    /// 数据持久化访问的配置信息
    /// 基于NHibernate技术
    /// 提供NHibernate.cfg.xml文件路径
    /// 要求NHibernate的实体类和配置文件被打包到程序集，并提供对应的程序集名称
    /// </summary>
    public class DataAccessorConfiguration
    {
        /// <summary>
        /// hibernate.cfg.xml 配置文件路径全名称
        /// </summary>
        public string ConfigFile { get; set; }

        /// <summary>
        /// 包含实体类以及映射文件的程序集集合
        /// </summary>
        public List<Assembly> MappingsAssemblies { get; set; }

        /// <summary>
        /// 需要传入的命名策略,
        /// 可改善的地方：如果不希望调用者引用NHibernate,可以替换INamingStrategy
        /// </summary>
        public INamingStrategy NamingStrategy { get; set; }

        /// <summary>
        /// 事务的隔离级别,当为空值时,使用数据库默认级别
        /// </summary>
        public System.Data.IsolationLevel? IsoLevel { get; set; }
    }
}
