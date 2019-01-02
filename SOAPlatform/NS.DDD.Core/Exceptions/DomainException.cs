using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 领域模型异常信息
    /// </summary>
    public class DomainException : System.Exception
    {
        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public DomainException()
            : base()
        {
        }

        public DomainException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
