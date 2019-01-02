using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExportAttribute : Attribute
    {
        private Type _interfaceType;
        public ExportAttribute(Type interfaceType)
        {
            this._interfaceType = interfaceType;
        }

        public Type InterfaceType
        {
            get
            {
                return this._interfaceType;
            }
        }
    }
}
