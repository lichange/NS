using System;
using System.Net;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// 服务器池接口定义
    /// </summary>
    public interface IServerPool
    {
        /// <summary>
        /// 当客户端与服务器建立连接成功时事件委托
        /// </summary>
        event Action<string, SocketBase.IConnection> Connected;
        /// <summary>
        /// 当服务器可用时的事件委托
        /// </summary>
        event Action<string, SocketBase.IConnection> ServerAvailable;

        /// <summary>
        /// 注册连接的服务器节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        bool TryRegisterNode(string name, EndPoint endPoint);

        /// <summary>
        /// 移除注册的服务器节点
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        bool UnRegisterNode(string name);

        /// <summary>
        /// 取得一个可用的服务器连接对象
        /// </summary>
        /// <returns></returns>
        SocketBase.IConnection Acquire();
        /// <summary>
        /// 根据一个特定的hash值取得一个可用的服务器连接对象
        /// </summary>
        /// <param name="hash">一致性哈希值</param>
        /// <returns></returns>
        SocketBase.IConnection Acquire(byte[] hash);

        /// <summary>
        /// 获取所有的服务器节点名称
        /// </summary>
        /// <returns></returns>
        string[] GetAllNodeNames();
    }
}