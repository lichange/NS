using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Rules
{
    /// <summary>
    /// 查询前执行的业务规则定义
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueryableRule<T> : IBussinessRule
    {
    }
}
