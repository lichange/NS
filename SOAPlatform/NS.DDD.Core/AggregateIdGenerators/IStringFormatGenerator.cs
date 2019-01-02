using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.AggregateIdGenerators
{
    /// <summary>
    /// 聚合根-主键生成器--字符串生成器--默认格式："YYYYmmDDHHMMSS+001序列号"
    /// </summary>
    public interface IStringFormatGenerator : IAggregateIdGenerator
    {

    }
}
