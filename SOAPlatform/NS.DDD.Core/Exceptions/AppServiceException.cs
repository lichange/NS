using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 应用服务层出现的异常信息
    /// </summary>
    public class AppServiceException : System.Exception
    {
        public AppServiceException(string message)
            : base(message)
        {
        }

        public AppServiceException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public AppServiceException()
            : base()
        {
        }

        public AppServiceException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
