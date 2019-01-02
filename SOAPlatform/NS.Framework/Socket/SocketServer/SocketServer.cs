using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// socket 服务器
    /// </summary>
    /// <typeparam name="TCommandInfo">命令类型</typeparam>
    public class SocketServer<TCommandInfo> : BaseSocketServer where TCommandInfo : class, Command.ICommandInfo
    {
        #region 私有成员
        /// <summary>
        /// 当前服务端的监听器列表
        /// </summary>
        private readonly List<SocketListener> _listListener = new List<SocketListener>();
        /// <summary>
        /// 负责处理命令的socket服务对象定义
        /// </summary>
        private readonly ISocketService<TCommandInfo> _socketService = null;
        /// <summary>
        /// 协议
        /// </summary>
        private readonly Protocol.IProtocol<TCommandInfo> _protocol = null;
        /// <summary>
        /// 消息的最大长度
        /// </summary>
        private readonly int _maxMessageSize;
        /// <summary>
        /// 最大连接数
        /// </summary>
        private readonly int _maxConnections;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketService">socket服务器端处理命令服务</param>
        /// <param name="protocol"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="maxConnections"></param>
        /// <exception cref="ArgumentNullException">socketService is null.</exception>
        /// <exception cref="ArgumentNullException">protocol is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxMessageSize</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxConnections</exception>
        public SocketServer(ISocketService<TCommandInfo> socketService,
            Protocol.IProtocol<TCommandInfo> protocol,
            int socketBufferSize,
            int messageBufferSize,
            int maxMessageSize,
            int maxConnections)
            : base(socketBufferSize, messageBufferSize)
        {
            if (socketService == null) throw new ArgumentNullException("socketService");
            if (protocol == null) throw new ArgumentNullException("protocol");
            if (maxMessageSize < 1) throw new ArgumentOutOfRangeException("maxMessageSize");
            if (maxConnections < 1) throw new ArgumentOutOfRangeException("maxConnections");

            this._socketService = socketService;
            this._protocol = protocol;
            this._maxMessageSize = maxMessageSize;
            this._maxConnections = maxConnections;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 监听器接收到socket连接时执行的事件处理函数
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="connection"></param>
        private void listener_Accepted(ISocketListener listener, SocketBase.IConnection connection)
        {
            if (base._listConnections.Count() > this._maxConnections)
            {
                connection.BeginDisconnect(); return;
            }
            base.RegisterConnection(connection);
        }
        #endregion

        #region 接口中方法
        /// <summary>
        /// 打开socket服务器-开启监听服务器
        /// </summary>
        public override void Start()
        {
            foreach (var child in this._listListener) child.Start();
        }
        /// <summary>
        /// 关闭socket服务器-停止监听-停止消息收发，释放资源
        /// </summary>
        public override void Stop()
        {
            foreach (var child in this._listListener) child.Stop();
            base.Stop();
        }
        /// <summary>
        /// 添加socket监听器
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <param name="endPoint">终结点</param>
        /// <returns></returns>
        public override ISocketListener AddListener(string name, IPEndPoint endPoint)
        {
            var listener = new SocketListener(name, endPoint, this);
            this._listListener.Add(listener);
            listener.Accepted += new Action<ISocketListener, SocketBase.IConnection>(this.listener_Accepted);
            return listener;
        }
        /// <summary>
        /// 当终端与服务器端连接成功时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnConnected(SocketBase.IConnection connection)
        {
            base.OnConnected(connection);
            try { this._socketService.OnConnected(connection); }
            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// 开始发送数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        protected override void OnStartSending(SocketBase.IConnection connection, SocketBase.Packet packet)
        {
            base.OnStartSending(connection, packet);
            try { this._socketService.OnStartSending(connection, packet); }
            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// 数据发送完成后的回调事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnSendCallback(SocketBase.IConnection connection, SocketBase.SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);
            try { this._socketService.OnSendCallback(connection, e); }
            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// 收到消息时执行的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnMessageReceived(SocketBase.IConnection connection, SocketBase.MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(connection, e);

            int readlength;
            TCommandInfo cmdInfo = null;
            try
            {
                cmdInfo = this._protocol.FindCommandInfo(connection, e.Buffer, this._maxMessageSize, out readlength);
            }
            catch (Exception ex)
            {
                this.OnError(connection, ex);
                connection.BeginDisconnect(ex);
                e.SetReadlength(e.Buffer.Count);
                return;
            }

            //通过task 来处理命令
            if (cmdInfo != null)
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try { this._socketService.OnReceived(connection, cmdInfo); }
                    catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
                });
            e.SetReadlength(readlength);
        }
        /// <summary>
        /// 当断开连接时执行的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnDisconnected(SocketBase.IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            try { this._socketService.OnDisconnected(connection, ex); }
            catch (Exception ex2) { SocketBase.Log.Trace.Error(ex.Message, ex2); }
        }
        /// <summary>
        /// 当通信的过程中出现异常时执行的事件处理
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnError(SocketBase.IConnection connection, Exception ex)
        {
            base.OnError(connection, ex);
            try { this._socketService.OnException(connection, ex); }
            catch (Exception ex2) { SocketBase.Log.Trace.Error(ex.Message, ex2); }
        }
        #endregion
    }
}