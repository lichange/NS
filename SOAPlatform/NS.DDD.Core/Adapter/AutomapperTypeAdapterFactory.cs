using NS.Framework.Attributes;

namespace NS.DDD.Core.Adapter
{
    using System;
    using System.Linq;
    using AutoMapper;
    using System.Reflection;
    using NS.Framework.Log;
    using NS.Framework.IOC;
    using System.Collections.Generic;

    [Export(typeof(ITypeAdapterFactory))]
    public class AutomapperTypeAdapterFactory
        : ITypeAdapterFactory
    {
        #region Constructor

        /// <summary>
        /// Create a new Automapper type adapter factory
        /// </summary>
        public AutomapperTypeAdapterFactory()
        {
            var tempAssemblys = NS.Framework.Config.PlatformConfig.ServerConfig.IOCSetting.ConfigurationItems.Select(pre => pre.Value);

            //scan all assemblies finding Automapper Profile
            var tempAllAssemblys = AppDomain.CurrentDomain.GetAssemblies().Where(pre =>
                                        !pre.GetName().Name.ToLower().EndsWith("fakes.test")
                                        && !pre.GetName().Name.ToLower().EndsWith("fakes")
                                        && !pre.GetName().Name.ToLower().StartsWith("system.")
                                        && !pre.GetName().Name.ToLower().StartsWith("microsoft.")
                                        && !pre.GetName().Name.ToLower().StartsWith("entity")
                                        && !pre.GetName().Name.ToLower().StartsWith("auto")
                                    ).ToList();

            IList<Assembly> tempAssemblyList = new List<Assembly>();
            List<string> tempAssemblyNames = new List<string>();

            foreach (var tempAssembly in tempAssemblys)
            {
                tempAssemblyNames.AddRange(tempAssembly.Split(';'));
            }

            foreach (var tempAllAssembly in tempAllAssemblys)
            {
                var tempAssemblyName = tempAllAssembly.GetName().Name;

                if (tempAssemblyNames.Where(pre => pre.Trim().ToLower().Contains(tempAssemblyName.ToLower())).Count() > 0)
                    tempAssemblyList.Add(tempAllAssembly);
            }

            //var assemblys = AppDomain.CurrentDomain
            //                        .GetAssemblies().Where((Assembly assembly) =>
            //                        {
            //                            return tempAssemblys.Where(pre => pre.Split(';').Contains(assembly.GetName().Name)).Count() > 0;
            //                            //&& !pre.GetName().Name.ToLower().EndsWith("fakes.test")
            //                            //&& !pre.GetName().Name.ToLower().EndsWith("fakes")
            //                            //&& !pre.GetName().Name.ToLower().StartsWith("system.")
            //                            //&& !pre.GetName().Name.ToLower().StartsWith("microsoft.")
            //                            //&& !pre.GetName().Name.ToLower().StartsWith("entity")
            //                            //&& !pre.GetName().Name.ToLower().StartsWith("auto")
            //                        }).ToList();
            var profiles = tempAssemblyList.SelectMany(pre => pre.GetTypes()).Where(t => t.BaseType == typeof(DomainDtoMapProfile)).Distinct().ToList();

            int count = profiles.Count();

            try
            {
                //Mapper.Initialize(cfg =>
                //{
                //    foreach (var item in profiles)
                //    {
                //        if (item.FullName != "AutoMapper.SelfProfiler`2")
                //            cfg.AddProfile(Activator.CreateInstance(item) as Profile);
                //    }
                //});
                foreach (var item in profiles)
                {
                    var tempProfile = Activator.CreateInstance(item) as DomainDtoMapProfile;
                    tempProfile.Configure();
                }
            }
            catch (Exception ex)
            {
                ILogProvider logger = ObjectContainer.CreateInstance<ILogProvider>();
                //logger.Error("初始化Automapper的过程中出现错误:" + ex.Message);
                logger.Error("初始化mapper的过程中出现错误:" + ex.Message);
            }
        }

        #endregion

        #region ITypeAdapterFactory Members

        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter();
        }

        #endregion
    }
}
