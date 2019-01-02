
namespace NS.Framework.RocketSocket.Client.Response
{
    /// <summary>
    /// thrift 响应.
    /// </summary>
    public class ThriftResponse : IResponse
    {
        private int _seqID;
        /// <summary>
        /// 数据信息
        /// </summary>
        public readonly byte[] Buffer = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        public ThriftResponse(int seqID, byte[] buffer)
        {
            this._seqID = seqID;
            this.Buffer = buffer;
        }

        /// <summary>
        /// 响应序列
        /// </summary>
        public int SeqID
        {
            get { return this._seqID; }
        }
    }
}