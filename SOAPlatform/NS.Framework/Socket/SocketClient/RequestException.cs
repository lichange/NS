using System;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// socket请求发送的过程中出现的异常信息
    /// </summary>
    public sealed class RequestException : ApplicationException
    {
        /// <summary>
        /// 错误类型
        /// </summary>
        public readonly Errors Error;
        /// <summary>
        /// 当前请求消息对应的命令名称
        /// </summary>
        public readonly string CmdName;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cmdName"></param>
        public RequestException(Errors error, string cmdName)
            : base(string.Concat("errorType:", error.ToString(), " cmdName:", cmdName ?? string.Empty))
        {
            this.Error = error;
        }

        /// <summary>
        /// 错误类型枚举定义
        /// </summary>
        public enum Errors : byte
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0,
            /// <summary>
            /// 等待发送超时
            /// </summary>
            PendingSendTimeout = 1,
            /// <summary>
            /// 接收超时
            /// </summary>
            ReceiveTimeout = 2
        }
    }
}