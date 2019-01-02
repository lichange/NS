using System;
using System.Configuration;

namespace NS.Component.Utility
{
    public class Appsettings
    {
        /// <summary>
        /// js/css文件版本号
        /// </summary>
        public static string Versions
        {
            //get { return ConfigurationManager.AppSettings["Versions"]; }
            get { return Guid.NewGuid().ToString(); }
        }

        /// <summary>
        /// web初始化配置文件地址
        /// </summary>
        public static string InitConfig
        {
            get { return FileUtility.GetPhysicalPath(ConfigurationManager.AppSettings["InitConfig"]); }
        }
    }
}
