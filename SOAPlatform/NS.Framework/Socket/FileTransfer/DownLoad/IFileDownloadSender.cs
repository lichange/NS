using System;
using System.Net;

namespace NS.Framework.Communication
{
    /// <summary>
    /// 大文件传输接口，要求支持断点续传，同时要求是异步传输，能够多线程处理。
    /// </summary>
    public interface IFileDownloadSender
    {
        void Start();

        void SendFile(string filePath);

        void Stop();

        /// <summary>
        /// 传输过程中出现错误是执行的事件通知
        /// </summary>
        event Action<string> OnErrorOccurredEvent;

        /// <summary>
        /// 文件块发送完成时执行的事件通知
        /// </summary>
        event Action<string, string> OnBlockFinishedEvent;

        /// <summary>
        /// 连接丢失或断开时的事件通知
        /// </summary>
        event Action<string> OnConnectLostedEvent;

        /// <summary>
        /// 收到socket相关命令时发生
        /// </summary>
        event Action<string> OnCommandReceivedEvent;
    }
}