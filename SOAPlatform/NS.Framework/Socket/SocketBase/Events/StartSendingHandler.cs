
namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 开始发送数据包的handler
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="packet"></param>
    public delegate void StartSendingHandler(IConnection connection, Packet packet);
}