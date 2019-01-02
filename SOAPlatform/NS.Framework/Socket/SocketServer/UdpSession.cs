using System;
using System.Net;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// UDP会话--主要是在服务器端记录与当前服务器连接的客户端会话信息
    /// </summary>
    public sealed class UdpSession
    {
        #region 成员定义
        /// <summary>
        /// 当前UDP连接会话的目标服务器
        /// </summary>
        private readonly IUdpServer _server = null;
        /// <summary>
        /// 远程终端的终结点ip信息
        /// </summary>
        public readonly EndPoint RemoteEndPoint = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <param name="server"></param>
        /// <exception cref="ArgumentNullException">server is null</exception>
        public UdpSession(EndPoint remoteEndPoint, IUdpServer server)
        {
            if (server == null) throw new ArgumentNullException("server");
            this.RemoteEndPoint = remoteEndPoint;
            this._server = server;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 异步发送信息
        /// </summary>
        /// <param name="payload">待发送的数据</param>
        /// <exception cref="ArgumentNullException">payload is null or empty</exception>
        public void SendAsync(byte[] payload)
        {
            this._server.SendTo(this.RemoteEndPoint, payload);
        }
        #endregion
    }
}