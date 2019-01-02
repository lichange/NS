using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NS.Framework.AOP.Interceptors;
using NS.Framework.Attributes;
using NS.Framework.Utility.Collections;

namespace NS.Framework.IOC
{
    internal class InternalLogicValidatorFactory
    {
        #region 构造函数

        private static readonly object lock_flag = new object();
        public static InternalLogicValidatorFactory factory = null;
        private static ThreadSafeDictionary<Type, IList<ValidatorMethodMapping>> methodMappingContainer = new ThreadSafeDictionary<Type, IList<ValidatorMethodMapping>>();
        private static ThreadSafeList<Type> validatorInstanceTypeContainer = new ThreadSafeList<Type>();

        public static InternalLogicValidatorFactory Instance
        {
            get
            {
                if (factory == null)
                {
                    lock (lock_flag)
                    {
                        if (factory == null)
                            factory = new InternalLogicValidatorFactory();
                    }
                }
                return factory;
            }
        }

        static InternalLogicValidatorFactory()
        {
            //var configItems = Config.PlatformConfig.ServerConfig.IOCSetting.ConfigurationItems;

            //if (configItems != null && configItems.Count > 0)
            //{
            //    foreach (var objectConfiguration in configItems)
            //    {
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

        private static string[] assemblyFilters = new string[] { "AutoMapper", "Iesi.Collections", "Microsoft.Practices.EnterpriseLibrary.Common", "Microsoft.Practices.EnterpriseLibrary.Validation", "Microsoft.Practices.ServiceLocation", 
            "Microsoft.Practices.Unity.Configuration", "Microsoft.Practices.Unity", "Microsoft.Practices.Unity.Interception","NHibernate","" };

        private static void LoadPath(string searchPath)
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

                //获取程序集名称，判断是否已经有后缀
                string assemblyName = objectConfiguration.Value;
                if (assemblyName.LastIndexOf(".dll") < 0)
                    assemblyName += ".dll";

                string assemblyPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, assemblyName);

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return;

                //装载程序集
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);

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
                    RegisterType(type, objectConfiguration.LifeCycle);
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        private static void RegisterType(Type type, string lifeCycle)
        {
            var exportAttributes = type.GetCustomAttributes(typeof(ExportAttribute), false);

            if (exportAttributes == null || exportAttributes.Length == 0)
                return;

            ExportAttribute exportAttribute = exportAttributes[0] as ExportAttribute;

            //string[] filterArray = new string[] { "IEnumerable" };
            //Type[] objectInterfaces = interfaces.Where(pre => !filterArray.Contains(pre.Name) && !(pre.IsGenericType)).ToArray();

            //反射类型中包含的所有非系统的方法，判断方法中是否带有特性，如果参数中不包含特性，则不处理
            var allMethods = exportAttribute.InterfaceType.GetMethods(System.Reflection.BindingFlags.Public);

            if (methodMappingContainer.ContainsKey(type))
            {
                IList<ValidatorMethodMapping> methodMappings = methodMappingContainer[type];

                //添加当前方法对应的映射元数据信息
                foreach (var method in allMethods)
                {
                    ValidatorMethodMapping methodMapping = new ValidatorMethodMapping(method);

                    if (methodMapping.MethodParameters != null && methodMapping.MethodParameters.Length > 0)
                        methodMappings.Add(methodMapping);
                }
            }
            else
            {
                IList<ValidatorMethodMapping> methodMappings = new List<ValidatorMethodMapping>();

                foreach (var method in allMethods)
                {
                    ValidatorMethodMapping methodMapping = new ValidatorMethodMapping(method);

                    if (methodMapping.MethodParameters != null && methodMapping.MethodParameters.Length > 0)
                        methodMappings.Add(methodMapping);
                }

                methodMappingContainer.Add(type, methodMappings);
            }
        }

