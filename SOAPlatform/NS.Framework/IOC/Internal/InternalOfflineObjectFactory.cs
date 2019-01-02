using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NS.Framework.AOP.Interceptors;
using NS.Framework.Attributes;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NS.Framework.Config;
using System.Reflection;
using Unity.Interception.ContainerIntegration;
using Unity.Registration;
using Unity;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;

namespace NS.Framework.IOC
{
    /// <summary>
    /// 离线服务IOC容器
    /// </summary>
    internal class InternalOfflineObjectFactory
    {
        #region 构造函数
        private log4net.ILog log4netLogger;
        private readonly object lock_flag = new object();

        /// <summary>
        /// IOC容器
        /// </summary>
        private UnityContainer unityContainer = new UnityContainer();

        /// <summary>
        /// GAC程序集集合
        /// </summary>
        private IList<string> GACAssemblys = new List<string>();

        /// <summary>
        /// 当前应用程序所在目录
        /// </summary>
        private string basePath = string.Empty;

        internal void InitializationFactory()
        {
            lock (lock_flag)
            {
#if SharePoint
               //从当前制定的路径下检索
                //var searchPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                //                                        "SharePoint");
                var searchPath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (!System.IO.Directory.Exists(searchPath))
                    return;

                LoadPath(searchPath);
                //TODO...
#else
                log4netLogger = log4net.LogManager.GetLogger(this.GetType());

                basePath = System.AppDomain.CurrentDomain.BaseDirectory;

                //添加拦截器
                unityContainer.AddNewExtension<Interception>();

                var configItems = Config.PlatformConfig.ServerConfig.IOCSetting.ConfigurationItems;

                if (configItems != null && configItems.Count > 0)
                {
                    //GAC中查找

                    this.GACAssemblys = GACAssemblyHelper.GetAssemblys();

                    foreach (var objectConfiguration in configItems)
                    {
                        RegisterTypeOrInstanceToUnity(objectConfiguration);
                    }
                }
                else
                {
                    //从当前制定的路径下检索
                    var searchPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath,
                                                            Config.PlatformConfig.ServerConfig.IOCSetting.SearchPath);
                    if (!System.IO.Directory.Exists(searchPath))
                        return;

                    LoadPath(searchPath);
                    //TODO...
                }

                var objectItems = Config.PlatformConfig.ServerConfig.IOCSetting.ObjectItems;

                if (objectItems != null && objectItems.Count > 0)
                {
                    foreach (var objectConfiguration in objectItems)
                    {
                        RegisterTypeOrInstanceToUnitySinglon(objectConfiguration);
                    }
                }

                //注册用户自定义实现组件
                var customObjectItems = Config.PlatformConfig.ServerConfig.IOCSetting.CustomObjectItems;

                if (customObjectItems != null && customObjectItems.Count > 0)
                {
                    foreach (var objectConfiguration in customObjectItems)
                    {
                        RegisterTypeOrInstanceToUnity(objectConfiguration);
                    }
                }
#endif
            }
        }

        private void RegisterTypeOrInstanceToUnity(Config.CustomObjectItem objectConfiguration)
        {
            try
            {
                //判断配置的值
                if (string.IsNullOrEmpty(objectConfiguration.Name) || string.IsNullOrEmpty(objectConfiguration.Interface) || string.IsNullOrEmpty(objectConfiguration.Implement))
                    return;

                RegisterSinglonTypeToUnity(objectConfiguration.Implement, objectConfiguration.LifeCycle);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                log4netLogger.Debug("注册类型的过程中出现错误" + errorMessage);

#if DEBUG
                throw;
#endif
            }
        }

        private void RegisterSinglonTypeToUnity(string implement, string liftCycle)
        {
            //获取程序集名称，判断是否已经有后缀
            string assemblyName = implement;

            string[] assemblyInfos = assemblyName.Split(';');
            if (assemblyInfos.Length != 2)
                return;

            assemblyName = assemblyInfos[0];

            string fileName = assemblyName.Replace(".dll", "").Replace(".DLL", "");

            if (assemblyName.LastIndexOf(".dll") < 0)
                assemblyName += ".dll";

            string assemblyPath = string.Empty;

            var tempKeyValuePair = PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key.ToLower() == "ioc").FirstOrDefault();

