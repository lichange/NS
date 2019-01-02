using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NS.Framework.Attributes;
using NS.Framework.Utility.Collections;
using Unity.Registration;

namespace NS.Framework.IOC
{
    internal class InternalEventHandlerFactory
    {
        #region 构造函数

        private static readonly object lock_flag = new object();
        public static InternalEventHandlerFactory factory = null;
        private static IDictionary<Type, IList<EventHandlerKeyValuePair>> eventHandlerContainer = new Dictionary<Type, IList<EventHandlerKeyValuePair>>();

        public static InternalEventHandlerFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lock_flag)
                    {
                        if (factory == null)
                            factory = new InternalEventHandlerFactory();
                    }
                }
                return factory;
            }
        }

        static InternalEventHandlerFactory()
        {
            //_objectBuilder = ObjectContainer.CreateObjectBuilder(true);

            //var configItems = Config.PlatformConfig.ServerConfig.IOCSetting.ConfigurationItems;

            //if (configItems != null && configItems.Count > 0)
            //{
            //    foreach (var objectConfiguration in configItems)
            //    {
            //        if (objectConfiguration.Key.ToLower() != "event")
            //            continue;
            //        RegisterTypeOrInstanceToUnity(objectConfiguration);
            //    }
            //}
            //else
            //{
            //    //从当前制定的路径下检索
            //    var searchPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,
            //                                            Config.PlatformConfig.ServerConfig.IOCSetting.SearchPath);
            //    if (!System.IO.Directory.Exists(searchPath))
            //        return;

            //    LoadPath(searchPath);
            //    //TODO...
            //}
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

                var assemblyNames = objectConfiguration.Value.Split(';');

                if (assemblyNames.Length == 0)
                    return;

                foreach (var assemblyName in assemblyNames)
                {
                    if (string.IsNullOrEmpty(assemblyName))
                        continue;

                    RegisterAssemblyEventHandler(assemblyName, objectConfiguration.LifeCycle);
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        private static void RegisterAssemblyEventHandler(string name, string lifeCycle)
        {
            string tempAssemblyName = name;
            if (tempAssemblyName.LastIndexOf(".dll") < 0)
                tempAssemblyName += ".dll";

            System.Reflection.Assembly assembly = System.AppDomain.CurrentDomain.GetAssemblies()
                  .Where(pre => pre.GetName(false).Name == name)
                  .FirstOrDefault();

            if (assembly == null)
            {
                string assemblyPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, tempAssemblyName);

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return;

                //装载程序集
                assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
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
                RegisterType(type, lifeCycle);
            }
        }
        #endregion

        internal static void RegisterType(Type type, string lifeCycle)
        {
            var exportAttributes = type.GetCustomAttributes(typeof(EventExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return;

            EventExportAttribute exportAttribute = exportAttributes[0] as EventExportAttribute;

            if (exportAttribute.EventType == null)
                return;

            var eventType = exportAttribute.EventType;

            switch (GetObjectLifeCycle(lifeCycle))
            {
                case ObjectLifeCycle.Singleton:
                    ObjectContainer.RegisterInstance(exportAttribute.InterfaceType, Activator.CreateInstance(type));

                    if (eventHandlerContainer.ContainsKey(eventType))
                        eventHandlerContainer[eventType].Add(new EventHandlerKeyValuePair()
                            {
                                InterfaceType = exportAttribute.InterfaceType,
                                ImplementType = type
                            });
                    else
                    {
                        IList<EventHandlerKeyValuePair> tempTypes = new List<EventHandlerKeyValuePair>();
                        tempTypes.Add(new EventHandlerKeyValuePair()
                        {
                            InterfaceType = exportAttribute.InterfaceType,
                            ImplementType = type
                        });
                        eventHandlerContainer.Add(eventType, tempTypes);
                    }
                    break;
                case ObjectLifeCycle.New:
                    if (eventHandlerContainer.ContainsKey(eventType))
                    {
                        if (eventHandlerContainer[eventType].Where(pre => pre.ImplementType == type).Count() > 0)
                            return;

                        eventHandlerContainer[eventType].Add(new EventHandlerKeyValuePair()
                             {
                                 InterfaceType = exportAttribute.InterfaceType,
                                 ImplementType = type
                             });
                    }
                    else
                    {
                        IList<EventHandlerKeyValuePair> tempTypes = new List<EventHandlerKeyValuePair>();
                        tempTypes.Add(new EventHandlerKeyValuePair()
                        {
                            InterfaceType = exportAttribute.InterfaceType,
                            ImplementType = type
                        });
                        eventHandlerContainer.Add(eventType, tempTypes);
                    }

                    ObjectContainer.RegisterType(exportAttribute.InterfaceType, type);

                    ObjectContainer.RegisterType(type, type);

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

        public IList<object> GetEventHandlers<T>()
        {
            IList<object> tResults = new List<object>();
            lock (lock_flag)
            {
                if (eventHandlerContainer.ContainsKey(typeof(T)))
                {
                    IList<EventHandlerKeyValuePair> eventHandlerKeyValuePairs = eventHandlerContainer[typeof(T)];

                    foreach (var eventHandlerKeyValuePair in eventHandlerKeyValuePairs)
                    {
                        tResults.Add(ObjectContainer.CreateInstance(eventHandlerKeyValuePair.InterfaceType));
                    }

                }
            }
            return tResults;
        }

        public IList<object> GetEventHandlers(Type eventType)
        {
            IList<object> tResults = new List<object>();
            lock (lock_flag)
            {
                if (eventHandlerContainer.ContainsKey(eventType))
                {
                    IList<EventHandlerKeyValuePair> eventHandlerKeyValuePairs = eventHandlerContainer[eventType];

                    foreach (var eventHandlerKeyValuePair in eventHandlerKeyValuePairs)
                    {
                        tResults.Add(ObjectContainer.CreateInstance(eventHandlerKeyValuePair.InterfaceType));
                    }

                }
            }
            return tResults;
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

            RegisterType(handlerType, "new");

            //var exportAttributes = handlerType.GetCustomAttributes(typeof(EventExportAttribute), false);

            //if (exportAttributes == null || exportAttributes.Length == 0)
            //    return false;

            //EventExportAttribute exportAttribute = exportAttributes[0] as EventExportAttribute;
            //if (exportAttribute == null)
            //    return false;
            //var eventType = exportAttribute.InterfaceType.GetGenericArguments()[0];
            //if (eventType == null)
            //    return false;

            //if (eventHandlerContainer.ContainsKey(eventType))
            //    eventHandlerContainer[eventType].Add(new EventHandlerKeyValuePair()
            //    {
            //        InterfaceType = exportAttribute.InterfaceType,
            //        ImplementType = handlerType
            //    });
            //else
            //{
            //    IList<EventHandlerKeyValuePair> tempTypes = new List<EventHandlerKeyValuePair>();
            //    tempTypes.Add(new EventHandlerKeyValuePair()
            //    {
            //        InterfaceType = exportAttribute.InterfaceType,
            //        ImplementType = handlerType
            //    });
            //    eventHandlerContainer.Add(eventType, tempTypes);
            //}

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
            var exportAttributes = handlerType.GetCustomAttributes(typeof(EventExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return false;

            EventExportAttribute exportAttribute = exportAttributes[0] as EventExportAttribute;
            if (exportAttribute == null)
                return false;
            var eventType = exportAttribute.InterfaceType.GetGenericArguments()[0];
            if (eventType == null)
                return false;

            if (eventHandlerContainer.ContainsKey(eventType))
            {
                var deleteEventlist = eventHandlerContainer[eventType].Where(pre => pre.ImplementType == handlerType || pre.InterfaceType == handlerType)
                                                  .ToList();

                for (int i = 0; i < deleteEventlist.Count; i++)
                {
                    eventHandlerContainer[eventType].Remove(deleteEventlist[i]);
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

            if (!eventHandlerContainer.ContainsKey(eventType))
                return;

            eventHandlerContainer.Remove(eventType);
        }

        public int GetRegisterCount()
        {
            lock (lock_flag)
                return eventHandlerContainer.Count();
        }

        public IEnumerable<ContainerRegistration> GetRegisterItems()
        {
            lock (lock_flag)
                return null;
        }
    }

    /// <summary>
    /// 事件对象键值对
    /// </summary>
    public class EventHandlerKeyValuePair
    {
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
