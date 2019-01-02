using System.Net;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// UDP服务器接口定义
    /// </summary>
    public interface IUdpServer
    {
        /// <summary>
        /// 开始
        /// </summary>
        void Start();
        /// <summary>
        /// stop
        /// </summary>
        void Stop();
        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="endPoint">目标地址</param>
        /// <param name="payload">消息内容</param>
        void SendTo(EndPoint endPoint, byte[] payload);
    }

    /// <summary>
    /// UDP服务接口定义-支持命令处理
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public interface IUdpServer<TCommandInfo> : IUdpServer where TCommandInfo : class, Command.ICommandInfo
    {

    }
}