using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 服务实现特性标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceExportAttribute : Attribute
    {
        private Type _interfaceType;
        public ServiceExportAttribute(Type interfaceType)
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
