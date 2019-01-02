using System;

namespace NS.Framework.RocketSocket.Client.Protocol
{
    /// <summary>
    /// 协议接口
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IProtocol<TResponse> where TResponse : Response.IResponse
    {
        /// <summary>
        /// 解析信息为响应对象
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        TResponse FindResponse(SocketBase.IConnection connection, ArraySegment<byte> buffer, out int readlength);
    }
}