
namespace NS.Framework.RocketSocket.Client.Response
{
    /// <summary>
    /// 异步二进制流响应对象
    /// </summary>
    public class AsyncBinaryResponse : IResponse
    {
        private int _seqID;
        /// <summary>
        /// 标志位信息
        /// </summary>
        public readonly string Flag = null;
        /// <summary>
        /// 数据
        /// </summary>
        public readonly byte[] Buffer = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        public AsyncBinaryResponse(string flag, int seqID, byte[] buffer)
        {
            this._seqID = seqID;
            this.Flag = flag;
            this.Buffer = buffer;
        }

        /// <summary>
        /// 序列
        /// </summary>
        public int SeqID
        {
            get { return this._seqID; }
        }
    }
}