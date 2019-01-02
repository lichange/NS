using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-用于事件总线的事件标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EventExportAttribute : Attribute
    {
        private Type _interfaceType;
        private Type _eventType;

        private int tempOrder;
        public EventExportAttribute(Type interfaceType)
        {
            this._interfaceType = interfaceType;
            tempOrder = -1;
        }

        public EventExportAttribute(Type interfaceType, Type eventType)
        {
            this._interfaceType = interfaceType;
            this._eventType = eventType;
            this.tempOrder = 0;
        }

        public EventExportAttribute(Type interfaceType, Type eventType,int order)
        {
            this._interfaceType = interfaceType;
            this._eventType = eventType;
            this.tempOrder = order;
        }

        public int Order
        {
            get { return this.tempOrder; }
        }

        /// <summary>
        /// 事件参数类型
        /// </summary>
        public Type EventType
        {
            get
            {
                return this._eventType;
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
