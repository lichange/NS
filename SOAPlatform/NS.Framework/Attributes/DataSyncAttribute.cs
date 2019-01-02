using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Framework.Attributes
{
    /// <summary>
    /// 上传到总部数据库
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DataSyncAttribute : Attribute
    {
        public DataSyncAttribute()
        {
            this.IsRealTimeSync = false;
        }

        public DataSyncAttribute(bool isRealTimeSync)
        {
            this.IsRealTimeSync = isRealTimeSync;
        }

        /// <summary>
        /// 指定该数据是否实时同步
        /// </summary>
        public bool IsRealTimeSync
        {
            get;
            set;
        }
    }
    /// <summary>
    /// 上传到阿里云
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DataSyncCloudAttribute : Attribute
    {
        public DataSyncCloudAttribute()
        {
            this.IsRealTimeSync = false;
        }

        public DataSyncCloudAttribute(bool isRealTimeSync)
        {
            this.IsRealTimeSync = isRealTimeSync;
        }

        /// <summary>
        /// 指定该数据是否实时同步
        /// </summary>
        public bool IsRealTimeSync
        {
            get;
            set;
        }
    }
}
