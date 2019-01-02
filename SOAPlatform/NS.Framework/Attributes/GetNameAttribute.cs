using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 标记需要自动从ID转换为Name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GetNameAttribute : Attribute
    {
        public GetNameAttribute()
        {
        }
    }
}
