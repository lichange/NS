using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    ///服务器宿主中德socket连接集合
    /// </summary>
    public sealed class ConnectionCollection
    {
        #region 私有成员
        /// <summary>
        /// 键:ConnectionID;值:socket连接对象
        /// </summary>
        private readonly ConcurrentDictionary<long, IConnection> _dic = new ConcurrentDictionary<long, IConnection>();
        #endregion

        #region 共有成员
        /// <summary>
        /// 向集合中添加连接
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">connection is null</exception>
        public bool Add(SocketBase.IConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            return this._dic.TryAdd(connection.ConnectionID, connection);
        }

        /// <summary>
        /// 从集合中移除连接,根据连接唯一标识
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        public bool Remove(long connectionID)
        {
            IConnection connection;
            return this._dic.TryRemove(connectionID, out connection);
        }

        /// <summary>
        /// 根据连接唯一标识从集合中获取连接
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        public SocketBase.IConnection Get(long connectionID)
        {
            IConnection connection;
            this._dic.TryGetValue(connectionID, out connection);
            return connection;
        }

        /// <summary>
        /// 转换为数组
        /// </summary>
        /// <returns></returns>
        public SocketBase.IConnection[] ToArray()
        {
            return this._dic.ToArray().Select(c => c.Value).ToArray();
        }

        /// <summary>
        /// 获取当前连接总数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this._dic.Count;
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void DisconnectAll()
        {
            var connections = this.ToArray();
            foreach (var conn in connections) conn.BeginDisconnect();
        }
        #endregion
    }
}