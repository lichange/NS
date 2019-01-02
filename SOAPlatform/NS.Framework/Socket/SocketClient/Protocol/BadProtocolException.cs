
namespace NS.Framework.RocketSocket.Client.Protocol
{
    /// <summary>
    /// 错误协议类型异常信息对象
    /// </summary>
    public sealed class BadProtocolException : System.ApplicationException
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BadProtocolException()
            : base("bad protocol.")
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public BadProtocolException(string message)
            : base(message)
        {
        }
    }
}