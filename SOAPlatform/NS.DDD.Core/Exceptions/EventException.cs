using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 领域事件异常信息
    /// </summary>
    public class EventException : System.Exception
    {
        public EventException(string message)
            : base(message)
        {
        }

        public EventException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public EventException()
            : base()
        {
        }

        public EventException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}
