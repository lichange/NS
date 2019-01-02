using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NS.Framework.AOP.Interceptors;
using NS.Framework.Attributes;
using Unity.Registration;

namespace NS.Framework.IOC
{
    /// <summary>
    /// 对象构建器 支持多实例
    /// </summary>
    public class OfflineObjectBuilder
    {
        private readonly object lock_flag = new object();
        private readonly InternalOfflineObjectFactory factory = null;

        internal OfflineObjectBuilder(bool isInitialization)
        {
            lock (lock_flag)
            {
                if (factory == null)
                {
                    factory = new InternalOfflineObjectFactory();
                    if (isInitialization)
                        factory.InitializationFactory();
                }
            }
        }

        internal InternalOfflineObjectFactory InternalOfflineObjectFactory
        {
            get
            {
                lock (lock_flag)
                {
                    return factory;
                }
            }
        }

        public void RegisterType(Type from, Type to)
        {
            lock (lock_flag)
                factory.RegisterType(from, to);
        }

        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            lock (lock_flag)
                factory.RegisterType<TFrom, TTo>();
        }

        public void RegisterType(Type from, Type to, params InjectionMember[] injectionMembers)
        {
            lock (lock_flag)
                factory.RegisterType(from, to, injectionMembers);
        }

        public T CreateInstance<T>()
        {
            lock (lock_flag)
                return factory.CreateInstance<T>();
        }

        public void RegisterInstance(Type type, object instance)
        {
            lock (lock_flag)
                factory.RegisterInstance(type, instance);
        }

        public void RegisterInstance<T>(T instance)
        {
            lock (lock_flag)
                factory.RegisterInstance<T>(instance);
        }

        public object CreateInstance(Type type)
        {
            lock (lock_flag)
                return factory.CreateInstance(type);
        }

        public int GetRegisterCount()
        {
            lock (lock_flag)
                return factory.GetRegisterCount();
        }

        public IEnumerable<IContainerRegistration> GetRegisterItems()
        {
            lock (lock_flag)
                return factory.GetRegisterItems();
        }

        public T GetInstance<T>(object unityProxyObject) where T : class
        {
            MemberInfo[] minss = unityProxyObject.GetType().GetMembers(BindingFlags.CreateInstance |
                                                   BindingFlags.Static |
                                                  BindingFlags.NonPublic | BindingFlags.GetField
                                                  | BindingFlags.GetProperty | BindingFlags.Instance
                                                  | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.FlattenHierarchy);

            //tempService.GetType().InvokeMember("get_ClassName", BindingFlags.InvokeMethod, null, Tb, null, null, null, null).ToString();
            if (minss != null && minss.Length > 0)
            {
                var tempMemberInfo = minss.Where(pre => pre.Name == "target").FirstOrDefault();
                var tempFieldInfo = tempMemberInfo as FieldInfo;

                if (tempFieldInfo == null)
                    return default(T);

                var tempValue = tempFieldInfo.GetValue(unityProxyObject);
                return tempValue as T;
            }

            return default(T);
        }
    }
}
