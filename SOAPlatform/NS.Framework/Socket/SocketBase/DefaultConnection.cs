using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 默认实现-与服务器链接的接口实现
    /// </summary>
    public class DefaultConnection : IConnection
    {
        #region 私有成员
        /// <summary>
        /// 当前链接的有效性：1、有效。
        /// </summary>
        private int _active = 1;//1表示有效
        /// <summary>
        /// socket服务器宿主对象
        /// </summary>
        private IHost _host = null;
        /// <summary>
        /// 消息缓冲区
        /// </summary>
        private readonly int _messageBufferSize;
        /// <summary>
        /// socket-服务器与终端间通信的安全套接字
        /// </summary>
        private Socket _socket = null;
        /// <summary>
        /// 发送消息的socket异步套接字操作对象
        /// </summary>
        private SocketAsyncEventArgs _saeSend = null;
        /// <summary>
        /// 发送消息的队列
        /// </summary>
        private SendQueue _sendQueue = null;
        /// <summary>
        /// 当前待发送或发送中的数据包对象
        /// </summary>
        private Packet _currSendingPacket = null;
        /// <summary>
        /// 接收消息的socket异步套接字操作对象
        /// </summary>
        private SocketAsyncEventArgs _saeReceive = null;
        /// <summary>
        /// 接收消息的临时存储内存流对象
        /// </summary>
        private MemoryStream _tsStream = null;//temporary storage stream for recieving
        /// <summary>
        /// 接收消息
        /// </summary>
        private int _isReceiving = 0;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionID">链接id</param>
        /// <param name="socket">socket对象</param>
        /// <param name="host">宿主对象</param>
        /// <exception cref="ArgumentNullException">socket对象 null</exception>
        /// <exception cref="ArgumentNullException">宿主对象 null</exception>
        public DefaultConnection(long connectionID, Socket socket, IHost host)
        {
            if (socket == null) throw new ArgumentNullException("socket");
            if (host == null) throw new ArgumentNullException("host");

            this.ConnectionID = connectionID;
            this._socket = socket;
            this._host = host;
            this._messageBufferSize = host.MessageBufferSize;

            try//获取套接字地址信息
            {
                this.LocalEndPoint = (IPEndPoint)socket.LocalEndPoint;
                this.RemoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
            }
            catch (Exception ex) { Log.Trace.Error("get socket endPoint error.", ex); }

            //初始化数据发送socket异步套接字操作对象
            this._saeSend = host.GetSocketAsyncEventArgs();
            this._saeSend.Completed += new EventHandler<SocketAsyncEventArgs>(this.SendAsyncCompleted);
            this._sendQueue = new SendQueue();

            //初始化接收数据socket异步套接字操作对象
            this._saeReceive = host.GetSocketAsyncEventArgs();
            this._saeReceive.Completed += new EventHandler<SocketAsyncEventArgs>(this.ReceiveAsyncCompleted);
        }
        #endregion

        #region 连接属性
        /// <summary>
        /// 数据包开始发送时的事件处理委托
        /// </summary>
        public event StartSendingHandler StartSending;
        /// <summary>
        /// 数据包开始发送完成时回调的事件处理委托
        /// </summary>
        public event SendCallbackHandler SendCallback;
        /// <summary>
        /// 接收到消息时的事件处理委托
        /// </summary>
        public event MessageReceivedHandler MessageReceived;
        /// <summary>
        /// 连接断开事件
        /// </summary>
        public event DisconnectedHandler Disconnected;
        /// <summary>
        /// 连接失败的事件处理委托
        /// </summary>
        public event ErrorHandler Error;

        /// <summary>
        /// 获取当前连接是否有效
        /// </summary>
        public bool Active { get { return Thread.VolatileRead(ref this._active) == 1; } }
        /// <summary>
        /// 获取当前连接的唯一标识
        /// </summary>
        public long ConnectionID { get; private set; }
        /// <summary>
        /// 获取本地IP地址
        /// </summary>
        public IPEndPoint LocalEndPoint { get; private set; }
        /// <summary>
        /// 获取远程IP地址
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; private set; }
        /// <summary>
        /// 获取或设置与用户数据
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="packet"></param>
        public void BeginSend(Packet packet)
        {
            this.SendPacketInternal(packet);
        }

        /// <summary>
        /// 异步接收数据
        /// </summary>
        public void BeginReceive()
        {
            if (Interlocked.CompareExchange(ref this._isReceiving, 1, 0) == 0) this.ReceiveInternal(this._saeReceive);
        }

        /// <summary>
        /// 异步断开连接
        /// </summary>
        /// <param name="ex"></param>
        public void BeginDisconnect(Exception ex = null)
        {
            if (Interlocked.CompareExchange(ref this._active, 0, 1) == 1) this.DisconnectInternal(ex);
        }

        #endregion

        #region 保护成员
        /// <summary>
        /// 释放连接资源
        /// </summary>
        protected virtual void Free()
        {
            var arrPacket = this._sendQueue.Close();
            this._sendQueue = null;
            if (arrPacket != null && arrPacket.Length > 0)
            {
                foreach (var packet in arrPacket)
                    this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed));
            }

            this._saeSend.Completed -= new EventHandler<SocketAsyncEventArgs>(this.SendAsyncCompleted);
            this._saeSend.UserToken = null;
            this._host.ReleaseSocketAsyncEventArgs(this._saeSend);
            this._saeSend = null;

            this._saeReceive.Completed -= new EventHandler<SocketAsyncEventArgs>(this.ReceiveAsyncCompleted);
            this._saeReceive.UserToken = null;
            this._host.ReleaseSocketAsyncEventArgs(this._saeReceive);
            this._saeReceive = null;

            this._socket = null;
            this._host = null;
        }
        #endregion

        #region 内部方法

        #region 事件通知
        /// <summary>
        /// 当开始发送数据包时的事件通知
        /// </summary>
        /// <param name="packet"></param>
        private void OnStartSending(Packet packet)
        {
            if (this.StartSending != null) this.StartSending(this, packet);
        }
        /// <summary>
        /// 当发送数据包完成时的事件通知
        /// </summary>
        /// <param name="e"></param>
        private void OnSendCallback(SendCallbackEventArgs e)
        {
            if (e.Status != SendCallbackStatus.Success) e.Packet.SentSize = 0;
            if (this.SendCallback != null) this.SendCallback(this, e);
        }
        /// <summary>
        ///当收到数据包时的事件通知
        /// </summary>
        /// <param name="e"></param>
        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            if (this.MessageReceived != null) this.MessageReceived(this, e);
        }
        /// <summary>
        /// 当连接断开后事件通知
        /// </summary>
        private void OnDisconnected(Exception ex)
        {
            if (this.Disconnected != null) this.Disconnected(this, ex);
        }
        /// <summary>
        /// 当连接失败时的事件通知
        /// </summary>
        /// <param name="ex"></param>
        private void OnError(Exception ex)
        {
            if (this.Error != null) this.Error(this, ex);
        }
        #endregion

        #region 发送数据

        /// <summary>
        /// 发送数据包方法
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <exception cref="ArgumentNullException">数据包 null</exception>
        private void SendPacketInternal(Packet packet)
        {
            var e = this._saeSend;
            var queue = this._sendQueue;
            if (!this.Active || e == null || queue == null)
            {
                this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed)); return;
            }

            switch (queue.TrySend(packet))
            {
                case SendResult.Closed:
                    this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed)); break;
                case SendResult.SendCurr:
                    this.OnStartSending(packet);
                    this.SendPacketInternal(packet, e);
                    break;
            }
        }

        /// <summary>
        /// 发送数据包方法
        /// </summary>
        /// <param name="packet">数据包</param>
        /// <param name="e">socket 发送异步套接字操作对象</param>
        private void SendPacketInternal(Packet packet, SocketAsyncEventArgs e)
        {
            this._currSendingPacket = packet;

            //按_messageBufferSize大小分块传输
            var length = Math.Min(packet.Payload.Length - packet.SentSize, this._messageBufferSize);

            var completedAsync = true;
            try
            {
                //copy data to send buffer
                Buffer.BlockCopy(packet.Payload, packet.SentSize, e.Buffer, 0, length);
                e.SetBuffer(0, length);
                completedAsync = this._socket.SendAsync(e);
            }
            catch (Exception ex)
            {
                this.BeginDisconnect(ex);
                this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed));
                this.OnError(ex);
            }

            if (!completedAsync) this.SendAsyncCompleted(this, e);
        }

        /// <summary>
        /// 数据发送完成时的异步回调通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            var packet = this._currSendingPacket;
            if (packet == null)
            {
                var ex = new Exception(string.Concat("未知的错误, connection state:",
                    this.Active.ToString(),
                    " conectionID:",
                    this.ConnectionID.ToString(),
                    " remote address:",
                    this.RemoteEndPoint.ToString()));
                this.OnError(ex);
                this.BeginDisconnect(ex);
                return;
            }

            //发送失败-出现错误
            if (e.SocketError != SocketError.Success)
            {
                this.BeginDisconnect(new SocketException((int)e.SocketError));
                this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed));
                return;
            }

            packet.SentSize += e.BytesTransferred;

            if (e.Offset + e.BytesTransferred < e.Count)
            {
                //循环发送-直到数据发送完成
                var completedAsync = true;
                try
                {
                    e.SetBuffer(e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred - e.Offset);
                    completedAsync = this._socket.SendAsync(e);
                }
                catch (Exception ex)
                {
                    this.BeginDisconnect(ex);
                    this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Failed));
                    this.OnError(ex);
                }

                if (!completedAsync) this.SendAsyncCompleted(sender, e);
            }
            else
            {
                if (packet.IsSent())
                {
                    this._currSendingPacket = null;
                    this.OnSendCallback(new SendCallbackEventArgs(packet, SendCallbackStatus.Success));

                    //send next packet
                    var queue = this._sendQueue;
                    if (this.Active && queue != null)
                    {
                        Packet nextPacket = queue.TrySendNext();
                        if (nextPacket != null)
                        {
                            this.OnStartSending(nextPacket);
                            this.SendPacketInternal(nextPacket, e);
                        }
                    }
                }
                else this.SendPacketInternal(packet, e);//继续发送数据包
            }
        }

        #endregion

        #region 接收数据
        /// <summary>
        /// 接收数据信息
        /// </summary>
        /// <param name="e">接收数据的socket异步套接字操作对象</param>
        private void ReceiveInternal(SocketAsyncEventArgs e)
        {
            if (!this.Active || e == null) return;

            bool completedAsync = true;
            try { completedAsync = this._socket.ReceiveAsync(e); }
            catch (Exception ex)
            {
                this.BeginDisconnect(ex);
                this.OnError(ex);
            }

            if (!completedAsync) this.ReceiveAsyncCompleted(this, e);
        }

        /// <summary>
        /// 当接收数据包完成时的回调事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                this.BeginDisconnect(new SocketException((int)e.SocketError)); return;
            }
            if (e.BytesTransferred < 1)
            {
                this.BeginDisconnect(); return;
            }

            ArraySegment<byte> buffer;
            var ts = this._tsStream;
            if (ts == null || ts.Length == 0) buffer = new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred);
            else
            {
                ts.Write(e.Buffer, 0, e.BytesTransferred);
                buffer = new ArraySegment<byte>(ts.GetBuffer(), 0, (int)ts.Length);
            }

            this.OnMessageReceived(new MessageReceivedEventArgs(buffer, this.MessageProcessCallback));
        }

        /// <summary>
        /// 消息处理过程中的回掉事件
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="readlength"></param>
        /// <exception cref="ArgumentOutOfRangeException">readlength less than 0 or greater than payload.Count.</exception>
        private void MessageProcessCallback(ArraySegment<byte> payload, int readlength)
        {
            if (readlength < 0 || readlength > payload.Count)
                throw new ArgumentOutOfRangeException("readlength", "readlength less than 0 or greater than payload.Count.");

            var ts = this._tsStream;
            if (readlength == 0)
            {
                if (ts == null) this._tsStream = ts = new MemoryStream(this._messageBufferSize);
                else ts.SetLength(0);

                ts.Write(payload.Array, payload.Offset, payload.Count);
                this.ReceiveInternal(this._saeReceive);
                return;
            }

            if (readlength == payload.Count)
            {
                if (ts != null) ts.SetLength(0);
                this.ReceiveInternal(this._saeReceive);
                return;
            }

            //粘包处理
            this.OnMessageReceived(new MessageReceivedEventArgs(
                new ArraySegment<byte>(payload.Array, payload.Offset + readlength, payload.Count - readlength),
                this.MessageProcessCallback));
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        private void DisconnectInternal(Exception ex)
        {
            try
            {
                this._socket.Shutdown(SocketShutdown.Both);
                this._socket.BeginDisconnect(false, this.DisconnectCallback, ex);
            }
            catch (Exception ex2)
            {
                Log.Trace.Error(ex2.Message, ex2);
                this.DisconnectCallback(null);
            }
        }
        /// <summary>
        /// 断开连接的回调日期
        /// </summary>
        /// <param name="result"></param>
        private void DisconnectCallback(IAsyncResult result)
        {
            if (result != null)
            {
                try
                {
                    this._socket.EndDisconnect(result);
                    this._socket.Close();
                }
                catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
            }

            //fire disconnected.
            this.OnDisconnected(result == null ? null : result.AsyncState as Exception);

            this.Free();
        }
        #endregion

        #endregion

        #region 私有类定义-消息发送队列及发送结果枚举
        /// <summary>
        /// 数据包发送队列
        /// </summary>
        private class SendQueue
        {
            #region Private Members
            private bool _isSending = false, _isClosed = false;
            private readonly Queue<Packet> _queue = new Queue<Packet>();
            #endregion

            #region Public Methods
            /// <summary>
            /// 开始发送
            /// </summary>
            /// <param name="packet"></param>
            /// <returns></returns>
            public SendResult TrySend(Packet packet)
            {
                lock (this)
                {
                    if (this._isClosed) return SendResult.Closed;

                    if (this._isSending)
                    {
                        if (this._queue.Count < 500)
                        {
                            this._queue.Enqueue(packet);
                            return SendResult.Enqueued;
                        }
                    }
                    else
                    {
                        this._isSending = true;
                        return SendResult.SendCurr;
                    }
                }

                Thread.Sleep(1);
                return this.TrySend(packet);
            }
            /// <summary>
            ///发送下一数据包
            /// </summary>
            /// <returns></returns>
            public Packet TrySendNext()
            {
                lock (this)
                {
                    if (this._queue.Count == 0)
                    {
                        this._isSending = false;
                        return null;
                    }

                    this._isSending = true;
                    return this._queue.Dequeue();
                }
            }
            /// <summary>
            /// 关闭队列
            /// </summary>
            /// <returns></returns>
            public Packet[] Close()
            {
                lock (this)
                {
                    if (this._isClosed) return null;
                    this._isClosed = true;

                    var packets = this._queue.ToArray();
                    this._queue.Clear();
                    return packets;
                }
            }
            #endregion
        }

        /// <summary>
        /// 发送结果
        /// </summary>
        private enum SendResult : byte
        {
            /// <summary>
            /// 已关闭
            /// </summary>
            Closed = 1,
            /// <summary>
            /// 发送中
            /// </summary>
            SendCurr = 2,
            /// <summary>
            /// 已入列
            /// </summary>
            Enqueued = 3
        }

        #endregion
    }
}