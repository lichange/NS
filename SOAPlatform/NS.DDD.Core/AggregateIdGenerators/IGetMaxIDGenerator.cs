﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.AggregateIdGenerators
{
    /// <summary>
    /// 聚合根-主键生成器
    /// </summary>
    public interface IGetMaxIDGenerator: IAggregateIdGenerator
    {
    }
}
