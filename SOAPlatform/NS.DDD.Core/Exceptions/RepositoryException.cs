using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 仓储异常信息
    /// </summary>
    public class RepositoryException : System.Exception
    {
        public RepositoryException(string message)
            : base(message)
        {
        }

        public RepositoryException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public RepositoryException()
            : base()
        {
        }

        public RepositoryException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
