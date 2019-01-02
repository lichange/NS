using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using NS.Framework.RocketSocket.SocketBase;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// socket 通信客户端基类-抽象类
    /// </summary>
    /// <typeparam name="TResponse">客户端响应的数据类型约束</typeparam>
    public abstract class BaseSocketClient<TResponse> : SocketBase.BaseHost where TResponse : class, Response.IResponse
    {
        #region 私有成员
        /// <summary>
        /// 请求序列
        /// </summary>
        private int _requestSeqId = 0;
        /// <summary>
        /// 当前客户端的通信协议
        /// </summary>
        private readonly Protocol.IProtocol<TResponse> _protocol = null;
        /// <summary>
        /// 发送信息超时设置
        /// </summary>
        private readonly int _millisecondsSendTimeout;
        /// <summary>
        /// 接收信息超时设置
        /// </summary>
        private readonly int _millisecondsReceiveTimeout;

        private readonly PendingSendQueue _pendingQueue = null;
        private readonly RequestCollection _requestCollection = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="protocol"></param>
        public BaseSocketClient(Protocol.IProtocol<TResponse> protocol)
            : this(protocol, 8192, 8192, 6000, 6000)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="protocol">消息协议</param>
        /// <param name="socketBufferSize">socket缓冲区大小</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        /// <param name="millisecondsSendTimeout">发送信息超时设置</param>
        /// <param name="millisecondsReceiveTimeout">接收信息超时设置</param>
        /// <exception cref="ArgumentNullException">protocol is null</exception>
        public BaseSocketClient(Protocol.IProtocol<TResponse> protocol,
            int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(socketBufferSize, messageBufferSize)
        {
            if (protocol == null) throw new ArgumentNullException("protocol");
            this._protocol = protocol;

            this._millisecondsSendTimeout = millisecondsSendTimeout;
            this._millisecondsReceiveTimeout = millisecondsReceiveTimeout;

            this._pendingQueue = new PendingSendQueue(this, millisecondsSendTimeout);
            this._requestCollection = new RequestCollection(this, millisecondsReceiveTimeout);
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// 发送超时毫秒数
        /// </summary>
        public int MillisecondsSendTimeout
        {
            get { return this._millisecondsSendTimeout; }
        }
        /// <summary>
        /// 接收超时毫秒数
        /// </summary>
        public int MillisecondsReceiveTimeout
        {
            get { return this._millisecondsReceiveTimeout; }
        }
        #endregion

        #region 保护方法
        /// <summary>
        /// 处理未知的response
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="response"></param>
        protected virtual void HandleUnknowResponse(IConnection connection, TResponse response)
        {
        }
        /// <summary>
        /// 当接收到服务器端响应时的事件处理
        /// </summary>
        /// <param name="connection">客户端与服务器连接对象</param>
        /// <param name="response">响应信息</param>
        protected virtual void OnResponse(IConnection connection, TResponse response)
        {
        }
        /// <summary>
        /// 当请求信息发送成功时的事件处理
        /// </summary>
        /// <param name="connection">客户端与服务器连接对象</param>
        /// <param name="request">请求信息</param>
        protected virtual void OnSendSucess(IConnection connection, Request<TResponse> request)
        {
        }
        /// <summary>
        /// 当请求信息发送失败时的事件处理
        /// </summary>
        /// <param name="connection">客户端与服务器连接对象</param>
        /// <param name="request">请求信息</param>
        protected virtual void OnSendFailed(IConnection connection, Request<TResponse> request)
        {
            this.Send(request);
        }
        /// <summary>
        /// 当请求信息发送超时的事件处理
        /// </summary>
        /// <param name="request">请求信息</param>
        protected virtual void OnSendTimeout(Request<TResponse> request)
        {
        }
        /// <summary>
        /// 当响应信息接收超时时的事件处理
        /// </summary>
        /// <param name="connection">客户端与服务器连接对象</param>
        /// <param name="request">请求信息</param>
        protected virtual void OnReceiveTimeout(IConnection connection, Request<TResponse> request)
        {
        }
        /// <summary>
        /// 发送请求-抽象方法
        /// </summary>
        /// <param name="request">请求信息</param>
        protected abstract void Send(Request<TResponse> request);

        /// <summary>
        /// 将待发送的请求信息入列-请求队列
        /// </summary>
        /// <param name="request">请求信息</param>
        protected void EnqueueToPendingQueue(Request<TResponse> request)
        {
            this._pendingQueue.Enqueue(request);
        }

        /// <summary>
        /// 从发送队列中取出一个待发送的信息
        /// </summary>
        /// <returns></returns>
        protected Request<TResponse> DequeueFromPendingQueue()
        {
            return this._pendingQueue.Dequeue();
        }
        /// <summary>
        /// 取出发送队列中所有的消息
        /// </summary>
        /// <returns></returns>
        protected Request<TResponse>[] DequeueAllFromPendingQueue()
        {
            return this._pendingQueue.DequeueAll();
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 产生不重复的seqID
        /// </summary>
        /// <returns></returns>
        public int NextRequestSeqID()
        {
            return Interlocked.Increment(ref this._requestSeqId) & 0x7fffffff;
        }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 当终端连接到服务器宿主时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
            connection.BeginReceive();//异步开始接收数据
        }
        /// <summary>
        /// 开始发送消息时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        protected override void OnStartSending(IConnection connection, Packet packet)
        {
            base.OnStartSending(connection, packet);
            var request = packet as Request<TResponse>;
            if (request != null) this._requestCollection.Add(request);
        }
        /// <summary>
        /// 消息发送完成时的回调事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);

            var request = e.Packet as Request<TResponse>;
            if (request == null) return;

            if (e.Status == SendCallbackStatus.Success)
            {
                request.CurrConnection = connection;
                request.SentTime = DateTime.UtcNow;
                this.OnSendSucess(connection, request);
                return;
            }

            request.CurrConnection = null;
            request.SentTime = DateTime.MaxValue;
            if (this._requestCollection.Remove(request.SeqID) == null) return;

            if (DateTime.UtcNow.Subtract(request.BeginTime).TotalMilliseconds < this._millisecondsSendTimeout)
            {
                this.OnSendFailed(connection, request);
                return;
            }

            //time out
            this.OnSendTimeout(request);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var rex = new RequestException(RequestException.Errors.PendingSendTimeout, request.CmdName);
                try { request.SetException(rex); }
                catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
            });
        }
        /// <summary>
        /// 收到消息时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnMessageReceived(IConnection connection, MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(connection, e);

            int readlength;
            TResponse response = null;
            try
            {
                response = this._protocol.FindResponse(connection, e.Buffer, out readlength);
            }
            catch (Exception ex)
            {
                this.OnError(connection, ex);
                connection.BeginDisconnect(ex);
                e.SetReadlength(e.Buffer.Count);
                return;
            }

            if (response != null)
            {
                this.OnResponse(connection, response);

                var request = this._requestCollection.Remove(response.SeqID);
                if (request == null)
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try { this.HandleUnknowResponse(connection, response); }
                        catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
                    });
                else
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try { request.SetResult(response); }
                        catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
                    });
            }
            e.SetReadlength(readlength);
        }
        /// <summary>
        /// 当终端与服务器宿主断开连接时的事件处理函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);

            var arrRemoved = this._requestCollection.Remove(connection);
            if (arrRemoved.Length == 0) return;

            var ex2 = ex ?? new SocketException((int)SocketError.Disconnecting);
            for (int i = 0, l = arrRemoved.Length; i < l; i++)
            {
                var r = arrRemoved[i]; if (r == null) continue;
                ThreadPool.QueueUserWorkItem(c =>
                {
                    try { r.SetException(ex2); }
                    catch (Exception ex3) { SocketBase.Log.Trace.Error(ex.Message, ex3); }
                });
            }
        }
        #endregion

        #region 请求消息发送队列
        /// <summary>
        /// 请求消息发送队列
        /// </summary>
        private class PendingSendQueue
        {
            #region 私有成员
            /// <summary>
            /// 客户端对象
            /// </summary>
            private readonly BaseSocketClient<TResponse> _client = null;
            /// <summary>
            /// 超时设置
            /// </summary>
            private readonly int _timeout;
            /// <summary>
            /// 计时器
            /// </summary>
            private readonly Timer _timer = null;
            /// <summary>
            /// 请求消息的存储队列
            /// </summary>
            private readonly ConcurrentQueue<Request<TResponse>> _queue = new ConcurrentQueue<Request<TResponse>>();
            #endregion

            #region 构造函数
            /// <summary>
            /// 析构函数函数
            /// </summary>
            ~PendingSendQueue()
            {
                this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                this._timer.Dispose();
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="client"></param>
            /// <param name="millisecondsSendTimeout"></param>
            public PendingSendQueue(BaseSocketClient<TResponse> client, int millisecondsSendTimeout)
            {
                this._client = client;
                this._timeout = millisecondsSendTimeout;
                this._timer = new Timer(_ =>
                {
                    this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                    this.Loop();
                    this._timer.Change(1000, 0);
                }, null, 1000, 0);
            }
            #endregion

            #region 公共方法
            /// <summary>
            /// 入列
            /// </summary>
            /// <param name="request"></param>
            /// <exception cref="ArgumentNullException">request is null</exception>
            public void Enqueue(Request<TResponse> request)
            {
                if (request == null) throw new ArgumentNullException("request");
                this._queue.Enqueue(request);
            }
            /// <summary>
            /// 出列
            /// </summary>
            /// <returns></returns>
            public Request<TResponse> Dequeue()
            {
                Request<TResponse> request;
                if (this._queue.TryDequeue(out request)) return request;
                return null;
            }
            /// <summary>
            /// 出列全部
            /// </summary>
            /// <returns></returns>
            public Request<TResponse>[] DequeueAll()
            {
                int count = this._queue.Count;
                List<Request<TResponse>> list = null;
                while (count-- > 0)
                {
                    Request<TResponse> request;
                    if (this._queue.TryDequeue(out request))
                    {
                        if (list == null) list = new List<Request<TResponse>>();
                        list.Add(request);
                    }
                    else break;
                }

                if (list != null) return list.ToArray();
                return new Request<TResponse>[0];
            }
            #endregion

            #region 私有方法
            /// <summary>
            /// 循环队列
            /// </summary>
            private void Loop()
            {
                var dtNow = DateTime.UtcNow;
                List<Request<TResponse>> listSend = null;
                List<Request<TResponse>> listTimeout = null;

                int count = this._queue.Count;
                while (count-- > 0)
                {
                    Request<TResponse> request;
                    if (this._queue.TryDequeue(out request))
                    {
                        if (dtNow.Subtract(request.BeginTime).TotalMilliseconds < this._timeout)
                        {
                            if (listSend == null) listSend = new List<Request<TResponse>>();
                            listSend.Add(request); continue;
                        }

                        if (listTimeout == null) listTimeout = new List<Request<TResponse>>();
                        listTimeout.Add(request);
                    }
                    else break;
                }

                if (listSend != null)
                {
                    for (int i = 0, l = listSend.Count; i < l; i++) this._client.Send(listSend[i]);
                }

                if (listTimeout != null)
                {
                    for (int i = 0, l = listTimeout.Count; i < l; i++)
                    {
                        var r = listTimeout[i];
                        this._client.OnSendTimeout(r);
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try { r.SetException(new RequestException(RequestException.Errors.PendingSendTimeout, r.CmdName)); }
                            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
                        });
                    }
                }
            }
            #endregion
        }
        #endregion

        #region 请求超时的信息集合
        /// <summary>
        /// 请求超时的信息集合
        /// </summary>
        private class RequestCollection
        {
            #region 私有成员
            /// <summary>
            /// 客户端对象
            /// </summary>
            private readonly BaseSocketClient<TResponse> _client = null;
            /// <summary>
            /// 超时设置
            /// </summary>
            private readonly int _timeout;
            /// <summary>
            /// 计时器
            /// </summary>
            private readonly Timer _timer = null;
            /// <summary>
            /// 请求超时的消息的存储字典
            /// </summary>
            private readonly ConcurrentDictionary<int, Request<TResponse>> _dic = new ConcurrentDictionary<int, Request<TResponse>>();
            #endregion

            #region 构造函数
            /// <summary>
            /// 析构函数
            /// </summary>
            ~RequestCollection()
            {
                this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                this._timer.Dispose();
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="client"></param>
            /// <param name="millisecondsReceiveTimeout"></param>
            public RequestCollection(BaseSocketClient<TResponse> client, int millisecondsReceiveTimeout)
            {
                this._client = client;
                this._timeout = millisecondsReceiveTimeout;

                this._timer = new Timer(_ =>
                {
                    this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                    this.Loop();
                    this._timer.Change(1000, 0);
                }, null, 1000, 0);
            }
            #endregion

            #region 公共方法
            /// <summary>
            /// 添加请求超时的消息到字典中
            /// </summary>
            /// <param name="request"></param>
            public void Add(Request<TResponse> request)
            {
                this._dic.TryAdd(request.SeqID, request);
            }

            /// <summary>
            /// 字典中移除请求超时的消息
            /// </summary>
            /// <param name="seqID"></param>
            /// <returns></returns>
            public Request<TResponse> Remove(int seqID)
            {
                Request<TResponse> removed;
                this._dic.TryRemove(seqID, out removed);
                return removed;
            }
            /// <summary>
            /// 清空字典中的所有的请求超时的消息
            /// </summary>
            /// <param name="connection"></param>
            /// <returns></returns>
            public Request<TResponse>[] Remove(IConnection connection)
            {
                var items = this._dic.Where(c => c.Value.CurrConnection == connection).ToArray();
                var arrRemoved = new Request<TResponse>[items.Length];
                for (int i = 0, l = items.Length; i < l; i++)
                {
                    Request<TResponse> removed;
                    if (this._dic.TryRemove(items[i].Key, out removed)) arrRemoved[i] = removed;
                }
                return arrRemoved;
            }
            #endregion

            #region 私有方法
            /// <summary>
            /// 循环处理字典中包含的请求超时的消息
            /// </summary>
            private void Loop()
            {
                if (this._dic.Count == 0) return;

                var dtNow = DateTime.UtcNow;
                var arrTimeout = this._dic.Where(c => dtNow.Subtract(c.Value.SentTime).TotalMilliseconds >
                    (c.Value.MillisecondsReceiveTimeout > 0 ? c.Value.MillisecondsReceiveTimeout : this._timeout)).ToArray();
                if (arrTimeout.Length == 0) return;

                for (int i = 0, l = arrTimeout.Length; i < l; i++)
                {
                    Request<TResponse> removed;
                    if (this._dic.TryRemove(arrTimeout[i].Key, out removed))
                    {
                        this._client.OnReceiveTimeout(removed.CurrConnection, removed);

                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try { removed.SetException(new RequestException(RequestException.Errors.ReceiveTimeout, removed.CmdName)); }
                            catch (Exception ex) { SocketBase.Log.Trace.Error(ex.Message, ex); }
                        });
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}