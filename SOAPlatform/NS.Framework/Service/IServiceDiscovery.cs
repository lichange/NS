//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NS.Framework.Service
//{
//    /// <summary>
//    /// 服务动态发现的组件
//    /// </summary>
//    public interface IServiceDiscovery
//    {
//        /// <summary>
//        /// 动态注册指定路径下的所有程序集中包含的服务信息。-注册到平台中的服务,就会被平台动态的发布和管理
//        /// </summary>
//        /// <param name="directoryPath">文件夹路径</param>
//        /// <param name="filter">文件名过滤器-后匹配</param>
//        /// <param name="ignoredfiles">忽略的程序集文件名称集合</param>
//        /// <returns>返回从指定路径下发现的所有服务信息</returns>
//        IList<ServiceMetaData> GetServiceMetaDatas(string directoryPath, string filter, params string[] ignoredfileNames);

//        /// <summary>
//        /// 动态注册指定程序集中包含的服务信息。-注册到平台中的服务,就会被平台动态的发布和管理
//        /// </summary>
//        /// <param name="directoryPath">程序集路径</param>
//        /// <returns>返回从指定路径下发现的所有服务信息</returns>
//        IList<ServiceMetaData> GetServiceMetaDatas(string filePath);
//    }
//}
