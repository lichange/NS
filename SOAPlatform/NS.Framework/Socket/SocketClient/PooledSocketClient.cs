using System;
using System.Net;
using NS.Framework.RocketSocket.SocketBase;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// 客户端
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class PooledSocketClient<TResponse> : BaseSocketClient<TResponse> where TResponse : class, Response.IResponse
    {
        #region 私有成员
        /// <summary>
        /// 服务池定义
        /// </summary>
        private readonly IServerPool _serverPool = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="protocol"></param>
        public PooledSocketClient(Protocol.IProtocol<TResponse> protocol)
            : this(protocol, 8192, 9192, 6000, 6000)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        /// <exception cref="ArgumentNullException">protocol is null</exception>
        public PooledSocketClient(Protocol.IProtocol<TResponse> protocol,
            int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(protocol, socketBufferSize, messageBufferSize, millisecondsSendTimeout, millisecondsReceiveTimeout)
        {
            this._serverPool = this.InitServerPool();
            this._serverPool.Connected += this.OnServerPoolConnected;
            this._serverPool.ServerAvailable += this.OnServerPoolServerAvailable;
        }
        #endregion

        #region 保护成员方法
        /// <summary>
        /// create <see cref="IServerPool"/> instance. 创建服务池实例
        /// </summary>
        /// <returns></returns>
        protected virtual IServerPool InitServerPool()
        {
            return new DefaultServerPool(this);
        }
        /// <summary>
        /// 当与服务器建立连接后
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        protected virtual void OnServerPoolConnected(string name, IConnection connection)
        {
            base.RegisterConnection(connection);
        }
        /// <summary>
        /// 当服务连接可用时
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connection"></param>
        protected virtual void OnServerPoolServerAvailable(string name, IConnection connection)
        {
            var arr = base.DequeueAllFromPendingQueue();
            if (arr == null) return;
            for (int i = 0, l = arr.Length; i < l; i++) this.Send(arr[i]);
        }
        #endregion

        #region 重写基类方法-发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="request">待发送的消息内容</param>
        protected override void Send(Request<TResponse> request)
        {
            var connection = this._serverPool.Acquire(request.ConsistentKey);
            if (connection == null) this.EnqueueToPendingQueue(request);//没有连接可用，放入待发送队列
            else connection.BeginSend(request);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 添加服务器节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public bool RegisterServerNode(string name, IPEndPoint endPoint)
        {
            return this._serverPool.TryRegisterNode(name, endPoint);
        }
        /// <summary>
        /// 根据服务器节点名称移除服务器节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UnRegisterServerNode(string name)
        {
            return this._serverPool.UnRegisterNode(name);
        }
        /// <summary>
        /// 获取所有的服务器节点名称
        /// </summary>
        /// <returns></returns>
        public string[] GetAllNodeNames()
        {
            return this._serverPool.GetAllNodeNames();
        }
        #endregion
    }
}