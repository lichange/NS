using System;
using System.Net;
using System.Net.Sockets;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// socket监听器
    /// </summary>
    public sealed class SocketListener : ISocketListener
    {
        #region 私有成员
        /// <summary>
        /// 当前监听所在的服务器宿主
        /// </summary>
        private readonly SocketBase.IHost _host = null;
        /// <summary>
        /// 
        /// </summary>
        private const int BACKLOG = 100;
        /// <summary>
        /// 当前接入服务器端的socket对象
        /// </summary>
        private Socket _socket = null;
        /// <summary>
        /// socket异步操作对象
        /// </summary>
        private readonly SocketAsyncEventArgs _ae = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">监听器名称</param>
        /// <param name="endPoint">监听ip地址及端口号</param>
        /// <param name="host">监听的宿主</param>
        /// <exception cref="ArgumentNullException">name is null or empty</exception>
        /// <exception cref="ArgumentNullException">endPoint is null</exception>
        /// <exception cref="ArgumentNullException">host is null</exception>
        public SocketListener(string name, IPEndPoint endPoint, SocketBase.IHost host)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (endPoint == null) throw new ArgumentNullException("endPoint");
            if (host == null) throw new ArgumentNullException("host");

            this.Name = name;
            this.EndPoint = endPoint;
            this._host = host;

            this._ae = new SocketAsyncEventArgs();
            this._ae.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
        }
        #endregion

        #region 监听器成员
        /// <summary>
        /// 监听器接收新的连接时的事件处理委托
        /// </summary>
        public event Action<ISocketListener, SocketBase.IConnection> Accepted;
        /// <summary>
        /// 获取监听器名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
        /// <summary>
        /// 监听器监听的ip地址及端口号
        /// </summary>
        public EndPoint EndPoint
        {
            get;
            private set;
        }
        /// <summary>
        /// 开始监听
        /// </summary>
        public void Start()
        {
            if (this._socket == null)
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._socket.Bind(this.EndPoint);
                this._socket.Listen(BACKLOG);

                this.AcceptAsync(this._socket);
            }
        }
        /// <summary>
        /// 停止监听
        /// </summary>
        public void Stop()
        {
            if (this._socket != null)
            {
                this._socket.Close();
                this._socket = null;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 开始异步接收连接的处理函数
        /// </summary>
        /// <param name="socket">负责监听和接入连接的socket套接字对象</param>
        private void AcceptAsync(Socket socket)
        {
            if (socket == null) return;

            bool asyncCompleted = true;
            try { asyncCompleted = this._socket.AcceptAsync(this._ae); }
            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }

            if (!asyncCompleted) this.AcceptAsyncCompleted(this, this._ae);
        }

        /// <summary>
        /// 当异步接入连接完成时执行的事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket acceptedSocket = null;
            if (e.SocketError == SocketError.Success) acceptedSocket = e.AcceptSocket;
            e.AcceptSocket = null;

            if (acceptedSocket != null)
            {
                acceptedSocket.NoDelay = true;
                acceptedSocket.ReceiveBufferSize = this._host.SocketBufferSize;
                acceptedSocket.SendBufferSize = this._host.SocketBufferSize;
                acceptedSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                this.Accepted(this, new SocketBase.DefaultConnection(this._host.NextConnectionID(), acceptedSocket, this._host));
            }

            //继续接收-递归
            this.AcceptAsync(this._socket);
        }
        #endregion
    }
}