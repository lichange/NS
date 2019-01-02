using NS.DDD.Core;
using NS.DDD.Core.AggregateIdGenerators;
using NS.Framework.IOC;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data
{
    public class AggregateIdGeneratorFactory
    {
        /// <summary>
        /// 根据类型获取类型对应的主键生成器类型
        /// </summary>
        /// <param name="type">待持久化的对象类型</param>
        /// <returns></returns>
        public static IAggregateIdGenerator GetGenerator(Type type)
        {
            if (type == null)
                throw new RepositoryException("在获取主键构建器时的聚合根类型不能为空");

            if (NS.Framework.Config.PlatformConfig.ServerConfig.DomainSetting.AggregateIdGeneratorItemStrategys.Where(pre => pre.AggregateTypeName == type.Name
                || pre.AggregateTypeName == type.FullName).Count() > 0)
            {
                var tempStrategys = NS.Framework.Config.PlatformConfig.ServerConfig.DomainSetting.AggregateIdGeneratorItemStrategys.Where(pre => pre.AggregateTypeName == type.Name
                || pre.AggregateTypeName == type.FullName).ToList();

                if (!string.IsNullOrEmpty(tempStrategys[0].Generator))
                {
                    var generatorType = Type.GetType(tempStrategys[0].Generator);

                    if (generatorType == null)
                        throw new RepositoryException(string.Format("没有找到配置的主键生成器类型，请确认配置是否正确,配置节名称:{0}", tempStrategys[0].AggregateTypeName));

                    return (IAggregateIdGenerator)ObjectContainer.CreateInstance(generatorType);
                }
            }

            var generatorFullType=string.Empty;
            var tempAssemblyName=string.Empty;

            if (NS.Framework.Config.PlatformConfig.ServerConfig.DomainSetting.AggregateIdGeneratorItems.Count > 0)
            {
                var tempItems = NS.Framework.Config.PlatformConfig.ServerConfig.DomainSetting.AggregateIdGeneratorItems;
                string assemblyName = type.Assembly.GetName().Name;

                foreach (var tempItem in tempItems)
                {
                    if (string.IsNullOrEmpty(tempItem.GeneratorKey) || string.IsNullOrEmpty(tempItem.GeneratorValue))
                        continue;

                    if (tempItem.AggregateIdGeneratorAssemblys.Count == 0)
                        continue;

                    var tempSelectItem=tempItem.AggregateIdGeneratorAssemblys.Where(pre => pre.AssemblyName == assemblyName).FirstOrDefault();

                    if (tempSelectItem==null)
                        continue;

                    generatorFullType = tempItem.GeneratorValue.Split(';')[1];
                    tempAssemblyName = tempItem.GeneratorValue.Split(';')[0];
                }
            }

            if (!string.IsNullOrEmpty(generatorFullType))
            {
                var generatorTypeAssembly = System.AppDomain.CurrentDomain.GetAssemblies().Where(Predicate => Predicate.GetName().Name == tempAssemblyName).FirstOrDefault();

                var generatorType = generatorTypeAssembly.GetTypes().Where(Predicate=>Predicate.FullName==generatorFullType).FirstOrDefault();

                if (generatorType == null)
                    throw new RepositoryException(string.Format("没有找到配置的主键生成器类型，请确认配置是否正确,配置节名称:{0}", generatorFullType));

                return (IAggregateIdGenerator)ObjectContainer.CreateInstance(generatorType);
            }

            return ObjectContainer.CreateInstance<IGuidGenerator>();
        }
    }
}
