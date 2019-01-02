using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-用于属性描述的标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyDescriptionAttribute : Attribute
    {
        private string _description;
        public PropertyDescriptionAttribute(string description)
        {
            this._description = description;
        }

        /// <summary>
        /// 属性描述内容
        /// </summary>
        public string Description
        {
            get
            {
                return this._description;
            }
        }
    }
}
