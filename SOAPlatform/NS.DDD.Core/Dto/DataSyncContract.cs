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
    public class DataSyncContract
    {
        /// <summary>
        /// 唯一标识 - 可以是该记录的id或主键
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType
        {
            get;
            set;
        }

        /// <summary>
        /// 具体的操作的数据内容定义
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// 操作类型-一般来说是3种操作 增加、更新、删除
        /// </summary>
        public string OperationType
        {
            get;
            set;
        }

        /// <summary>
        /// 执行该操作的服务名及所在的程序集-该参数的格式为 xxxx.dll;xxxx.Service
        /// </summary>
        public string ServicePath
        {
            get;
            set;
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string MethodName
        {
            get;
            set;
        }

        /// <summary>
        /// 数据同步操作的结果-通过几种状态标识：1、成功，2、失败、3、未执行
        /// </summary>
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// 当同步出现异常时，该属性记录异常的详细信息
        /// </summary>
        public string Exception
        {
            get;
            set;
        }

        public DateTime OperationTime
        {
            get;
            set;
        }
        /// <summary>
        /// 传输标识,0为总部,1为云端
        /// </summary>
        public string SyncFlag { get; set; }
    }
}
