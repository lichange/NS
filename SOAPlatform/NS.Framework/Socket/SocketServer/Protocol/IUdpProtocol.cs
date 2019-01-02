using System;
using System.Net;

namespace NS.Framework.RocketSocket.Server.Protocol
{
    /// <summary>
    /// UDP协议
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public interface IUdpProtocol<TCommandInfo> where TCommandInfo : Command.ICommandInfo
    {
        /// <summary>
        /// 根据消息内容查找对应的命令处理器
        /// </summary>
        /// <param name="buffer">数据信息</param>
        /// <returns></returns>
        TCommandInfo FindCommandInfo(ArraySegment<byte> buffer);
    }
}