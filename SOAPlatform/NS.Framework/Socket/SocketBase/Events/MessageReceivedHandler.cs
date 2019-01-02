using System;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 收到消息时的handler
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="e"></param>
    public delegate void MessageReceivedHandler(IConnection connection, MessageReceivedEventArgs e);
    /// <summary>
    /// 消息处理handler
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="readlength"></param>
    public delegate void MessageProcessHandler(ArraySegment<byte> buffer, int readlength);

    /// <summary>
    /// 消息接收时的事件参数对象
    /// </summary>
    public sealed class MessageReceivedEventArgs
    {
        #region Members
        private readonly MessageProcessHandler _processCallback = null;
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public readonly ArraySegment<byte> Buffer;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="processCallback"></param>
        /// <exception cref="ArgumentNullException">processCallback is null</exception>
        public MessageReceivedEventArgs(ArraySegment<byte> buffer, MessageProcessHandler processCallback)
        {
            if (processCallback == null) throw new ArgumentNullException("processCallback");
            this.Buffer = buffer;
            this._processCallback = processCallback;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 设置已读取长度
        /// </summary>
        /// <param name="readlength"></param>
        public void SetReadlength(int readlength)
        {
            this._processCallback(this.Buffer, readlength);
        }
        #endregion
    }
}