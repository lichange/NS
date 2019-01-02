using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.DDD.Core.Dto
{
    /// <summary>
    /// 数据同步服务中的传输对象定义-每次调用的时候，都是传输该对象或该对象的集合
    /// </summary>
    [Serializable()]
    public class ServerDataSyncContract
    {
        ///<summary>
        ///主键
        /// </summary>
        public virtual string DataSyncId
        {
            get;
            set;
        }

        /// <summary>
        /// 数据实体类型
        /// </summary>
        public virtual string EntityType
        {
            get;
            set;
        }

        /// <summary>
        /// 数据实体信息
        /// </summary>
        public virtual string EntityData
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型-添加、编辑、删除
        /// </summary>
        public virtual string OperationType
        {
            get;
            set;
        }

        /// <summary>
        /// 实体唯一标识
        /// </summary>
        public virtual string EntityIndentity
        {
            get;
            set;
        }

        /// <summary>
        /// 服务器该数据被修改的最后日期
        /// </summary>
        public virtual DateTime EventTime
        {
            get;
            set;
        }
    }
}
