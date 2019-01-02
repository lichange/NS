using System;
using NS.Framework.RocketSocket.SocketBase;

namespace NS.Framework.RocketSocket.Server.Protocol
{
    /// <summary>
    /// 协议接口
    /// </summary>
    /// <typeparam name="TCommandInfo">命令类型</typeparam>
    public interface IProtocol<TCommandInfo> where TCommandInfo : Command.ICommandInfo
    {
        /// <summary>
        /// 根据传输的数据返回该消息对应的命令类型
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        TCommandInfo FindCommandInfo(IConnection connection, ArraySegment<byte> buffer,
            int maxMessageSize, out int readlength);
    }
}