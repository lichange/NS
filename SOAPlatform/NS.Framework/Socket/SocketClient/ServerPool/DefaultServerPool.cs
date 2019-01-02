using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NS.Framework.RocketSocket.SocketBase.Utils;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// 默认的服务器池实现
    /// </summary>
    public sealed class DefaultServerPool : IServerPool
    {
        #region 私有成员
        /// <summary>
        /// 服务器宿主
        /// </summary>
        private SocketBase.IHost _host = null;
        /// <summary>
        /// 心跳检测次数
        /// </summary>
        private int _acquireTimes = 0;

        /// <summary>
        /// key:服务器心跳检测器
        /// </summary>
        private readonly Dictionary<string, SocketConnector> _dicNodes =
            new Dictionary<string, SocketConnector>();
        /// <summary>
        /// key:node name
        /// value:socket connection
        /// 服务器池字典
        /// </summary>
        private readonly Dictionary<string, SocketBase.IConnection> _dicConnections =
            new Dictionary<string, SocketBase.IConnection>();
        /// <summary>
        /// 服务器数组对象
        /// </summary>
        private SocketBase.IConnection[] _arrConnections = null;
        /// <summary>
        /// socket服务器hash容器
        /// </summary>
        private ConsistentHashContainer<SocketBase.IConnection> _hashConnections = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host">服务器宿主</param>
        /// <exception cref="ArgumentNullException">host is null</exception>
        public DefaultServerPool(SocketBase.IHost host)
        {
            if (host == null) throw new ArgumentNullException("host");
            this._host = host;
        }
        #endregion

        #region IServerPool 成员
        /// <summary>
        /// 当客户端与服务器建立连接成功时事件委托
        /// </summary>
        public event Action<string, SocketBase.IConnection> Connected;
        /// <summary>
        /// 当服务器可用时的事件委托
        /// </summary>
        public event Action<string, SocketBase.IConnection> ServerAvailable;

        /// <summary>
        /// 注册服务器节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public bool TryRegisterNode(string name, EndPoint endPoint)
        {
            SocketConnector node = null;
            lock (this)
            {
                if (this._dicNodes.ContainsKey(name)) return false;
                this._dicNodes[name] = node = new SocketConnector(name, endPoint, this._host,
                    this.OnConnected, this.OnDisconnected);
            }
            node.Start();
            return true;
        }

        /// <summary>
        /// 移除服务器节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">name is null or empty</exception>
        public bool UnRegisterNode(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            SocketConnector node = null;
            SocketBase.IConnection connection = null;
            lock (this)
            {
                //remove node by name,
                if (this._dicNodes.TryGetValue(name, out node)) this._dicNodes.Remove(name);
                //get connection by name.
                this._dicConnections.TryGetValue(name, out connection);
            }

            if (node != null) node.Stop();
            if (connection != null) connection.BeginDisconnect();
            return node != null;
        }

        /// <summary>
        /// 取得一个可用的服务器连接对象
        /// </summary>
        /// <returns></returns>
        public SocketBase.IConnection Acquire()
        {
            var arr = this._arrConnections;
            if (arr == null || arr.Length == 0) return null;

            if (arr.Length == 1) return arr[0];
            return arr[(Interlocked.Increment(ref this._acquireTimes) & 0x7fffffff) % arr.Length];
        }

        /// <summary>
        /// 根据一个特定的hash值取得一个可用的服务器连接对象
        /// </summary>
        /// <param name="hash">一致性哈希值</param>
        /// <returns></returns>
        public SocketBase.IConnection Acquire(byte[] hash)
        {
            if (hash == null || hash.Length == 0) return this.Acquire();

            var hashConnections = this._hashConnections;
            if (hashConnections == null) return null;
            return hashConnections.Get(hash);
        }

        /// <summary>
        /// 获取所有的服务器节点名称
        /// </summary>
        /// <returns></returns>
        public string[] GetAllNodeNames()
        {
            lock (this) return this._dicNodes.Keys.ToArray();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 当客户端与服务器建立连接成功时发生的事件处理
        /// </summary>
        /// <param name="node"></param>
        /// <param name="connection"></param>
        private void OnConnected(SocketConnector node, SocketBase.IConnection connection)
        {
            //fire connected event.
            this.Connected(node.Name, connection);

            bool isActive = false;
            SocketBase.IConnection oldConnection = null;
            lock (this)
            {
                //remove exists connection by name.
                if (this._dicConnections.TryGetValue(node.Name, out oldConnection)) this._dicConnections.Remove(node.Name);
                //add curr connection to list if node is active
                if (isActive = this._dicNodes.ContainsKey(node.Name)) this._dicConnections[node.Name] = connection;

                this._arrConnections = this._dicConnections.Values.ToArray();
                this._hashConnections = new ConsistentHashContainer<SocketBase.IConnection>(this._dicConnections);
            }
            //disconect old connection.
            if (oldConnection != null) oldConnection.BeginDisconnect();
            //disconnect not active node connection.
            if (!isActive) connection.BeginDisconnect();
            //fire server available event.
            if (isActive && this.ServerAvailable != null) this.ServerAvailable(node.Name, connection);
        }
        /// <summary>
        /// 当与服务器端断开连接时发生的事件处理
        /// </summary>
        /// <param name="node"></param>
        /// <param name="connection"></param>
        private void OnDisconnected(SocketConnector node, SocketBase.IConnection connection)
        {
            lock (this)
            {
                if (!this._dicConnections.Remove(node.Name)) return;

                this._arrConnections = this._dicConnections.Values.ToArray();
                this._hashConnections = new ConsistentHashContainer<SocketBase.IConnection>(this._dicConnections);
            }
        }
        #endregion
    }
}