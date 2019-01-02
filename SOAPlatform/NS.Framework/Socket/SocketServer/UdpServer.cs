using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NS.Framework.RocketSocket.SocketBase;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// UDP服务器
    /// </summary>
    /// <typeparam name="TCommandInfo">命令类型约束</typeparam>
    public sealed class UdpServer<TCommandInfo> : SocketBase.Utils.DisposableBase, IUdpServer<TCommandInfo> where TCommandInfo : class, Command.ICommandInfo
    {
        #region 私有成员
        /// <summary>
        /// 服务器负责通信的端口号
        /// </summary>
        private readonly int _port;
        /// <summary>
        /// 消息缓冲区大小
        /// </summary>
        private readonly int _messageBufferSize;
        /// <summary>
        /// 接收线程数
        /// </summary>
        private readonly int _receiveThreads;//接收线程数
        /// <summary>
        /// 负责进行数据传输的套接字
        /// </summary>
        private Socket _socket = null;
        /// <summary>
        /// 异步发送socket异步操作对象池
        /// </summary>
        private AsyncSendPool _pool = null;
        /// <summary>
        /// 通信协议定义
        /// </summary>
        private readonly Protocol.IUdpProtocol<TCommandInfo> _protocol = null;
        /// <summary>
        /// 命令处理服务
        /// </summary>
        private readonly IUdpService<TCommandInfo> _service = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="protocol">协议</param>
        /// <param name="service">命令处理服务</param>
        public UdpServer(int port, Protocol.IUdpProtocol<TCommandInfo> protocol,
            IUdpService<TCommandInfo> service)
            : this(port, 2048, 1, protocol, service)
        {
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        /// <param name="receiveThreads">接收线程数</param>
        /// <param name="protocol">协议</param>
        /// <param name="service">命令处理服务</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException">protocol is null.</exception>
        /// <exception cref="ArgumentNullException">service is null.</exception>
        public UdpServer(int port, int messageBufferSize, int receiveThreads,
            Protocol.IUdpProtocol<TCommandInfo> protocol,
            IUdpService<TCommandInfo> service)
        {
            if (receiveThreads < 1) throw new ArgumentOutOfRangeException("receiveThreads");
            if (protocol == null) throw new ArgumentNullException("protocol");
            if (service == null) throw new ArgumentNullException("service");

            this._port = port;
            this._messageBufferSize = messageBufferSize;
            this._receiveThreads = receiveThreads;
            this._protocol = protocol;
            this._service = service;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 异步接收数据
        /// </summary>
        /// <param name="e"></param>
        private void BeginReceive(SocketAsyncEventArgs e)
        {
            if (!this._socket.ReceiveFromAsync(e)) this.ReceiveCompleted(this, e);
        }

        /// <summary>
        /// 接收数据完成时执行的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                TCommandInfo cmdInfo = null;

                try { cmdInfo = this._protocol.FindCommandInfo(new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred)); }
                catch (Exception ex)
                {
                    SocketBase.Log.Trace.Error(ex.Message, ex);
                    try { this._service.OnError(new UdpSession(e.RemoteEndPoint, this), ex); }
                    catch (Exception ex2) { SocketBase.Log.Trace.Error(ex2.Message, ex2); }
                }

                if (cmdInfo != null)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try { this._service.OnReceived(new UdpSession(e.RemoteEndPoint, this), cmdInfo); }
                        catch (Exception ex)
                        {
                            SocketBase.Log.Trace.Error(ex.Message, ex);
                            try { this._service.OnError(new UdpSession(e.RemoteEndPoint, this), ex); }
                            catch (Exception ex2) { SocketBase.Log.Trace.Error(ex2.Message, ex2); }
                        }
                    });
                }
            }
            this.BeginReceive(e);
        }
        #endregion

        #region IUdpServer 成员
        /// <summary>
        /// 打开UDP服务
        /// </summary>
        public void Start()
        {
            base.CheckDisposedWithException();

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this._socket.Bind(new IPEndPoint(IPAddress.Any, this._port));
            this._socket.DontFragment = true;

            this._pool = new AsyncSendPool(this._messageBufferSize, this._socket);

            for (int i = 0; i < this._receiveThreads; i++)
            {
                var e = new SocketAsyncEventArgs();
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                e.SetBuffer(new byte[this._messageBufferSize], 0, this._messageBufferSize);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.ReceiveCompleted);
                this.BeginReceive(e);
            }
        }
        /// <summary>
        /// 关闭UDP服务
        /// </summary>
        public void Stop()
        {
            base.Dispose();
        }
        /// <summary>
        /// 发送数据信息到指定终端
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="payload"></param>
        public void SendTo(EndPoint endPoint, byte[] payload)
        {
            base.CheckDisposedWithException();
            this._pool.SendAsync(endPoint, payload);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// free
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Free(bool disposing)
        {
            this._socket.Close();
            this._socket = null;
            this._pool = null;
        }
        #endregion

        /// <summary>
        /// 用于异步发送的<see cref="SocketAsyncEventArgs"/>对象池
        /// </summary>
        private class AsyncSendPool : ISAEAPool
        {
            #region 私有成员
            /// <summary>
            /// 对象池最大数量
            /// </summary>
            private const int MAXPOOLSIZE = 3000;
            /// <summary>
            /// 消息缓冲区大小
            /// </summary>
            private readonly int _messageBufferSize;
            /// <summary>
            /// 当前对象池所属的socket对象
            /// </summary>
            private readonly Socket _socket = null;
            /// <summary>
            /// 当前异步socket操作对象集合
            /// </summary>
            private readonly ConcurrentStack<SocketAsyncEventArgs> _stack = new ConcurrentStack<SocketAsyncEventArgs>();
            #endregion

            #region 构造函数
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="messageBufferSize">消息缓冲区大小</param>
            /// <param name="socket">当前对象池所属的socket对象</param>
            public AsyncSendPool(int messageBufferSize, Socket socket)
            {
                if (socket == null) throw new ArgumentNullException("socket");
                this._messageBufferSize = messageBufferSize;
                this._socket = socket;
            }
            #endregion

            #region ISAEAPool 成员
            /// <summary>
            /// 从对象池中获取新的socket异步操作对象
            /// </summary>
            /// <returns></returns>
            public SocketAsyncEventArgs GetSocketAsyncEventArgs()
            {
                SocketAsyncEventArgs e;
                if (this._stack.TryPop(out e)) return e;

                e = new SocketAsyncEventArgs();
                e.SetBuffer(new byte[this._messageBufferSize], 0, this._messageBufferSize);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.SendCompleted);
                return e;
            }
            /// <summary>
            /// 从对象池中释放指定socket异步操作对象
            /// </summary>
            /// <param name="e"></param>
            public void ReleaseSocketAsyncEventArgs(SocketAsyncEventArgs e)
            {
                if (this._stack.Count >= MAXPOOLSIZE)
                {
                    e.Completed -= new EventHandler<SocketAsyncEventArgs>(this.SendCompleted);
                    e.Dispose();
                    return;
                }

                this._stack.Push(e);
            }
            #endregion

            #region 私有成员
            /// <summary>
            /// 发送完成后释放资源
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void SendCompleted(object sender, SocketAsyncEventArgs e)
            {
                this.ReleaseSocketAsyncEventArgs(e);
            }
            #endregion

            #region 公有方法
            /// <summary>
            /// 异步发送数据到指定终端
            /// </summary>
            /// <param name="endPoint">终端地址</param>
            /// <param name="payload">待发送的数据信息</param>
            /// <exception cref="ArgumentNullException">endPoint is null</exception>
            /// <exception cref="ArgumentNullException">payload is null or empty</exception>
            /// <exception cref="ArgumentOutOfRangeException">payload length大于messageBufferSize</exception>
            public void SendAsync(EndPoint endPoint, byte[] payload)
            {
                if (endPoint == null) throw new ArgumentNullException("endPoint");
                if (payload == null || payload.Length == 0) throw new ArgumentNullException("payload");
                if (payload.Length > this._messageBufferSize) throw new ArgumentOutOfRangeException("payload.Length", "payload length大于messageBufferSize");

                var e = this.GetSocketAsyncEventArgs();
                e.RemoteEndPoint = endPoint;

                Buffer.BlockCopy(payload, 0, e.Buffer, 0, payload.Length);
                e.SetBuffer(0, payload.Length);

                if (!this._socket.SendToAsync(e)) this.ReleaseSocketAsyncEventArgs(e);
            }
            #endregion
        }
    }
}