using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.AggregateIdGenerators
{
    /// <summary>
    /// 聚合根-主键生成器--自定义代码生成器--实现接口时可以返回任意的自定义的主键对象
    /// </summary>
    public interface ICustomGenerator : IAggregateIdGenerator
    {

    }
}
