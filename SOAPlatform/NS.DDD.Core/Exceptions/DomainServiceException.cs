using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 领域服务异常信息
    /// </summary>
    public class DomainServiceException : System.Exception
    {
        public DomainServiceException(string message)
            : base(message)
        {
        }

        public DomainServiceException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public DomainServiceException()
            : base()
        {
        }

        public DomainServiceException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
