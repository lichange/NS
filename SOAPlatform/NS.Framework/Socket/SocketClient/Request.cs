using System;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// 请求对象封装
    /// </summary>
    /// <typeparam name="TResponse">请求对应的响应信息</typeparam>
    public class Request<TResponse> : SocketBase.Packet where TResponse : Response.IResponse
    {
        #region 成员
        /// <summary>
        /// 一致性哈希标识code
        /// </summary>
        public readonly byte[] ConsistentKey = null;
        /// <summary>
        /// seqID-请求消息的序列id
        /// </summary>
        public readonly int SeqID;
        /// <summary>
        /// 请求消息对应的命令名称
        /// </summary>
        public readonly string CmdName;

        /// <summary>
        /// 设置请求响应消息的超时时限
        /// </summary>
        public int MillisecondsReceiveTimeout;

        /// <summary>
        /// 当前请求消息发送的连接通道
        /// </summary>
        internal SocketBase.IConnection CurrConnection = null;
        /// <summary>
        /// 发送时间
        /// </summary>
        internal DateTime SentTime = DateTime.MaxValue;

        /// <summary>
        /// 异常回调
        /// </summary>
        private readonly Action<Exception> _onException = null;
        /// <summary>
        /// 结果回调
        /// </summary>
        private readonly Action<TResponse> _onResult = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seqID">seqID</param>
        /// <param name="cmdName">command name</param>
        /// <param name="payload">发送内容</param>
        /// <param name="onException">异常回调</param>
        /// <param name="onResult">结果回调</param>
        public Request(int seqID, string cmdName, byte[] payload, Action<Exception> onException, Action<TResponse> onResult)
            : this(null, seqID, cmdName, payload, onException, onResult)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="consistentKey">一致性哈希标识code, 可为null</param>
        /// <param name="seqID">seqID</param>
        /// <param name="cmdName">command name</param>
        /// <param name="payload">发送内容</param>
        /// <param name="onException">异常回调</param>
        /// <param name="onResult">结果回调</param>
        /// <exception cref="ArgumentNullException">onException is null</exception>
        /// <exception cref="ArgumentNullException">onResult is null</exception>
        public Request(byte[] consistentKey, int seqID, string cmdName, byte[] payload, Action<Exception> onException, Action<TResponse> onResult)
            : base(payload)
        {
            if (onException == null) throw new ArgumentNullException("onException");
            if (onResult == null) throw new ArgumentNullException("onResult");

            this.ConsistentKey = consistentKey;
            this.SeqID = seqID;
            this.CmdName = cmdName;
            this._onException = onException;
            this._onResult = onResult;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 当出现异常时引发异常通知
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        public bool SetException(Exception ex)
        {
            this._onException(ex);
            return true;
        }
        /// <summary>
        /// 当收到请求响应信息时触发事件处理
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool SetResult(TResponse response)
        {
            this._onResult(response);
            return true;
        }
        #endregion
    }
}