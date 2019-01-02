using System;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// UDP服务接口定义
    /// </summary>
    /// <typeparam name="TCommandInfo">消息命令</typeparam>
    public interface IUdpService<TCommandInfo> where TCommandInfo : class, Command.ICommandInfo
    {
        /// <summary>
        /// 当接收到消息命令时发生
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cmdInfo"></param>
        void OnReceived(UdpSession session, TCommandInfo cmdInfo);

        /// <summary>
        /// 当出现错误时发生的事件处理
        /// </summary>
        /// <param name="session"></param>
        /// <param name="ex"></param>
        void OnError(UdpSession session, Exception ex);
    }
}