using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data
{
    /// <summary>
    /// 领域对象的持久化操作类型
    /// </summary>
    public enum OperationType : int
    {
        Add = 0,
        Update = 1,
        Delete = 2
    }
}
