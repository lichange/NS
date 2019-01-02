using System;
using System.Net;
using System.Net.Sockets;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// socket连接心跳检测器
    /// </summary>
    public sealed class SocketConnector
    {
        #region 成员定义
        /// <summary>
        /// 心态检测的服务器节点名称
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// 获取服务器节点的终结点地址
        /// </summary>
        private readonly EndPoint EndPoint;
        /// <summary>
        /// 获取服务器节点的宿主
        /// </summary>
        private readonly SocketBase.IHost Host = null;
        /// <summary>
        /// 当与服务器连接成功时的事件委托
        /// </summary>
        private Action<SocketConnector, SocketBase.IConnection> _onConnected;
        /// <summary>
        /// 当与服务器断开连接成功时的事件委托
        /// </summary>
        private Action<SocketConnector, SocketBase.IConnection> _onDisconnected;
        /// <summary>
        /// 当前心跳检测是否停止
        /// </summary>
        private volatile bool _isStop = false;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <param name="host"></param>
        /// <param name="onConnected"></param>
        /// <param name="onDisconnected"></param>
        public SocketConnector(string name,
            EndPoint endPoint,
            SocketBase.IHost host,
            Action<SocketConnector, SocketBase.IConnection> onConnected,
            Action<SocketConnector, SocketBase.IConnection> onDisconnected)
        {
            this.Name = name;
            this.EndPoint = endPoint;
            this.Host = host;
            this._onConnected = onConnected;
            this._onDisconnected = onDisconnected;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 开始检测
        /// </summary>
        public void Start()
        {
            BeginConnect(this.EndPoint, this.Host, connection =>
            {
                if (this._isStop)
                {
                    if (connection != null) connection.BeginDisconnect(); return;
                }
                if (connection == null)
                {
                    SocketBase.Utils.TaskEx.Delay(new Random().Next(1500, 3000), this.Start); return;
                }
                connection.Disconnected += this.OnDisconnected;
                this._onConnected(this, connection);
            });
        }
        /// <summary>
        /// 停止检测
        /// </summary>
        public void Stop()
        {
            this._isStop = true;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        private void OnDisconnected(SocketBase.IConnection connection, Exception ex)
        {
            connection.Disconnected -= this.OnDisconnected;
            //delay reconnect 20ms ~ 200ms
            if (!this._isStop) SocketBase.Utils.TaskEx.Delay(new Random().Next(20, 200), this.Start);
            //fire disconnected event
            this._onDisconnected(this, connection);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// 开始连接服务器节点
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="host"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException">endPoint is null</exception>
        /// <exception cref="ArgumentNullException">host is null</exception>
        /// <exception cref="ArgumentNullException">callback is null</exception>
        static public void BeginConnect(EndPoint endPoint, SocketBase.IHost host, Action<SocketBase.IConnection> callback)
        {
            if (endPoint == null) throw new ArgumentNullException("endPoint");
            if (host == null) throw new ArgumentNullException("host");
            if (callback == null) throw new ArgumentNullException("callback");

            SocketBase.Log.Trace.Debug(string.Concat("begin connect to ", endPoint.ToString()));

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.BeginConnect(endPoint, ar =>
                {
                    try
                    {
                        socket.EndConnect(ar);
                        socket.NoDelay = true;
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                        socket.ReceiveBufferSize = host.SocketBufferSize;
                        socket.SendBufferSize = host.SocketBufferSize;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            socket.Close();
                            socket.Dispose();
                        }
                        catch { }

                        SocketBase.Log.Trace.Error(ex.Message, ex);
                        callback(null); return;
                    }

                    callback(new SocketBase.DefaultConnection(host.NextConnectionID(), socket, host));
                }, null);
            }
            catch (Exception ex)
            {
                SocketBase.Log.Trace.Error(ex.Message, ex);
                callback(null);
            }
        }
        #endregion
    }
}