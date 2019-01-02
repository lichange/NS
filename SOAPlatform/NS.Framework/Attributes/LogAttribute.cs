using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-标记该特性的方法的全过程会被自动执行log
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class LogAttribute : Attribute
    {
        public LogAttribute()
        {
        }
    }
}
