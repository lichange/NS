
namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// Socket连接的服务器端宿主接口定义
    /// </summary>
    public interface IHost : ISAEAPool
    {
        /// <summary>
        /// 设置Socket的缓冲区大小
        /// </summary>
        int SocketBufferSize { get; }

        /// <summary>
        /// 消息缓冲区大小
        /// </summary>
        int MessageBufferSize { get; }

        /// <summary>
        /// 生成下一个连接ID
        /// </summary>
        /// <returns></returns>
        long NextConnectionID();

        /// <summary>
        /// get <see cref="IConnection"/> by connectionID
        /// </summary>
        /// <param name="connectionID">链接Id</param>
        /// <returns>返回该链接id对应的链接对象</returns>
        IConnection GetConnectionByID(long connectionID);

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}