using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Events
{
    /// <summary>
    /// 事件对象-基础的事件对象，该对象被标记为数据对象，可以被WCF自动序列化。
    /// </summary>
    [Serializable, DataContract]
    public abstract class Event : IEvent
    {
        [DataMember]
        public DateTime UtcTimestamp { get; protected set; }

        protected Event()
            : this(DateTime.UtcNow)
        {
        }

        protected Event(DateTime utcTimestamp)
        {
            UtcTimestamp = utcTimestamp;
        }
    }
}
