using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NS.Framework.Attributes;
using NS.Framework.Utility.Collections;
using NS.Framework.Config;
using Unity;
using Unity.Registration;

namespace NS.Framework.IOC
{
    internal class InternaRuleHandlerFactory
    {
        #region 构造函数

        private static readonly object lock_flag = new object();
        public static InternaRuleHandlerFactory factory = null;
        private static UnityContainer unityContainer = new UnityContainer();
        private static ThreadSafeDictionary<Type, IList<RuleHandlerKeyValuePair>> ruleHandlerContainer = new ThreadSafeDictionary<Type, IList<RuleHandlerKeyValuePair>>();
        /// <summary>
        /// GAC程序集集合
        /// </summary>
        private static IList<string> GACAssemblys = new List<string>();
        public static InternaRuleHandlerFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lock_flag)
                    {
                        if (factory == null)
                            factory = new InternaRuleHandlerFactory();
                    }
                }
                return factory;
            }
        }

        static InternaRuleHandlerFactory()
        {
            var configItems = Config.PlatformConfig.ServerConfig.IOCSetting.ConfigurationItems;

            if (configItems != null && configItems.Count > 0)
            {
                //GAC中查找

                GACAssemblys = GACAssemblyHelper.GetAssemblys();

                foreach (var objectConfiguration in configItems)
                {
                    if (objectConfiguration.Key.ToLower() != "rule")
                        continue;
                    RegisterTypeOrInstanceToUnity(objectConfiguration);
                }
            }
            else
            {
                //从当前制定的路径下检索
                var searchPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
                                                        Config.PlatformConfig.ServerConfig.IOCSetting.SearchPath);
                if (!System.IO.Directory.Exists(searchPath))
                    return;

                LoadPath(searchPath);
                //TODO...
            }
        }

        #region 单独注册

        private static void LoadPath(string searchPath)
        {
            try
            {
                System.IO.DirectoryInfo rootinfo = new DirectoryInfo(searchPath);

                FileInfo[] info = rootinfo.GetFiles();

                foreach (var fileInfo in info)
                {
                    if (fileInfo.Extension.ToLower() != ".dll")
                        continue;

                    //装载程序集
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(fileInfo.FullName);

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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 注册程序集中的所有类型，通过反射来进行注册
        /// </summary>
        /// <param name="objectConfiguration"></param>
        private static void RegisterTypeOrInstanceToUnity(Config.ConfigurationItem objectConfiguration)
        {
            try
            {
                //判断配置的值
                if (string.IsNullOrEmpty(objectConfiguration.Value))
                    return;

                if (objectConfiguration.Value.Split(';').Length == 0)
                    return;

                var tempAssemblys = objectConfiguration.Value.Split(';');

                foreach (var tempAssembly in tempAssemblys)
                {
                    if (string.IsNullOrEmpty(tempAssembly))
                        continue;

                    //获取程序集名称，判断是否已经有后缀
                    string assemblyName = tempAssembly;
                    var searchName = tempAssembly.Replace(".dll", "").Replace(".Dll", "").Replace(".DLL", "");

                    if (assemblyName.ToLower().LastIndexOf(".dll") < 0)
                        assemblyName += ".dll";

                    string assemblyPath = string.Empty;

                    var tempKeyValuePair = PlatformConfig.ServerConfig.KeyValueSettings.KeyValueItems.Where(pre => pre.Key.ToLower() == "ioc").FirstOrDefault();

                    if (tempKeyValuePair != null && tempKeyValuePair.Value.ToLower() == "gac")
                        assemblyPath = GACAssemblys.Where(pre => pre.Split(',')[0].Contains(searchName)).FirstOrDefault();

                    //装载程序集
                    System.Reflection.Assembly assembly = null;

                    if (string.IsNullOrEmpty(assemblyPath))
                    {
                        assemblyPath = System.IO.Path.Combine(PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, assemblyName);

                        //判断程序集是否存在
                        if (!System.IO.File.Exists(assemblyPath))
                            return;

                        //装载程序集
                        assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
                    }
                    else
                    {
                        //装载程序集
                        assembly = System.Reflection.Assembly.Load(assemblyPath);
                    }

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
                        RegisterType(type, objectConfiguration.LifeCycle);
                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        #endregion

        internal static void RegisterType(Type type, string lifeCycle)
        {
            var exportAttributes = type.GetCustomAttributes(typeof(RuleExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return;

            RuleExportAttribute exportAttribute = exportAttributes[0] as RuleExportAttribute;

            switch (GetObjectLifeCycle(lifeCycle))
            {
                case ObjectLifeCycle.Singleton:
                    unityContainer.RegisterInstance(exportAttribute.InterfaceType, Activator.CreateInstance(type));
                    var tempArguments = exportAttribute.InterfaceType.GetGenericArguments();
                    var eventType = tempArguments.Count() > 0 ? tempArguments[0] : exportAttribute.InterfaceType;

                    if (ruleHandlerContainer.ContainsKey(eventType))
                        ruleHandlerContainer[eventType].Add(new RuleHandlerKeyValuePair()
                            {
                                InterfaceType = exportAttribute.InterfaceType,
                                ImplementType = type
                            });
                    else
                    {
                        IList<RuleHandlerKeyValuePair> tempTypes = new List<RuleHandlerKeyValuePair>();
                        tempTypes.Add(new RuleHandlerKeyValuePair()
                        {
                            InterfaceType = exportAttribute.InterfaceType,
                            ImplementType = type
                        });
                        ruleHandlerContainer.Add(eventType, tempTypes);
                    }
                    break;
                case ObjectLifeCycle.New:
                    //foreach (var tempInstance in objectInterfaces)
                    unityContainer.RegisterType(exportAttribute.InterfaceType, type);
                    var tempArguments1 = exportAttribute.InterfaceType.GetGenericArguments();
                    var eventType1 = tempArguments1.Count() > 0 ? tempArguments1[0] : exportAttribute.InterfaceType;

                    if (ruleHandlerContainer.ContainsKey(eventType1))
                        ruleHandlerContainer[eventType1].Add(new RuleHandlerKeyValuePair()
                             {
                                 InterfaceType = exportAttribute.InterfaceType,
                                 ImplementType = type
                             });
                    else
                    {
                        IList<RuleHandlerKeyValuePair> tempTypes = new List<RuleHandlerKeyValuePair>();
                        tempTypes.Add(new RuleHandlerKeyValuePair()
                        {
                            InterfaceType = exportAttribute.InterfaceType,
                            ImplementType = type
                        });
                        ruleHandlerContainer.Add(eventType1, tempTypes);
                    }
                    break;
            }
        }

        private static ObjectLifeCycle GetObjectLifeCycle(string lifeCycle)
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

        public IList<object> GetRuleHandlers<T>()
        {
            IList<object> tResults = new List<object>();
            lock (lock_flag)
            {
                if (ruleHandlerContainer.ContainsKey(typeof(T)))
                {
                    IList<RuleHandlerKeyValuePair> ruleHandlerKeyValuePairs = ruleHandlerContainer[typeof(T)];

                    foreach (var ruleHandlerKeyValuePair in ruleHandlerKeyValuePairs)
                    {
                        tResults.Add(unityContainer.Resolve(ruleHandlerKeyValuePair.InterfaceType));
                    }

                }
            }
            return tResults;
        }

        public IList<object> GetRuleHandlers(Type type, string ruleType, string realTargetName)
        {
            IList<object> tResults = new List<object>();
            lock (lock_flag)
            {
                //var tempType = ruleHandlerContainer.Where(pre => pre.Key.FullName == type.FullName).FirstOrDefault();
                //if (tempType.Key==null || tempType.Value==null)
                //    return tResults;

                //var interfaceTypeName = this.GetTypeName(ruleType);
                //interfaceTypeName = realTargetName + interfaceTypeName;
                //var tempKeyValuePair = tempType.Value.Where(
                //    pre =>
                //    pre.ImplementType.Name.StartsWith(interfaceTypeName) &&
                //    pre.InterfaceType.GetGenericArguments()[0].Name == type.Name).LastOrDefault();

                //if (tempKeyValuePair == null)
                //    return new List<object>();

                //tResults.Add(ObjectContainer.CreateInstance(tempKeyValuePair.InterfaceType));

                if (ruleHandlerContainer.ContainsKey(type))
                {
                    IList<RuleHandlerKeyValuePair> ruleHandlerKeyValuePairs = ruleHandlerContainer[type];

                    if (ruleHandlerKeyValuePairs == null)
                        return tResults;

                    if (ruleHandlerKeyValuePairs.Count == 0)
                        return new List<object>();

                    var interfaceTypeName = this.GetTypeName(ruleType);
                    interfaceTypeName = realTargetName + interfaceTypeName;
                    var tempKeyValuePair = ruleHandlerKeyValuePairs.Where(
                        pre =>
                        pre.ImplementType.Name.StartsWith(interfaceTypeName) &&
                        pre.InterfaceType.GetGenericArguments()[0].Name == type.Name).LastOrDefault();

                    if (tempKeyValuePair == null)
                        return new List<object>();

                    tResults.Add(ObjectContainer.CreateInstance(tempKeyValuePair.ImplementType));
                }
            }
            return tResults;
        }

        private string GetTypeName(string ruleType)
        {
            switch (ruleType.ToLower())
            {
                case "add":
                    return "Addable";
                    break;
                case "update":
                    return "Updateable";
                    break;
                case "delete":
                    return "Deleteable";
                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// 向总线上注册事件
        /// </summary>
        ///<param name="handlerType">处理器类型</param>
        /// <returns>返回操作的结果</returns>
        public bool RegisterHandler(Type handlerType)
        {
            if (handlerType == null || handlerType.IsInterface)
                return false;
            var exportAttributes = handlerType.GetCustomAttributes(typeof(RuleExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return false;

            RuleExportAttribute exportAttribute = exportAttributes[0] as RuleExportAttribute;
            if (exportAttribute == null)
                return false;

            Type eventType = null;

            if (exportAttribute.InterfaceType.GetGenericArguments().Length > 0)
                eventType = exportAttribute.InterfaceType.GetGenericArguments()[0];
            else
                eventType = exportAttribute.InterfaceType;

            if (eventType == null)
                return false;

            if (ruleHandlerContainer.ContainsKey(eventType))
                ruleHandlerContainer[eventType].Add(new RuleHandlerKeyValuePair()
                {
                    InterfaceType = exportAttribute.InterfaceType,
                    ImplementType = handlerType
                });
            else
            {
                IList<RuleHandlerKeyValuePair> tempTypes = new List<RuleHandlerKeyValuePair>();
                tempTypes.Add(new RuleHandlerKeyValuePair()
                {
                    InterfaceType = exportAttribute.InterfaceType,
                    ImplementType = handlerType
                });
                ruleHandlerContainer.Add(eventType, tempTypes);
            }

            return true;
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public bool UnregisterHandler(Type handlerType)
        {
            if (handlerType == null || handlerType.IsInterface)
                return false;
            var exportAttributes = handlerType.GetCustomAttributes(typeof(RuleExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return false;

            RuleExportAttribute exportAttribute = exportAttributes[0] as RuleExportAttribute;
            if (exportAttribute == null)
                return false;
            var eventType = exportAttribute.InterfaceType.GetGenericArguments()[0];
            if (eventType == null)
                return false;

            if (ruleHandlerContainer.ContainsKey(eventType))
            {
                var deleteEventlist = ruleHandlerContainer[eventType].Where(pre => pre.ImplementType == handlerType || pre.InterfaceType == handlerType)
                                                  .ToList();

                for (int i = 0; i < deleteEventlist.Count; i++)
                {
                    ruleHandlerContainer[eventType].Remove(deleteEventlist[i]);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="eventType"></param>
        public void UnregisterHandlers(Type eventType)
        {
            if (eventType == null || eventType.IsInterface)
                return;

            if (!ruleHandlerContainer.ContainsKey(eventType))
                return;

            ruleHandlerContainer.Remove(eventType);
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

    /// <summary>
    /// 规则对象键值对
    /// </summary>
    public class RuleHandlerKeyValuePair
    {
        public string RuleName
        {
            get;
            set;
        }
        public Type InterfaceType
        {
            get;
            set;
        }
        public Type ImplementType
        {
            get;
            set;
        }
    }
}
