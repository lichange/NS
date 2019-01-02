using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 对象特性标记-用于属性描述的标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassDescriptionAttribute : Attribute
    {
        private string _description;
        private int _index = 0;
        public ClassDescriptionAttribute(string description)
        {
            this._description = description;
        }

        public ClassDescriptionAttribute(string description, int index)
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


        /// <summary>
        /// 排序
        /// </summary>
        public int Index
        {
            get
            {
                return this._index;
            }
        }
    }
}
