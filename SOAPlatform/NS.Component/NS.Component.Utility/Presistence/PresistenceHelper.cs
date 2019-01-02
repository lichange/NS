using NS.Framework.Attributes;
using NS.Framework.IOC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NS.Component.Utility
{
    /// <summary>
    /// 平台
    /// </summary>
    public class PresistenceHelper
    {
        /// <summary>
        /// 获取对象属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException("对象实例为空！");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("属性名称不能为空！");

            return ReflectHelper.GetPropertyValueEmit(obj, propertyName);
        }

        public static bool SetPropertyValue(object obj, string propertyName,object setValue)
        {
            if (obj == null)
                throw new ArgumentNullException("对象实例为空！");
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("属性名称不能为空！");
            try
            {
                ReflectHelper.SetProperty(obj, propertyName, setValue);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 动态调用方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object InvokeMothed(string apptypeName, string methodName, params object[] parameters)
        {
            //根据服务名称，获取服务接口类型
            Type getServiceType = GetAppServiceType(apptypeName);
            if (getServiceType == null)
            {
                getServiceType = GetAppServiceType(apptypeName.Replace("Service.Implement", ""));
            }

            if (getServiceType == null)
            {
                throw new Exception(string.Format("服务器端没有找到名称：'{0}'的服务接口", apptypeName));
            }

            //根据IOC 创建本地服务实例
            var tempServiceInstance = ObjectContainer.CreateInstance(getServiceType);

            var getTypes = GetParameterTypes(parameters);

            //获取客户端调用的方法名称，查找服务器端服务实例中的方法
            var tempInvokeMethod = tempServiceInstance.GetType().GetMethod(methodName, getTypes);


            if (tempInvokeMethod == null)
                throw new Exception(string.Format("服务{0}没有找到名称：'{1}'的方法", apptypeName, methodName));

            try
            {
                //执行方法调用-可能在方法调用的过程中出现异常信息
                return tempInvokeMethod.Invoke(tempServiceInstance, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("调用服务{0}中的：'{1}'方法时出现错误:{2}", apptypeName, methodName, ex.Message));
            }
        }

        private static Type[] GetParameterTypes(params object[] paras)
        {
            if (paras == null || paras.Length == 0)
                return Type.EmptyTypes;

            IList<Type> tempTypes = new List<Type>();

            foreach (var item in paras)
            {
                if (item == null)
                    tempTypes.Add(null);
                else
                    tempTypes.Add(item.GetType());
            }

            return tempTypes.ToArray();
        }

        /// <summary>
        /// 获取待调用的应用服务类型
        /// </summary>
        /// <param name="appService">数据类型</param>
        /// <returns></returns>
        public static Type GetAppServiceType(string appService)
        {
            if (string.IsNullOrEmpty(appService))
                return null;

            var tempServiceSettings = appService.Split(';');

            //先从当前程序域中筛选，如果不存在，再通过load机制，装入内存
            var tempServiceAssemly = System.AppDomain.CurrentDomain.GetAssemblies().Where(pre => pre.GetName().Name == tempServiceSettings[0]).FirstOrDefault();

            if (tempServiceAssemly == null)
            {
                var assemblyPath = System.IO.Path.Combine(NS.Framework.Config.PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, tempServiceSettings[0]);

                if (!Directory.Exists(NS.Framework.Config.PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath))
                {
                    assemblyPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, tempServiceSettings[0]);
                }

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return null;

                //装载程序集
                tempServiceAssemly = System.Reflection.Assembly.LoadFrom(assemblyPath);
            }

            if (tempServiceAssemly == null)
                return null;

            var tempImplementType = tempServiceAssemly.GetType(tempServiceSettings[1]);

            if (tempImplementType == null)
                return tempImplementType;

            var tempTypeExportAttributes = tempImplementType.GetCustomAttributes(typeof(ExportAttribute), false);
            if (tempTypeExportAttributes.Length == 0)
                return null;

            var tempTypeExportAttribute = tempTypeExportAttributes[0] as ExportAttribute;

            return tempTypeExportAttribute == null ? null : tempTypeExportAttribute.InterfaceType;
        }

        /// <summary>
        /// 获取待调用的应用服务类型
        /// </summary>
        /// <param name="appService">数据类型</param>
        /// <returns></returns>
        public static Type GetDomainType(string domainType)
        {
            if (string.IsNullOrEmpty(domainType))
                return null;

            var tempServiceSettings = domainType.Split(';');

            //先从当前程序域中筛选，如果不存在，再通过load机制，装入内存
            var tempServiceAssemly = System.AppDomain.CurrentDomain.GetAssemblies().Where(pre => pre.GetName().Name == tempServiceSettings[0]).FirstOrDefault();

            if (tempServiceAssemly == null)
            {
                var assemblyPath = System.IO.Path.Combine(NS.Framework.Config.PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath, tempServiceSettings[0]);

                if (!Directory.Exists(NS.Framework.Config.PlatformConfig.ServerConfig.IOCSetting.AssemblyFilePath))
                {
                    assemblyPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, tempServiceSettings[0]);
                }

                //判断程序集是否存在
                if (!System.IO.File.Exists(assemblyPath))
                    return null;

                //装载程序集
                tempServiceAssemly = System.Reflection.Assembly.LoadFrom(assemblyPath);
            }

            if (tempServiceAssemly == null)
                return null;

            var tempImplementType = tempServiceAssemly.GetType(tempServiceSettings[1]);

            return tempImplementType;
        }
    }
}
