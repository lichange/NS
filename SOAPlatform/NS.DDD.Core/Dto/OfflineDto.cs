using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    /// <summary>
    /// 离线数据信息
    /// </summary>
    [DataContract]
    public class OfflineDto
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [DataMember]
        public string OfflineDtoId
        {
            get;
            set;
        }

        /// <summary>
        /// 服务类型
        /// </summary>
        [DataMember]
        public string ServiceType
        {
            get;
            set;
        }

        /// <summary>
        /// 服务类型所在程序集
        /// </summary>
        [DataMember]
        public string ServiceAssembly
        {
            get;
            set;
        }

        /// <summary>
        /// 服务调用的方法名称
        /// </summary>
        [DataMember]
        public string MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// 服务调用的方法参数
        /// </summary>
        [DataMember]
        public IList<object> ParameterInfos
        {
            get;
            set;
        }
    }
}
