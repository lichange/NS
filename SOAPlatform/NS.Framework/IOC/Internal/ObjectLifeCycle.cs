using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.IOC
{
    internal enum ObjectLifeCycle
    {
        /// <summary>
        /// 单例
        /// </summary>
        Singleton,
        /// <summary>
        /// 每次创建
        /// </summary>
        New
    }
}
