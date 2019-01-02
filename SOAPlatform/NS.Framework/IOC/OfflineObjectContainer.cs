using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Registration;

namespace NS.Framework.IOC
{
    /// <summary>
    /// IOC对象容器-离线应用支持-在线应用服务不从该容器中调用
    /// </summary>
    public class OfflineObjectContainer
    {
        private static readonly object lock_flag = new object();
        private static InternalOfflineObjectFactory _factory = null;
        private static InternalOfflineObjectFactory _cloneFactory = null;
        private static InternalOfflineObjectFactory Instance
        {
            get
            {
                if (_factory == null)
                {
                    lock (lock_flag)
                    {
                        if (_factory == null)
                        {
                            _factory = new InternalOfflineObjectFactory();
                            _factory.InitializationFactory();

                            _cloneFactory = _factory;
                        }
                    }
                }
                return _factory;
            }
        }

        /// <summary>
        /// 设置当前IOC容器实例-替换默认的IOC容器
        /// </summary>
        /// <param name="objectBuilder"></param>
        public static void SetCurrent(OfflineObjectBuilder objectBuilder)
        {
            _factory = objectBuilder.InternalOfflineObjectFactory;
        }

        /// <summary>
        /// 重置IOC容器到初始状态
        /// </summary>
        public static void Reset()
        {
            _factory = _cloneFactory;
        }

        public static T CreateInstance<T>()
        {
            return Instance.CreateInstance<T>();
        }

        public static void RegisterInstance(Type type, object instance)
        {
            Instance.RegisterInstance(type, instance);
        }

        public static void RegisterInstance<T>(T instance)
        {
            Instance.RegisterInstance<T>(instance);
        }

        public static void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            Instance.RegisterType<TFrom, TTo>();
        }

        public static void RegisterType<TFrom, TTo>(params InjectionMember[] injectedMemberses) where TTo : TFrom
        {
            Instance.RegisterType<TFrom, TTo>(injectedMemberses);
        }

        public static void RegisterType(Type fromType, Type toType)
        {
            Instance.RegisterType(fromType, toType);
        }

        public static bool IsRegisterType(Type isRegister)
        {
            return true;
        }

        public static object CreateInstance(Type type)
        {
            return Instance.CreateInstance(type);
        }

        public static int GetRegisterCount()
        {
            return Instance.GetRegisterCount();
        }

        public static IEnumerable<IContainerRegistration> GetRegisterItems()
        {
            return Instance.GetRegisterItems();
        }

        public static OfflineObjectBuilder CreateObjectBuilder(bool isInitilization = false)
        {
            return new OfflineObjectBuilder(isInitilization);
        }
    }
}
