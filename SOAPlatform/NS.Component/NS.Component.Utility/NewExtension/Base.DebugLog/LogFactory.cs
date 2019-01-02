using log4net;
using log4net.Repository;
using NS.Framework.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;

namespace NS.Component.Utility
{
    /// <summary>
    /// Log4Net日志 工厂
    /// 版本：2.0
    /// <author>
    ///		<name></name>
    ///		<date>2014.03.03</date>
    /// </author>
    /// </summary>
    public class LogFactory
    {
        private static ILoggerRepository repo { get; set; }
        static LogFactory()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(NSWebPath.GetServerPath("/XmlConfig/log4net.config")));
            repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
        }

        public static LogHelper GetLogger(Type type)
        {
            return new LogHelper(LogManager.GetLogger(type));
        }

        public static LogHelper GetLogger(string str)
        {
            return new LogHelper(LogManager.GetLogger(repo.Name, str));
        }
    }
}
