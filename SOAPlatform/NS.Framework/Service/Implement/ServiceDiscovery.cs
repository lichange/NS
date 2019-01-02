//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Reflection;

//using NS.Framework.Attributes;

//namespace NS.Framework.Service.Implement
//{
//    /// <summary>
//    /// 服务动态发现的组件-
//    /// </summary>
//    [Export(typeof(ServiceDiscovery))]
//    internal class ServiceDiscovery : IServiceDiscovery
//    {
//        /// <summary>
//        /// 同步对象flag
//        /// </summary>
//        private static readonly object lock_flag = new object();

//        /// <summary>
//        /// 动态注册指定路径下的所有程序集中包含的服务信息。-注册到平台中的服务,就会被平台动态的发布和管理
//        /// </summary>
//        /// <param name="directoryPath">文件夹路径</param>
//        /// <param name="filter">文件名过滤器-后匹配</param>
//        /// <param name="ignoredfiles">忽略的程序集文件名称集合</param>
//        /// <returns>返回从指定路径下发现的所有服务信息</returns>
//        public IList<ServiceMetaData> GetServiceMetaDatas(string directoryPath, string filter, params string[] ignoredfileNames)
//        {
//            IList<ServiceMetaData> serviceMetaDatas = new List<ServiceMetaData>();

//            if (!Directory.Exists(directoryPath))
//            {
//                directoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
//            }

//            if (System.Web.HttpContext.Current != null)
//                directoryPath = Path.Combine(directoryPath, "bin");

//            var tempFilters = filter.Split(';');

//            DirectoryInfo info = new DirectoryInfo(directoryPath);
//            var tempFiles = info.EnumerateFiles().Where(pre=>pre.Extension.ToLower()==".dll").ToList();
//            foreach (FileInfo fi in tempFiles)
//            {
//                if (ignoredfileNames != null && ignoredfileNames.Contains(fi.Name))
//                    continue;

//                if (!string.IsNullOrEmpty(filter) && (!fi.Name.Contains(filter) && tempFilters.Where(pre=>fi.Name.Contains(pre)).Count()==0))
//                    continue;
                
//                this.AddRange(serviceMetaDatas,this.GetServiceMetaDatas(fi));
//            }

//            return serviceMetaDatas;
//        }

//        #region IServiceDiscovery 成员

//        public IList<ServiceMetaData> GetServiceMetaDatas(string filePath)
//        {
//            IList<ServiceMetaData> serviceMetaDatas = new List<ServiceMetaData>();

//            if (string.IsNullOrEmpty(filePath))
//                return serviceMetaDatas;

//            if (!File.Exists(filePath))
//                return serviceMetaDatas;
//            FileInfo fi = new FileInfo(filePath);

//            return GetServiceMetaDatas(fi);
//        }

//        private IList<ServiceMetaData> GetServiceMetaDatas(FileInfo fi)
//        {
//            IList<ServiceMetaData> serviceMetaDatas = new List<ServiceMetaData>();

//            if (fi.Extension.ToLower() != ".dll")
//                return serviceMetaDatas;

//            Assembly assembly = null;
//            try
//            {
//                assembly = Assembly.LoadFrom(fi.FullName);
//            }
//            catch
//            {
//                // 文件不是合法的程序集
//                return serviceMetaDatas;
//            }

//            Type[] types = assembly.GetExportedTypes();
//            foreach (Type type in types)
//            {
//                if (type.IsClass)
//                {
//                    var attributes = (ServiceExportAttribute[])type.GetCustomAttributes(typeof(ServiceExportAttribute), false);

//                    if (attributes != null && attributes.Length > 0)
//                    {
//                        var tempAttribute = attributes[0];

//                        var serviceMetaData = new ServiceMetaData();
//                        serviceMetaData.ServiceContract = tempAttribute.InterfaceType;
//                        serviceMetaData.ImplementType = type;
//                        serviceMetaDatas.Add(serviceMetaData);
//                    }
//                }
//            }

//            return serviceMetaDatas;
//        }

//        #endregion

//        #region 平台启动或关闭时的处理
//        internal void OnStartPlatform(object sender, EventArgs e)
//        {

//        }

//        internal void OnStopPlatform(object sender, EventArgs e)
//        {
//        }
//        #endregion

//        private void AddRange<T>(ICollection<T> source, IEnumerable<T> objects)
//        {
//            if (source != null && objects != null)
//            {
//                foreach (var item in objects)
//                {
//                    source.Add(item);
//                }
//            }
//        }
//    }
//}
