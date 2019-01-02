using System;

namespace NS.Framework.RocketSocket.SocketBase.Log
{
    /// <summary>
    /// 跟踪-监听器接口
    /// </summary>
    public interface ITraceListener
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        void Error(string message, Exception ex);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);
    }
}