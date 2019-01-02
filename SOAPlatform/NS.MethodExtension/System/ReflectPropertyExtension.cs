using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 属性反射扩展方法
    /// </summary>
    public static class ReflectPropertyExtension
    {
        public static object GetPropertyValue(this object instance, string PropertyName)
        {
            if (PropertyName == null)
            {
                return null;
            }
            Type type = instance.GetType();
            System.Reflection.PropertyInfo property = type.GetProperty(PropertyName);
            if (property == null)
            {
                return null;
            }
            System.Reflection.MethodInfo method = property.GetGetMethod();
            if (method == null)
            {
                return null;
            }
            return method.Invoke(instance, null);
        }

        public static bool SetPropertyValue(this object instance,string PropertyName, object PropertyValue)
        {
            if (PropertyName == null)
            {
                return false;
            }
            Type type = instance.GetType();
            System.Reflection.PropertyInfo property = type.GetProperty(PropertyName);
            if (property == null)
            {
                return false;
            }
            System.Reflection.MethodInfo method = property.GetSetMethod();
            if (method == null)
            {
                return false;
            }
            method.Invoke(instance, new object[] { PropertyValue });
            return true;
        }
    }
}
