﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    internal class AttributeProvider
    {
        /// <summary>
        /// 线程安全的字典集合，该集合负责保存属性和属性上的验证标记的映射关系
        /// </summary>
        private readonly ConcurrentDictionary<PropertyInfo, IEnumerable<Attribute>> _discoveredAttributes =
            new ConcurrentDictionary<PropertyInfo, IEnumerable<Attribute>>();

        public virtual IEnumerable<Attribute> GetAttributes(MemberInfo memberInfo)
        {
            DebugCheck.NotNull(memberInfo);

            var type = memberInfo as Type;

            if (type != null)
            {
                return GetAttributes(type);
            }

            return GetAttributes((PropertyInfo)memberInfo);
        }

        public virtual IEnumerable<Attribute> GetAttributes(Type type)
        {
            DebugCheck.NotNull(type);

            var attrs = new HashSet<Attribute>(type.GetCustomAttributes(true).Cast<Attribute>());

            // Data Services workaround
            foreach (var attribute in type.GetCustomAttributes(true).Cast<Attribute>()
                                          .Where(
                                              a =>
                                              a.GetType().FullName.Equals(
                                                  "System.Data.Services.Common.EntityPropertyMappingAttribute", StringComparison.Ordinal) &&
                                              !attrs.Contains(a)))
            {
                attrs.Add(attribute);
            }

            return attrs;
        }

        public virtual IEnumerable<Attribute> GetAttributes(PropertyInfo propertyInfo)
        {
            DebugCheck.NotNull(propertyInfo);

            return _discoveredAttributes.GetOrAdd(
                propertyInfo, (pi) =>
                {
                    //var typeDescriptor = GetTypeDescriptor(propertyInfo.DeclaringType);
                    var propertyCollection = TypeDescriptor.GetProperties(propertyInfo.DeclaringType); // typeDescriptor.GetProperties();
                    var propertyDescriptor = propertyCollection[propertyInfo.Name];

                    var propertyAttributes
                        = (propertyDescriptor != null)
                                ? propertyDescriptor.Attributes.Cast<Attribute>()
                        // Fallback to standard reflection (non-public properties)
                                : propertyInfo.GetCustomAttributes(true).Cast<Attribute>();

                    // Get the attributes for the property's type and exclude them
                    var propertyTypeAttributes = GetAttributes(propertyInfo.PropertyType);
                    return propertyAttributes.Except(propertyTypeAttributes);
                });
        }

        //private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        //{
        //    DebugCheck.NotNull(type);

        //    return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        //}

        public virtual void ClearCache()
        {
            _discoveredAttributes.Clear();
        }
    }
}
