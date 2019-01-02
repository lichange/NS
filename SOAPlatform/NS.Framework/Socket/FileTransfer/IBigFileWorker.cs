using System;
using System.Net;
using System.Threading;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 文件传输后台工作处理器
    /// </summary>
    public interface IBigFileWorker
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsFinished
        {
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        void Initialize(string path, long position, long length);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="worked"></param>
        /// <param name="total"></param>
        void Initialize(string path, long position, long length, long worked, long total);

        /// <summary>
        /// 实时进度通知
        /// </summary>
        /// <param name="worked"></param>
        /// <param name="total"></param>
        void ReportProgress(out long worked, out long total);

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="waitHandle"></param>
        void RunWork(EventWaitHandle waitHandle);
    }

    public interface IBigFileSendWorker : IBigFileWorker
    {
    }

    public interface IBigFileReceiveWorker : IBigFileWorker
    {
    }
}