
//using System.ServiceModel;
//using System.ServiceModel.Channels;
//using System.ServiceModel.Dispatcher;
//using System.Collections.Generic;
//using System.Linq;
//using NS.Component.Data;

//namespace NS.Component.NHibernate
//{
//    public class NHibernateContextInitializer : IDispatchMessageInspector
//    {
//        public void Initialize(InstanceContext instanceContext, Message message)
//        {
//            DataAccessorConfiguration config = new DataAccessorConfiguration(); // TODO: 初始化为适当的值
//            config.MappingsAssemblies = new List<System.Reflection.Assembly>();
//            config.MappingsAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
//            config.MappingsAssemblies.AddRange(NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.GetMappingAssemblys);
//            var configFileName = string.Format("{0}-{1}", "car", NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConfigFile);

//            if (NS.Framework.Config.PlatformConfig.ServerConfig.ConfigFilePath.EndsWith("bin"))
//                config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", configFileName);
//            else
//                config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, configFileName);

//            NHibernateFactory.Initialize(config);
//            instanceContext.Extensions.Add(
//                new NHibernateContextExtension(
//                    NHibernateFactory.OpenSession()
//                    )
//                );
//        }
//        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
//        {
//            instanceContext.Extensions.Add(new NHibernateContextExtension(NHibernateFactory.OpenSession()));
//            return null;
//        }

//        public void BeforeSendReply(ref Message reply, object correlationState)
//        {
//            var extensions = OperationContext.Current.InstanceContext.Extensions.FindAll<NHibernateContextExtension>();

//            foreach (var extension in extensions)
//            {
//                OperationContext.Current.InstanceContext.Extensions.Remove(extension);
//            }

//            var errorHandlers = new List<IErrorHandler>(OperationContext.Current.EndpointDispatcher.ChannelDispatcher.ErrorHandlers.
//                                            Where(h => h.GetType() == typeof(NHibernateAutoRollbackErrorHandler)));

//            foreach (var errorHandler in errorHandlers)
//            {
//                OperationContext.Current.EndpointDispatcher.ChannelDispatcher.ErrorHandlers.Remove(errorHandler);
//            }


//        }

        
//    }
//}