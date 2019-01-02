using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// socket服务器宿主抽象实现-所有的socket服务器宿主从该类继承
    /// </summary>
    public abstract class BaseHost : IHost
    {
        #region 成员定义
        /// <summary>
        /// 当前连接到宿主的当前连接的最大唯一标识
        /// </summary>
        private long _connectionID = 1000L;
        /// <summary>
        /// 与当前宿主建立连接的连接集合
        /// </summary>
        protected readonly ConnectionCollection _listConnections = new ConnectionCollection();
        /// <summary>
        /// socket异步操作对象栈 
        /// </summary>
        private readonly ConcurrentStack<SocketAsyncEventArgs> _stack = new ConcurrentStack<SocketAsyncEventArgs>();
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize">socket缓冲区大小</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        /// <exception cref="ArgumentOutOfRangeException">socketBufferSize</exception>
        /// <exception cref="ArgumentOutOfRangeException">messageBufferSize</exception>
        protected BaseHost(int socketBufferSize, int messageBufferSize)
        {
            if (socketBufferSize < 1) throw new ArgumentOutOfRangeException("socketBufferSize");
            if (messageBufferSize < 1) throw new ArgumentOutOfRangeException("messageBufferSize");

            this.SocketBufferSize = socketBufferSize;
            this.MessageBufferSize = messageBufferSize;
        }
        #endregion

        #region 宿主公有成员

        /// <summary>
        /// 获取socket的缓冲区大小
        /// </summary>
        public int SocketBufferSize
        {
            get;
            private set;
        }

        /// <summary>
        ///获取消息的缓冲区大小
        /// </summary>
        public int MessageBufferSize
        {
            get;
            private set;
        }

        /// <summary>
        /// 生成下一个连接ID
        /// </summary>
        /// <returns></returns>
        public long NextConnectionID()
        {
            return Interlocked.Increment(ref this._connectionID);
        }

        /// <summary>
        /// get <see cref="IConnection"/> 根据连接标识获取连接对象
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        public IConnection GetConnectionByID(long connectionID)
        {
            return this._listConnections.Get(connectionID);
        }

        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            this._listConnections.DisconnectAll();
        }

        #endregion

        #region 保护成员

        /// <summary>
        /// 注册连接到连接池中
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="ArgumentNullException">连接 is null</exception>
        protected void RegisterConnection(IConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (!connection.Active) return;

            connection.StartSending += new StartSendingHandler(this.OnStartSending);
            connection.SendCallback += new SendCallbackHandler(this.OnSendCallback);
            connection.MessageReceived += new MessageReceivedHandler(this.OnMessageReceived);
            connection.Disconnected += new DisconnectedHandler(this.OnDisconnected);
            connection.Error += new ErrorHandler(this.OnError);

            this._listConnections.Add(connection);
            this.OnConnected(connection);
        }

        /// <summary>
        /// 当终端连接到服务器宿主时的事件处理函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        protected virtual void OnConnected(IConnection connection)
        {
            Log.Trace.Debug(string.Concat("socket connected, id:", connection.ConnectionID.ToString(),
                ", remot endPoint:", connection.RemoteEndPoint == null ? string.Empty : connection.RemoteEndPoint.ToString(),
                ", local endPoint:", connection.LocalEndPoint == null ? string.Empty : connection.LocalEndPoint.ToString()));
        }

        /// <summary>
        /// 开始发送消息时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        protected virtual void OnStartSending(IConnection connection, Packet packet)
        {
        }

        /// <summary>
        /// 消息发送完成时的回调事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected virtual void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
        }

        /// <summary>
        /// 收到消息时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected virtual void OnMessageReceived(IConnection connection, MessageReceivedEventArgs e)
        {
        }

        /// <summary>
        /// 当终端与服务器宿主断开连接时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected virtual void OnDisconnected(IConnection connection, Exception ex)
        {
            this._listConnections.Remove(connection.ConnectionID);

            connection.StartSending -= new StartSendingHandler(this.OnStartSending);
            connection.SendCallback -= new SendCallbackHandler(this.OnSendCallback);
            connection.MessageReceived -= new MessageReceivedHandler(this.OnMessageReceived);
            connection.Disconnected -= new DisconnectedHandler(this.OnDisconnected);
            connection.Error -= new ErrorHandler(this.OnError);

            Log.Trace.Debug(string.Concat("socket disconnected, id:", connection.ConnectionID.ToString(),
                ", remot endPoint:", connection.RemoteEndPoint == null ? string.Empty : connection.RemoteEndPoint.ToString(),
                ", local endPoint:", connection.LocalEndPoint == null ? string.Empty : connection.LocalEndPoint.ToString(),
                ex == null ? string.Empty : string.Concat(", reason is: ", ex.ToString())));
        }

        /// <summary>
        /// 当与服务区建立连接或发生其他的错误时执行的事件处理函数
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="ex">异常信息对象</param>
        protected virtual void OnError(IConnection connection, Exception ex)
        {
            Log.Trace.Error(ex.Message, ex);
        }
        #endregion

        #region socket连接池成员

        /// <summary>
        /// 获取新的socket异步操作对象
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs GetSocketAsyncEventArgs()
        {
            SocketAsyncEventArgs e;
            if (this._stack.TryPop(out e)) return e;

            e = new SocketAsyncEventArgs();
            e.SetBuffer(new byte[this.MessageBufferSize], 0, this.MessageBufferSize);
            return e;
        }

        /// <summary>
        /// 释放指定的socket异步操作对象
        /// </summary>
        /// <param name="e">socket异步操作对象</param>
        public void ReleaseSocketAsyncEventArgs(SocketAsyncEventArgs e)
        {
            if (e.Buffer == null || e.Buffer.Length != this.MessageBufferSize)
            {
                e.Dispose(); return;
            }

            if (this._stack.Count >= 50000) { e.Dispose(); return; }

            this._stack.Push(e);
        }
        #endregion
    }
}