        public void RegisterType(Type type)
        {
            //string[] filterArray = new string[] { "IEnumerable" };
            //Type[] objectInterfaces = interfaces.Where(pre => !filterArray.Contains(pre.Name) && !(pre.IsGenericType)).ToArray();

            //反射类型中包含的所有非系统的方法，判断方法中是否带有特性，如果参数中不包含特性，则不处理
            var allMethods = type.GetMethods();

            if (allMethods.Length == 0)
                return;

            if (methodMappingContainer.ContainsKey(type))
            {
                IList<ValidatorMethodMapping> methodMappings = methodMappingContainer[type];

                //添加当前方法对应的映射元数据信息
                foreach (var method in allMethods)
                {
                    ValidatorMethodMapping methodMapping = new ValidatorMethodMapping(method);

                    if (methodMapping.MethodParameters != null && methodMapping.MethodParameters.Length > 0)
                        methodMappings.Add(methodMapping);
                }
            }
            else
            {
                IList<ValidatorMethodMapping> methodMappings = new List<ValidatorMethodMapping>();

                foreach (var method in allMethods)
                {
                    ValidatorMethodMapping methodMapping = new ValidatorMethodMapping(method);

                    if (methodMapping.MethodParameters != null && methodMapping.MethodParameters.Length > 0)
                        methodMappings.Add(methodMapping);
                }

                if (methodMappings.Count > 0)
                    methodMappingContainer.Add(type, methodMappings);
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

        public object CreateInstance(System.Reflection.MethodInfo methodInfo, System.Reflection.ParameterInfo parmeterInfo)
        {
            lock (lock_flag)
            {
                ValidatorMethodMapping mapping = new ValidatorMethodMapping(methodInfo);

                if (!methodMappingContainer.ContainsKey(methodInfo.DeclaringType))
                    return null;

                var mappings = methodMappingContainer[methodInfo.DeclaringType];

                var methodMapping = mappings.Where(pre => pre == mapping).FirstOrDefault();

                if (methodMapping.MethodParameters != null && methodMapping.MethodParameters.Length > 0)
                {
                    var parameterMapping = methodMapping.MethodParameters.Where(pre => pre.ParameterInfo.ParameterType == parmeterInfo.ParameterType && pre.ParameterInfo.Position == parmeterInfo.Position).FirstOrDefault();

                    if (parameterMapping == null || parameterMapping.ValidatorType == null)
                        return null;

                    //如果不包含则处理所有的内容
                    if (!validatorInstanceTypeContainer.Contains(parameterMapping.ValidatorType))
                        ObjectContainer.RegisterInstance(parameterMapping.ValidatorType, ObjectContainer.CreateInstance(parameterMapping.ValidatorType));

                    return ObjectContainer.CreateInstance(parameterMapping.ValidatorType);
                }

                return null;
            }
        }
    }

    /// <summary>
    /// 方法的完整元数据信息-主要提供对方法匹配的查找
    /// </summary>
    public class ValidatorMethodMapping
    {
        public ValidatorMethodMapping(System.Reflection.MethodInfo methodInfo)
        {
            this.ClassType = methodInfo.DeclaringType;
            this.MethodName = methodInfo.Name;
            this.MethodParameters = this.GetMethodMappingParameters(methodInfo);

        }

        private ValidatorParameterMapping[] GetMethodMappingParameters(System.Reflection.MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters == null || parameters.Length == 0)
                return null;

            IList<ValidatorParameterMapping> validatorMappings = new List<ValidatorParameterMapping>();

            foreach (var parameter in parameters)
            {
                ValidatorParameterMapping parameterMapping = new ValidatorParameterMapping();

                //设置属性
                parameterMapping.ParameterInfo = parameter;

                var parameterAttributes = Attribute.GetCustomAttributes(parameter);

                if (parameterAttributes.Where(pre => pre.GetType() == typeof(ParameterValidationAttribute)).Count()>0)
                {
                    var paraAttribute = parameterAttributes.Length > 0 ? ((ParameterValidationAttribute)parameterAttributes[0]) : null;

                    //设置参数执行的业务逻辑
                    parameterMapping.ValidatorType = paraAttribute != null ? paraAttribute.HandlerType : null;

                    validatorMappings.Add(parameterMapping);
                }
            }

            return validatorMappings.ToArray();
        }

        public Type ClassType
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }

        public ValidatorParameterMapping[] MethodParameters
        {
            get;
            set;
        }

        public static bool operator ==(ValidatorMethodMapping mapping1, ValidatorMethodMapping mapping2)
        {
            if (mapping1.ClassType != mapping2.ClassType)
                return false;

            if (mapping1.MethodName != mapping2.MethodName)
                return false;

            if (mapping1.MethodParameters.Length != mapping2.MethodParameters.Length)
                return false;

            for (int i = 0; i < mapping1.MethodParameters.Length; i++)
            {
                if (mapping1.MethodParameters[i].ParameterInfo.ParameterType != mapping2.MethodParameters[i].ParameterInfo.ParameterType)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(ValidatorMethodMapping mapping1, ValidatorMethodMapping mapping2)
        {
            if (mapping1.ClassType != mapping2.ClassType)
                return true;

            if (mapping1.MethodName != mapping2.MethodName)
                return true;

            if (mapping1.MethodParameters.Length != mapping2.MethodParameters.Length)
                return true;

            for (int i = 0; i < mapping1.MethodParameters.Length; i++)
            {
                if (mapping1.MethodParameters[i].ParameterInfo.ParameterType != mapping2.MethodParameters[i].ParameterInfo.ParameterType)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    /// <summary>
    /// 参数对象与验证器的映射关系
    /// </summary>
    public class ValidatorParameterMapping
    {
        /// <summary>
        /// 参数对象
        /// </summary>
        public System.Reflection.ParameterInfo ParameterInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 当前参数类型对应的验证器
        /// </summary>
        public Type ValidatorType
        {
            get;
            set;
        }
    }
}
