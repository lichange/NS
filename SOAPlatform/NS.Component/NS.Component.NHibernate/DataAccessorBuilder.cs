using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NS.Component.Data
{
    public class DataAccessorBuilder
    {
        private static IDataAccessor DataAccessor = null;
        private static readonly object lock_flag = new object();
        public static IDataAccessor CreateDataAccessor(params Assembly[] assemblys)
        {
            if (DataAccessor == null)
            {
                lock (lock_flag)
                {
                    if (DataAccessor == null)
                    {
                        DataAccessorConfiguration config = new DataAccessorConfiguration(); // TODO: 初始化为适当的值
                        config.MappingsAssemblies = new List<System.Reflection.Assembly>();
                        config.MappingsAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
                        config.MappingsAssemblies.AddRange(assemblys);

                        if (NS.Framework.Config.PlatformConfig.ServerConfig.ConfigFilePath.EndsWith("bin"))
                            config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", "hibernate.cfg.xml");
                        else
                            config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "hibernate.cfg.xml");

                        DataAccessorFactory target = new DataAccessorFactory(config);

                        DataAccessor = target.CreateDataAccessor();

                    }
                }
            }

            return DataAccessor;
        }

        public static IDataAccessor CreateDataAccessor()
        {
            if (DataAccessor == null)
            {
                lock (lock_flag)
                {
                    if (DataAccessor == null)
                    {
                        DataAccessorConfiguration config = new DataAccessorConfiguration(); // TODO: 初始化为适当的值
                        config.MappingsAssemblies = new List<System.Reflection.Assembly>();
                        config.MappingsAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
                        config.MappingsAssemblies.AddRange(NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.GetMappingAssemblys);
                        config.ConfigFile = NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConfigFile;

                        DataAccessorFactory target = new DataAccessorFactory(config);

                        DataAccessor = target.CreateDataAccessor();

                    }
                }
            }

            return DataAccessor;
        }

        public static IDataAccessor CreateDataAccessor(string connectionString)
        {
            if (DataAccessor == null || DataAccessor.ContextName != connectionString)
            {
                lock (lock_flag)
                {
                    if (DataAccessor == null || DataAccessor.ContextName != connectionString)
                    {
                        DataAccessorConfiguration config = new DataAccessorConfiguration(); // TODO: 初始化为适当的值
                        config.MappingsAssemblies = new List<System.Reflection.Assembly>();
                        config.MappingsAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly());
                        config.MappingsAssemblies.AddRange(NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.GetMappingAssemblys);
                        var configFileName = string.Format("{0}-{1}", connectionString, NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.ConfigFile);

                        if (NS.Framework.Config.PlatformConfig.ServerConfig.ConfigFilePath.EndsWith("bin"))
                            config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin", configFileName);
                        else
                            config.ConfigFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, configFileName);

                        DataAccessorFactory target = new DataAccessorFactory(config);

                        DataAccessor = target.CreateDataAccessor();
                        DataAccessor.ContextName = connectionString;
                    }
                }
            }

            return DataAccessor;
        }
    }
}
