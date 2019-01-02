using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NS.Framework.Attributes;
using NS.Framework.Log;
using System.Xml;
using NS.Framework.Global;
using System.Reflection;
using log4net.Repository;

namespace NS.Component.Log
{
    [Export(typeof(ILogProvider))]
    public class Log4netProvider : ILogProvider
    {
        private readonly log4net.ILog log4netLogger;

        private static ILoggerRepository repo { get; set; }

        public Log4netProvider()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead(NSWebPath.GetServerPath("/XmlConfig/log4net.config")));
            repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            //log4net.Config.XmlConfigurator.Configure(new FileInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "log4net.xml")));
            log4netLogger = log4net.LogManager.GetLogger(this.GetType());
        }

        public void Debug(string message)
        {
            log4netLogger.Debug(message);
        }

        public void Debug(string message, Exception ex)
        {
            log4netLogger.Debug(message, ex);
        }

        public void Info(string message)
        {
            log4netLogger.Info(message);
        }

        public void Info(string message, Exception ex)
        {
            log4netLogger.Info(message, ex);
        }

        public void Warn(object message)
        {
            log4netLogger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log4netLogger.Warn(message, exception);
        }

        public void Error(object message)
        {
            log4netLogger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log4netLogger.Error(message, exception);
        }

        public void Fatal(object message)
        {
            log4netLogger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log4netLogger.Fatal(message, exception);
        }

        public bool IsDebugEnabled
        {
            get { return log4netLogger.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return log4netLogger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return log4netLogger.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return log4netLogger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return log4netLogger.IsFatalEnabled; }
        }
    }
}
