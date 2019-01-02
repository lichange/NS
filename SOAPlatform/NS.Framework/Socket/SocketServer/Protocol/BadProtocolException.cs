
namespace NS.Framework.RocketSocket.Server.Protocol
{
    /// <summary>
    /// 错误协议异常
    /// </summary>
    public sealed class BadProtocolException : System.ApplicationException
    {
        /// <summary>
        /// new
        /// </summary>
        public BadProtocolException()
            : base("bad protocol.")
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="message"></param>
        public BadProtocolException(string message)
            : base(message)
        {
        }
    }
}