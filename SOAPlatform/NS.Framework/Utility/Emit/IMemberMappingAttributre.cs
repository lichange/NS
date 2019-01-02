﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NS.Framework.Utility
{
    /// <summary> 控制对象成员名称的映射关系
    /// </summary>
    public interface IMemberMappingAttributre
    {
        /// <summary> 映射名
        /// </summary>
        string Name { get; }
    }
}
