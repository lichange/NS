using System;
using System.Net;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件传输接口，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public interface IBigFileSender
    {
        /// <summary>
        /// 实时通知当前传输的进度情况-以便在前台显示进度条情况。
        /// </summary>
        event Action<int, int> ReportProcessState;

        void SendFile(string filePath);
    }
}