using System;
using System.Net;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件传输接口，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public interface IFileDownloadReceiver
    {
        void Start();

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        event Action<string> OnErrorOccurredEvent;

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        event Action<string> OnAllTaskFinishedEvent;

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        event Action<string> OnConnectLostedEvent;
        /// <summary>
        /// 文件块发送完成时执行的事件通知
        /// </summary>
        event Action<NS.Framework.Communication.FileDownloadHelper.FileTransmission> OnProgressChangedEvent;

    }
}