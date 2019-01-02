using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-用于业务规则的标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RuleExportAttribute : Attribute
    {
        private Type _interfaceType;
        private int tempOrder;
        public RuleExportAttribute(Type interfaceType)
        {
            this._interfaceType = interfaceType;
            tempOrder = -1;
        }

        public RuleExportAttribute(Type interfaceType, int order)
        {
            this._interfaceType = interfaceType;
            this.tempOrder = order;
        }

        /// <summary>
        /// 规则执行的顺序
        /// </summary>
        public int Order
        {
            get
            {
                return this.tempOrder;
            }
        }

        /// <summary>
        /// 接口类型
        /// </summary>
        public Type InterfaceType
        {
            get
            {
                return this._interfaceType;
            }
        }
    }
}