            if (tempKeyValuePair != null && tempKeyValuePair.Value.ToLower() == "gac")
                assemblyPath = this.GACAssemblys.Where(pre => pre.Split(',')[0].Contains(fileName)).FirstOrDefault();

            //装载程序集
            System.Reflection.Assembly assembly = null;

            if (string.IsNullOrEmpty(assemblyPath))
            {
                assembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(pre => pre.GetName().Name.ToLower() == assemblyName.ToLower()).FirstOrDefault();

                if (assembly == null)
                {
                    assemblyPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, assemblyName);

                    if (!Directory.Exists(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath))
                    {
                        assemblyPath = System.IO.Path.Combine(basePath, assemblyName);
                    }
                    else
                    {
                        //判断程序集是否存在
                        if (!System.IO.File.Exists(assemblyPath) && PlatformConfig.ServerConfig.IOCSetting.AutomaticSniffing)
                        {
                            assemblyPath = System.IO.Path.Combine(basePath, assemblyName);
                        }
                    }
                    //判断程序集是否存在
                    if (!System.IO.File.Exists(assemblyPath))
                        return;

                    //装载程序集
                    assembly = System.Reflection.Assembly.LoadFile(assemblyPath);
                }
            }
            else
            {
                //装载程序集
                assembly = System.Reflection.Assembly.Load(assemblyPath);
            }

            //如果装载失败
            if (assembly == null)
                return;

            if (assemblyFilters.Contains(fileName))
                return;

            //反射得到所有的程序集中的类型
            Type type = assembly.GetType(assemblyInfos[1]);

            if (type == null)
                return;

            RegisterCustomObjectItemType(type, liftCycle);
        }

        /// <summary>
        /// 注册指定的类型到IOC容器中
        /// </summary>
        /// <param name="objectConfiguration"></param>
        private void RegisterTypeOrInstanceToUnitySinglon(Config.ObjectItem objectConfiguration)
        {
            try
            {
                //判断配置的值
                if (string.IsNullOrEmpty(objectConfiguration.Name) || string.IsNullOrEmpty(objectConfiguration.Interface) || string.IsNullOrEmpty(objectConfiguration.Implement))
                    return;

                RegisterSinglonTypeToUnity(objectConfiguration.Implement, objectConfiguration.LifeCycle);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                log4netLogger.Debug("注册类型的过程中出现错误" + errorMessage);

#if DEBUG
                throw;
#endif
            }
        }

        private void RegisterTypeOrInstanceToUnity(Config.ObjectItem objectConfiguration)
        {
            try
            {
                //判断配置的值
                if (string.IsNullOrEmpty(objectConfiguration.Name) || string.IsNullOrEmpty(objectConfiguration.Interface) || string.IsNullOrEmpty(objectConfiguration.Implement))
                    return;

                //获取程序集名称，判断是否已经有后缀
                string assemblyName = objectConfiguration.Implement;

                string[] assemblyInfos = assemblyName.Split(';');
                if (assemblyInfos.Length != 2)
                    return;

                assemblyName = assemblyInfos[0];

                if (assemblyName.LastIndexOf(".dll") < 0)
                    assemblyName += ".dll";

                string assemblyPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, assemblyName);

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return;

                //装载程序集
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(assemblyPath);

                //如果装载失败
                if (assembly == null)
                    return;

                //反射得到所有的程序集中的类型
                Type[] types = assembly.GetTypes();

                //循环所有的类型，准备注册
                foreach (var type in types)
                {
                    if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                        continue;

                    //if (Config.PlatformConfig.ServerConfig.IOCSetting.InterceptionItems.Where(pre => System.IO.Path.GetFileNameWithoutExtension(pre.AssemblyName) == (System.IO.Path.GetFileNameWithoutExtension(objectConfiguration.Value))).Count() == 0) //判断如果注册拦截其器则拦截，否则不拦截
                    //    RegisterType(type, objectConfiguration.LifeCycle);
                    //else
                    RegisterInterceptionType(type, "Singleton");
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                log4netLogger.Debug("注册类型的过程中出现错误" + errorMessage);
#if DEBUG
                throw;
#endif
            }
        }

        private static string[] assemblyFilters = new string[] { "AutoMapper", "Iesi.Collections", "Microsoft.Practices.EnterpriseLibrary.Common", "Microsoft.Practices.EnterpriseLibrary.Validation", "Microsoft.Practices.ServiceLocation", 
            "Microsoft.Practices.Unity.Configuration", "Microsoft.Practices.Unity", "Microsoft.Practices.Unity.Interception","NHibernate","" };

        private void LoadPath(string searchPath)
        {
            try
            {
                System.IO.DirectoryInfo rootinfo = new DirectoryInfo(searchPath);

                FileInfo[] info = rootinfo.GetFiles();

                foreach (var fileInfo in info)
                {
                    if (fileInfo.Extension.ToLower() != ".dll" || assemblyFilters.Contains(System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name)))
                        continue;

                    //装载程序集
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(fileInfo.FullName);

                    //如果装载失败
                    if (assembly == null)
                        return;

                    //反射得到所有的程序集中的类型
                    Type[] types = assembly.GetTypes();

                    //循环所有的类型，准备注册
                    foreach (var type in types)
                    {
                        if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                            continue;
                        RegisterType(type, ObjectLifeCycle.New.ToString());
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException)
                    {
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();

                log4netLogger.Debug("注册类型的过程中出现错误" + errorMessage);
            }
        }

        /// <summary>
        /// 注册程序集中的所有类型，通过反射来进行注册
        /// </summary>
        /// <param name="objectConfiguration"></param>
        private void RegisterTypeOrInstanceToUnity(Config.ConfigurationItem objectConfiguration)
        {
            try
            {
                var tempArray = new string[] { "all", "offline" };
                if (!string.IsNullOrEmpty(objectConfiguration.LineMode) && !(tempArray.Contains(objectConfiguration.LineMode.ToLower())))
                    return;

                //判断配置的值
                if (string.IsNullOrEmpty(objectConfiguration.Value) || objectConfiguration.Value.Split(';').Length == 0)
                    return;

                string[] items = objectConfiguration.Value.Split(';');

                foreach (var assemblyFilter in items)
                {
                    if (string.IsNullOrEmpty(assemblyFilter))
                        continue;

                    RegisterTypeFromAssembly(assemblyFilter, objectConfiguration.LifeCycle);
                }

            }
            catch (Exception ex)
            {
                log4netLogger.Debug("注册类型的过程中出现错误", ex);

#if DEBUG
                throw;
#endif
            }
        }

        private void RegisterTypeFromAssembly(string assemblyFilter, string lifeCycle)
        {
            //获取程序集名称，判断是否已经有后缀
            string assemblyName = assemblyFilter;
            if (assemblyName.ToLower().LastIndexOf(".dll") < 0)
                assemblyName += ".dll";

            string fileName = assemblyFilter.Replace(".dll", "").Replace(".DLL", "");

            string assemblyPath = string.Empty;

            var tempKeyValuePair = PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key.ToLower() == "ioc").FirstOrDefault();

            if (tempKeyValuePair != null && tempKeyValuePair.Value.ToLower() == "gac")
                assemblyPath = this.GACAssemblys.Where(pre => pre.Split(',')[0].Contains(fileName)).FirstOrDefault();

            //装载程序集
            System.Reflection.Assembly assembly = null;

            if (string.IsNullOrEmpty(assemblyPath))
            {
                //从当前的程序集域中加载程序集
                assembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(pre => pre.GetName().Name.ToLower() == assemblyName.ToLower()).FirstOrDefault();

                if (assembly == null)
                {
                    assemblyPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, assemblyName);

                    if (!Directory.Exists(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath))
                    {
                        assemblyPath = System.IO.Path.Combine(basePath, assemblyName);
                    }
                    else
                    {
                        //判断程序集是否存在
                        if (!System.IO.File.Exists(assemblyPath) && PlatformConfig.ServerConfig.IOCSetting.AutomaticSniffing)
                        {
                            assemblyPath = System.IO.Path.Combine(basePath, assemblyName);
                        }
                    }

                    //判断程序集是否存在
                    if (!System.IO.File.Exists(assemblyPath))
                        return;

                    //装载程序集
                    assembly = System.Reflection.Assembly.LoadFile(assemblyPath);
                }
            }
            else
            {
                //装载程序集
                assembly = System.Reflection.Assembly.Load(assemblyPath);
            }

            //如果装载失败
            if (assembly == null)
                return;

            if (assemblyFilters.Contains(fileName))
                return;

            //反射得到所有的程序集中的类型
            Type[] types = assembly.GetTypes();

            //循环所有的类型，准备注册
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                    continue;

                //if (Config.PlatformConfig.ServerConfig.IOCSetting.InterceptionItems.Where(pre => System.IO.Path.GetFileNameWithoutExtension(pre.AssemblyName) == (System.IO.Path.GetFileNameWithoutExtension(objectConfiguration.Value))).Count() == 0) //判断如果注册拦截其器则拦截，否则不拦截
                //    RegisterType(type, objectConfiguration.LifeCycle);
                //else
                RegisterInterceptionType(type, lifeCycle);
            }
        }

        private void RegisterType(Type type, string lifeCycle)
        {
            var exportAttributes = type.GetCustomAttributes(typeof(ExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return;

            ExportAttribute exportAttribute = exportAttributes[0] as ExportAttribute;

            if (exportAttribute == null)
                return;

            //string[] filterArray = new string[] { "IEnumerable" };
            //Type[] objectInterfaces = interfaces.Where(pre => !filterArray.Contains(pre.Name) && !(pre.IsGenericType)).ToArray();

            switch (GetObjectLifeCycle(lifeCycle))
            {
                case ObjectLifeCycle.Singleton:
                    //foreach (var tempInstance in objectInterfaces)
                    unityContainer.RegisterInstance(exportAttribute.InterfaceType, Activator.CreateInstance(type));
                    break;
                case ObjectLifeCycle.New:
                    //foreach (var tempInstance in objectInterfaces)
                    unityContainer.RegisterType(exportAttribute.InterfaceType, type);
                    break;
            }
        }

        private void RegisterCustomObjectItemType(Type type, string lifeCycle)
        {
            Type interfaceType = null;

            interfaceType = this.GetInterfaceType(type, lifeCycle);

            if (interfaceType == null)
                return;

            if (unityContainer.IsRegistered(interfaceType))
                return;

            switch (GetObjectLifeCycle(lifeCycle))
            {
                case ObjectLifeCycle.Singleton:
                    unityContainer.RegisterInstance(interfaceType, Activator.CreateInstance(type));
                    break;
                case ObjectLifeCycle.New:
                    //判定是否给予该程序集注册了类型，如果没有给予该程序集注册类型，则不进行任何的处理.
                    bool flag = ValidationIsRegisterInterceptor(type);
                    if (!flag)
                        unityContainer.RegisterType(interfaceType, type);
                    else
                    {
                        this.RegisterAop(interfaceType, type);
                    }

                    break;
            }
        }

        private void RegisterInterceptionType(Type type, string lifeCycle)
        {
            Type interfaceType = null;

            interfaceType = this.GetInterfaceType(type, lifeCycle);

            if (interfaceType == null)
                return;

            if (!(lifeCycle.ToLower() == "singleton"))
            {
                //判定是否单独配置了类型应对的具体实现，如果配置则不执行任何的注册操作，最后统一注册。
                if (Config.PlatformConfig.ServerConfig.IOCSetting.ObjectItems.Where(pre => pre.Interface == interfaceType.FullName).Count() > 0)
                    return;

                //判定是否单独配置了类型应对的具体实现，如果配置则不执行任何的注册操作，最后统一注册。
                if (Config.PlatformConfig.ServerConfig.IOCSetting.CustomObjectItems.Where(pre => pre.Interface == interfaceType.FullName).Count() > 0)
                    return;
            }

            //string[] filterArray = new string[] { "IEnumerable" };
            //Type[] objectInterfaces = interfaces.Where(pre => !filterArray.Contains(pre.Name) && !(pre.IsGenericType)).ToArray();

            if (unityContainer.IsRegistered(interfaceType))
                return;

            switch (GetObjectLifeCycle(lifeCycle))
            {
                case ObjectLifeCycle.Singleton:

                    //foreach (var tempInstance in objectInterfaces)
                    unityContainer.RegisterInstance(interfaceType, Activator.CreateInstance(type));
                    break;
                case ObjectLifeCycle.New:
                    //foreach (var tempInstance in objectInterfaces)

                    //判定是否给予该程序集注册了类型，如果没有给予该程序集注册类型，则不进行任何的处理.

                    bool flag = ValidationIsRegisterInterceptor(type);

                    if (!flag)
                    {
                        unityContainer.RegisterType(interfaceType, type);
                        if (!unityContainer.IsRegistered(interfaceType))
                        {
                            throw new NS.Framework.Exceptions.FrameworkException(string.Format("向IOC容器中注册类型:{0}失败，未注册成功", interfaceType.FullName));
                        }
                    }
                    else
                    {
                        this.RegisterAop(interfaceType, type);
                    }

                    //动态方式
                    //IList<InjectionMember> injectionMembers = new List<InjectionMember>();

                    //int i = 0;
                    //string assemblyName = type.Assembly.GetName().Name;
                    //foreach (var InterceptorItem in Config.PlatformConfig.ServerConfig.IOCSetting.InterceptorItems)
                    //{
                    //    if (InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).Count() == 0)
                    //        continue;

                    //    var tempInterceptorAssembly =
                    //        InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).FirstOrDefault();

                    //    if (tempInterceptorAssembly.InterceptorTypeIgnores.Where(pre => pre.Type == type.FullName).Count() > 0)
                    //        continue;

                    //    if (string.IsNullOrEmpty(InterceptorItem.TypeName))
                    //        continue;

                    //    Type tempType = TryFindType(InterceptorItem.TypeName);

                    //    if (tempType == null)
                    //        continue;

                    //    unityContainer.RegisterType(tempType, tempType);

                    //    injectionMembers.Add((InterceptionBehavior)unityContainer.Resolve(tempType));
                    //    i++;
                    //}

                    //injectionMembers.Add(new Interceptor<InterfaceInterceptor>());

                    //unityContainer.RegisterType(exportAttribute.InterfaceType, type, injectionMembers.ToArray());
                    //固定方式
                    //unityContainer.RegisterType(exportAttribute.InterfaceType, type, new Interceptor<InterfaceInterceptor>(),
                    //    new InterceptionBehavior<CachingBehavior>(),
                    //    new InterceptionBehavior<ExceptionLoggingBehavior>(),
                    //    new InterceptionBehavior<PermissionInterceptionBehavior>(),
                    //    new InterceptionBehavior<ValidatorInterceptionBehavior>(),
                    //    new InterceptionBehavior<MethodCallToMessageInterceptionBehavior>());

                    //unityContainer.RegisterType(exportAttribute.InterfaceType, type, type.FullName, new Interceptor<InterfaceInterceptor>(),
                    //    new InterceptionBehavior<CachingBehavior>(),
                    //    new InterceptionBehavior<ExceptionLoggingBehavior>(),
                    //    new InterceptionBehavior<PermissionInterceptionBehavior>(),
                    //    new InterceptionBehavior<MethodCallToMessageInterceptionBehavior>(), new InjectionConstructor(unityContainer.Resolve(exportAttribute.InterfaceType,type.FullName)));
                    break;
            }

            //根据指定的类型获取所有的对方法参数的判定
            //LogicValidatorContainer.RegisterType(interfaceType);
        }

        private Type GetInterfaceType(Type type, string lifeCycle)
        {
            Type interfaceType = null;
            var exportAttributes = type.GetCustomAttributes(typeof(ExportAttribute), false);

            if (exportAttributes != null && exportAttributes.Length > 0)
            {
                ExportAttribute exportAttribute = exportAttributes[0] as ExportAttribute;

                if (exportAttribute != null)
                {
                    interfaceType = exportAttribute.InterfaceType;
                    return interfaceType;
                }
            }

            //判定是本地访问模式-读取服务特性
            if (interfaceType == null && Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode.ToLower() == "local")
            {
                interfaceType = GetServiceType(type);

                if (interfaceType != null)
                    return interfaceType;
            }

            //判定是本地访问模式-读取事件处理特性
            if (interfaceType == null && Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode.ToLower() == "local")
            {
                interfaceType = GetEventType(type);

                if (interfaceType != null)
                {
                    //注册事件
                    InternalEventHandlerFactory.RegisterType(type, lifeCycle);
                    return interfaceType;
                }
            }

            //判定是本地访问模式-读取规则特性
            if (interfaceType == null && Config.PlatformConfig.ServerConfig.WCFSetting.ServiceMode.ToLower() == "local")
            {
                interfaceType = GetRuleType(type);

                if (interfaceType != null)
                {
                    //注册规则
                    //InternaRuleHandlerFactory.RegisterType(type, lifeCycle);
                    return interfaceType;
                }
            }

            return interfaceType;
        }

        private Type GetServiceType(Type type)
        {
            Type interfaceType = null;
            var serviceExportAttributes = type.GetCustomAttributes(typeof(ServiceExportAttribute), false);
            if (serviceExportAttributes == null || serviceExportAttributes.Length == 0)
                return interfaceType;

            ServiceExportAttribute exportAttribute = serviceExportAttributes[0] as ServiceExportAttribute;

            if (exportAttribute == null)
                return interfaceType;

            interfaceType = exportAttribute.InterfaceType;
            return interfaceType;
        }

        private Type GetRuleType(Type type)
        {
            Type interfaceType = null;
            var ruleExportAttributes = type.GetCustomAttributes(typeof(RuleExportAttribute), false);
            if (ruleExportAttributes == null || ruleExportAttributes.Length == 0)
                return interfaceType;

            RuleExportAttribute ruleExportAttribute = ruleExportAttributes[0] as RuleExportAttribute;

            if (ruleExportAttribute == null)
                return interfaceType;

            interfaceType = ruleExportAttribute.InterfaceType;

            return interfaceType;
        }

        private Type GetEventType(Type type)
        {
            Type interfaceType = null;
            var eventExportAttributes = type.GetCustomAttributes(typeof(EventExportAttribute), false);
            if (eventExportAttributes == null || eventExportAttributes.Length == 0)
                return interfaceType;

            EventExportAttribute eventExportAttribute = eventExportAttributes[0] as EventExportAttribute;

            if (eventExportAttribute == null)
                return interfaceType;

            interfaceType = eventExportAttribute.InterfaceType;

            return interfaceType;
        }

        private bool ValidationIsRegisterInterceptor(Type type)
        {
            string assemblyName = type.Assembly.GetName().Name;
            if (Config.PlatformConfig.ServerConfig.IOCSetting.InterceptorItems.Count == 0)
                return false;

            foreach (var InterceptorItem in Config.PlatformConfig.ServerConfig.IOCSetting.InterceptorItems)
            {

                if (InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).Count() == 0)
                    continue;

                return true;

                //var tempInterceptorAssembly =
                //    InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).FirstOrDefault();

                //return tempInterceptorAssembly.InterceptorTypeIgnores.Where(pre => pre.Type == type.FullName).Count()>0;
            }

            return false;
        }

        private Type TryFindType(string typeName)
        {
            //判断配置的值
            if (string.IsNullOrEmpty(typeName) || typeName.Split(';').Length != 2)
                return null;

            string[] items = typeName.Split(';');

            if (string.IsNullOrEmpty(items[0]) || string.IsNullOrEmpty(items[1]))
                return null;

            string assemblyName = items[0];
            var tempName = assemblyName;

            if (assemblyName.LastIndexOf(".dll") < 0)
                assemblyName += ".dll";

            System.Reflection.Assembly assembly = null;

            assembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(pre => pre.GetName().Name == tempName).FirstOrDefault();

            if (assembly == null)
            {
                string assemblyPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, assemblyName);

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return null;

                //装载程序集
                assembly = System.Reflection.Assembly.LoadFile(assemblyPath);
            }

            if (assembly == null)
                return null;

            return assembly.GetType(items[1]);
        }

        private ObjectLifeCycle GetObjectLifeCycle(string lifeCycle)
        {
            ObjectLifeCycle tempLifeCycle = ObjectLifeCycle.Singleton;
            if (string.IsNullOrEmpty(lifeCycle))
                return tempLifeCycle;

            switch (lifeCycle.ToLower())
            {
                case "singleton":
                    tempLifeCycle = ObjectLifeCycle.Singleton;
                    break;
                case "new":
                    tempLifeCycle = ObjectLifeCycle.New;
                    break;
            }
            return tempLifeCycle;
        }

        #endregion

        public void RegisterType(Type from, Type to)
        {
            lock (lock_flag)
                this.RegisterAop(from, to);
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            lock (lock_flag)
                this.RegisterAop(typeof(TFrom), typeof(TTo));
        }

        public void RegisterType<TFrom, TTo>(params InjectionMember[] injectionMembers) where TTo : TFrom
        {
            lock (lock_flag)
                unityContainer.RegisterType(typeof(TFrom), typeof(TTo), injectionMembers);
        }

        private void RegisterAop(Type fromType, Type toType)
        {
            //动态方式

            IList<InjectionMember> injectionMembers = new List<InjectionMember>();

            string assemblyName = toType.Assembly.GetName().Name;
            foreach (var InterceptorItem in Config.PlatformConfig.ServerConfig.IOCSetting.InterceptorItems)
            {
                if (InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).Count() == 0)
                    continue;

                var tempInterceptorAssembly =
                    InterceptorItem.InterceptorAssemblys.Where(pre => pre.AssemblyName == assemblyName).FirstOrDefault();

                if (tempInterceptorAssembly.InterceptorTypeIgnores.Where(pre => pre.Type == toType.FullName).Count() > 0)
                    continue;
                //assemblyName="LS.IS.OrganizationalService.Repository.Meta"
                if (string.IsNullOrEmpty(InterceptorItem.TypeName))
                    continue;

                Type tempType = TryFindType(InterceptorItem.TypeName);

                if (tempType == null)
                    continue;

                unityContainer.RegisterType(tempType, tempType);

                injectionMembers.Add(new InterceptionBehavior(tempType));
            }

            injectionMembers.Add(new Interceptor<InterfaceInterceptor>());

            if (injectionMembers.Count > 1)
            {
                unityContainer.RegisterType(fromType, toType, injectionMembers.ToArray());
            }
            else
                unityContainer.RegisterType(fromType, toType);
        }

        public void RegisterType(Type from, Type to, params InjectionMember[] injectionMembers)
        {
            lock (lock_flag)
                unityContainer.RegisterType(from, to, injectionMembers);
        }

        public T CreateInstance<T>()
        {
            lock (lock_flag)
                return unityContainer.Resolve<T>();
        }

        public void RegisterInstance(Type type, object instance)
        {
            lock (lock_flag)
                unityContainer.RegisterInstance(type, instance);
        }

        public void RegisterInstance<T>(T instance)
        {
            lock (lock_flag)
                unityContainer.RegisterInstance<T>(instance);
        }

        public object CreateInstance(Type type)
        {
            lock (lock_flag)
                return unityContainer.Resolve(type);
        }

        public int GetRegisterCount()
        {
            lock (lock_flag)
                return unityContainer.Registrations.Count();
        }

        public IEnumerable<IContainerRegistration> GetRegisterItems()
        {
            lock (lock_flag)
                return unityContainer.Registrations;
        }
    }
}
