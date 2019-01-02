using System;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 异常或错误处理的委托定义
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="ex"></param>
    public delegate void ErrorHandler(IConnection connection, Exception ex);
}