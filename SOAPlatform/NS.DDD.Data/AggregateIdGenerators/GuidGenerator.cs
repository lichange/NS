using NS.DDD.Core.AggregateIdGenerators;
using NS.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Data.AggregateIdGenerators
{
    /// <summary>
    /// GUID生成器
    /// </summary>
    [Export(typeof(IGuidGenerator))]
    public class GuidGenerator : IGuidGenerator
    {
        /// <summary>
        /// 上下文
        /// </summary>
        public object Context
        {
            get;
            set;
        }

        public object Generate<T>() where T : class, Core.IAggregateRoot
        {
            return Guid.NewGuid().ToString();
        }
    }
}
