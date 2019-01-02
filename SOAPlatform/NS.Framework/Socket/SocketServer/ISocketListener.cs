using System;
using System.Net;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// socket监听器-服务器宿主监听接口
    /// </summary>
    public interface ISocketListener
    {
        /// <summary>
        /// 当客户端socket连接到服务器时的事件委托
        /// </summary>
        event Action<ISocketListener, SocketBase.IConnection> Accepted;

        /// <summary>
        /// 监听器名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// IP地址
        /// </summary>
        EndPoint EndPoint { get; }
        /// <summary>
        /// 开始监听
        /// </summary>
        void Start();
        /// <summary>
        /// 停止监听
        /// </summary>
        void Stop();
    }
}