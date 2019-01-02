using System.Net;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// 抽象的socket服务器
    /// </summary>
    public abstract class BaseSocketServer : SocketBase.BaseHost
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize">socket缓冲区大小</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        protected BaseSocketServer(int socketBufferSize, int messageBufferSize)
            : base(socketBufferSize, messageBufferSize)
        {
        }

        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <param name="endPoint">监听的地址及端口号</param>
        /// <returns></returns>
        public abstract ISocketListener AddListener(string name, IPEndPoint endPoint);
    }
}