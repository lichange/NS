﻿using System;
using System.Reflection.Emit;
using System.Reflection;
using EmitMapper.Mappers;

namespace EmitMapper
{
    /// <summary>
    /// Class which maintains an assembly for created object Mappers
    /// </summary>
    public class DynamicAssemblyManager
    {
        /// <summary>
        /// 已废弃
        /// Saves assembly with created Mappers to file. This method is useful for debugging purpose.
        /// </summary>
        public static void SaveAssembly()
        {
#if !SILVERLIGHT
            lock (typeof(DynamicAssemblyManager))
            {
                throw new Exception(".net standard 2.0 不支持assemblyBuilder.Save 方法");
                //assemblyBuilder.Save(assemblyName.Name + ".dll");
            }
#else
                      throw new NotImplementedException("DynamicAssemblyManager.SaveAssembly");
#endif
        }

        #region Non-public members

        private static AssemblyName assemblyName;
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;

        static DynamicAssemblyManager()
        {
#if !SILVERLIGHT
            assemblyName = new AssemblyName("EmitMapperAssembly");
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run
                );
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            assemblyName = new AssemblyName("EmitMapperAssembly.SL");
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                  assemblyName,
                  AssemblyBuilderAccess.Run
                  );
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
#endif
        }

        private static string CorrectTypeName(string typeName)
        {
            if (typeName.Length >= 1042)
            {
                typeName = "type_" + typeName.Substring(0, 900) + Guid.NewGuid().ToString().Replace("-", "");
            }
            return typeName;
        }

        internal static TypeBuilder DefineMapperType(string typeName)
        {
            lock (typeof(DynamicAssemblyManager))
            {
                return moduleBuilder.DefineType(
                    CorrectTypeName(typeName + Guid.NewGuid().ToString().Replace("-", "")),
                    TypeAttributes.Public,
                    typeof(MapperForClassImpl),
                    null
                    );
            }
        }

        internal static TypeBuilder DefineType(string typeName, Type parent)
        {
            lock (typeof(DynamicAssemblyManager))
            {
                return moduleBuilder.DefineType(
                    CorrectTypeName(typeName),
                    TypeAttributes.Public,
                    parent,
                    null
                    );
            }
        }
        #endregion
    }
}