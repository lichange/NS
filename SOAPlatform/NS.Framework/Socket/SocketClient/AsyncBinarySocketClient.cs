using System;
using System.Text;
using System.Threading.Tasks;
using NS.Framework.RocketSocket.Client.Response;
using NS.Framework.RocketSocket.SocketBase.Utils;

namespace NS.Framework.RocketSocket.Client
{
    /// <summary>
    /// 异步socket客户端--二进制流协议时的通信客户端
    /// </summary>
    public class AsyncBinarySocketClient : PooledSocketClient<AsyncBinaryResponse>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public AsyncBinarySocketClient()
            : base(new Protocol.AsyncBinaryProtocol())
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize">socket缓冲区大小</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        public AsyncBinarySocketClient(int socketBufferSize, int messageBufferSize)
            : base(new Protocol.AsyncBinaryProtocol(), socketBufferSize, messageBufferSize, 6000, 6000)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socketBufferSize">socket缓冲区大小</param>
        /// <param name="messageBufferSize">消息缓冲区大小</param>
        /// <param name="millisecondsSendTimeout">发送超时设置</param>
        /// <param name="millisecondsReceiveTimeout">接收超时设置</param>
        public AsyncBinarySocketClient(int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(new Protocol.AsyncBinaryProtocol(),
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cmdName"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        public Task<TResult> Send<TResult>(string cmdName, byte[] payload,
            Func<AsyncBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            return this.Send(null, cmdName, payload, funcResultFactory, asyncState);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="consistentKey"></param>
        /// <param name="cmdName"></param>
        /// <param name="payload"></param>
        /// <param name="funcResultFactory"></param>
        /// <param name="asyncState"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">cmdName is null or empty.</exception>
        /// <exception cref="ArgumentNullException">funcResultFactory is null.</exception>
        public Task<TResult> Send<TResult>(byte[] consistentKey, string cmdName, byte[] payload,
            Func<AsyncBinaryResponse, TResult> funcResultFactory, object asyncState = null)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");
            if (funcResultFactory == null) throw new ArgumentNullException("funcResultFactory");

            var seqID = base.NextRequestSeqID();
            var cmdLength = cmdName.Length;
            var messageLength = (payload == null ? 0 : payload.Length) + cmdLength + 6;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write seqID.
            Buffer.BlockCopy(NetworkBitConverter.GetBytes(seqID), 0, sendBuffer, 4, 4);
            //write response flag length.
            Buffer.BlockCopy(NetworkBitConverter.GetBytes((short)cmdLength), 0, sendBuffer, 8, 2);
            //write response flag
            Buffer.BlockCopy(Encoding.ASCII.GetBytes(cmdName), 0, sendBuffer, 10, cmdLength);
            //write body buffer
            if (payload != null && payload.Length > 0)
                Buffer.BlockCopy(payload, 0, sendBuffer, 10 + cmdLength, payload.Length);

            var source = new TaskCompletionSource<TResult>(asyncState);
            base.Send(new Request<Response.AsyncBinaryResponse>(consistentKey, seqID, cmdName, sendBuffer,
                ex => source.TrySetException(ex),
                response =>
                {
                    TResult result;
                    try { result = funcResultFactory(response); }
                    catch (Exception ex) { source.TrySetException(ex); return; }

                    source.TrySetResult(result);
                }));
            return source.Task;
        }
        #endregion
    }
}