using System;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 断开与服务器连接时的委托定义
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="ex"></param>
    public delegate void DisconnectedHandler(IConnection connection, Exception ex);
}