using System;
using NS.Framework.RocketSocket.Client.Response;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// thrift socket客户端
    /// </summary>
    public class ThriftClient : PooledSocketClient<ThriftResponse>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ThriftClient()
            : base(new Protocol.ThriftProtocol())
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        public ThriftClient(int socketBufferSize, int messageBufferSize)
            : base(new Protocol.ThriftProtocol(), socketBufferSize, messageBufferSize, 6000, 6000)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        public ThriftClient(int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(new Protocol.ThriftProtocol(),
            socketBufferSize,
            messageBufferSize,
            millisecondsSendTimeout,
            millisecondsReceiveTimeout)
        {
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cmdName"></param>
        /// <param name="seqID"></param>
        /// <param name="payload"></param>
        /// <param name="onException"></param>
        /// <param name="onResult"></param>
        public void Send(string service, string cmdName, int seqID, byte[] payload,
            Action<Exception> onException, Action<byte[]> onResult)
        {
            this.Send(null, service, cmdName, seqID, payload, onException, onResult);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="consistentKey"></param>
        /// <param name="service"></param>
        /// <param name="cmdName"></param>
        /// <param name="seqID"></param>
        /// <param name="payload"></param>
        /// <param name="onException"></param>
        /// <param name="onResult"></param>
        /// <exception cref="ArgumentNullException">payload is null or empty</exception>
        /// <exception cref="ArgumentNullException">onException is null</exception>
        /// <exception cref="ArgumentNullException">onResult is null</exception>
        public void Send(byte[] consistentKey, string service, string cmdName, int seqID, byte[] payload,
            Action<Exception> onException, Action<byte[]> onResult)
        {
            if (payload == null || payload.Length == 0) throw new ArgumentNullException("payload");
            if (onException == null) throw new ArgumentNullException("onException");
            if (onResult == null) throw new ArgumentNullException("onResult");

            base.Send(new Request<Response.ThriftResponse>(consistentKey, seqID, string.Concat(service, ".", cmdName), payload,
                onException, response => onResult(response.Buffer)));
        }
        #endregion
    }
}