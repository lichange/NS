using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.Dto
{
    [DataContract]
    public class BaseDto
    {
        /// <summary>
        /// 服务调用名称
        /// </summary>
        [DataMember]
        public string Operation
        {
            get;
            set;
        }

        private bool success = true;
        /// <summary>
        /// 标识请求的状态是否成功
        /// </summary>
        [DataMember]
        public bool IsSuccess
        {
            get
            {
                return this.success;
            }
            set
            {
                this.success = value;
            }
        }

        /// <summary>
        /// 服务调用过程中的错误信息
        /// </summary>
        [DataMember]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 服务调用过程中的错误信息
        /// </summary>
        [DataMember]
        public OperatingType OperationType
        {
            get;
            set;
        }
    }
}
