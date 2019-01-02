using System;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// Socket服务端命令处理服务接口定义
    /// </summary>
    /// <typeparam name="TCommandInfo">服务器端处理的命令类型</typeparam>
    public interface ISocketService<TCommandInfo> where TCommandInfo : class, Command.ICommandInfo
    {
        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        void OnConnected(SocketBase.IConnection connection);
        /// <summary>
        /// 开始发送<see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        void OnStartSending(SocketBase.IConnection connection, SocketBase.Packet packet);
        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        void OnSendCallback(SocketBase.IConnection connection, SocketBase.SendCallbackEventArgs e);
        /// <summary>
        /// 当接收到客户端新消息时，会调用此方法.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdInfo"></param>
        void OnReceived(SocketBase.IConnection connection, TCommandInfo cmdInfo);
        /// <summary>
        /// 当socket连接断开时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnDisconnected(SocketBase.IConnection connection, Exception ex);
        /// <summary>
        /// 当发生异常时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnException(SocketBase.IConnection connection, Exception ex);
    }
}