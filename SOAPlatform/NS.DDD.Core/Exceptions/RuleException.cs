using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 业务规则异常信息
    /// </summary>
    public class RuleException : System.Exception
    {
        public RuleException(string message)
            : base(message)
        {
        }

        public RuleException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public RuleException()
            : base()
        {
        }

        public RuleException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
