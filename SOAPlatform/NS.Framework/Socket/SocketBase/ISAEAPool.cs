using System.Net.Sockets;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 异步socket 连接池接口
    /// </summary>
    public interface ISAEAPool
    {
        /// <summary>
        /// 获取socket 异步连接
        /// </summary>
        /// <returns></returns>
        SocketAsyncEventArgs GetSocketAsyncEventArgs();

        /// <summary>
        /// 释放一个socket连接
        /// </summary>
        /// <param name="e"></param>
        void ReleaseSocketAsyncEventArgs(SocketAsyncEventArgs e);
    }
}