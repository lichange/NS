using System;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 发送完成后的回掉事件
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="e"></param>
    public delegate void SendCallbackHandler(IConnection connection, SendCallbackEventArgs e);

    /// <summary>
    /// 发送完成时回调的事件参数对象
    /// </summary>
    public sealed class SendCallbackEventArgs
    {
        #region Public Members
        /// <summary>
        /// 数据包
        /// </summary>
        public readonly Packet Packet;
        /// <summary>
        /// 状态
        /// </summary>
        public readonly SendCallbackStatus Status;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="status"></param>
        /// <exception cref="ArgumentNullException">packet is null</exception>
        public SendCallbackEventArgs(Packet packet, SendCallbackStatus status)
        {
            if (packet == null) throw new ArgumentNullException("packet");
            this.Packet = packet;
            this.Status = status;
        }
        #endregion
    }

    /// <summary>
    /// 数据包的发送状态
    /// </summary>
    public enum SendCallbackStatus : byte
    {
        /// <summary>
        /// 发送成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 发送失败
        /// </summary>
        Failed = 2
    }
}