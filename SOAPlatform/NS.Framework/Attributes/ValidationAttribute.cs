using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-标记该特性的对象，才会执行验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
   public class ValidationAttribute : Attribute
    {
        public ValidationAttribute()
        {
        }
    }
}
