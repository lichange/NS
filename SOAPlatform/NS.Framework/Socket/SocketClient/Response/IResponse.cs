
namespace NS.Framework.RocketSocket.Client.Response
{
    /// <summary>
    /// 服务器响应结果接口定义
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 序列-主要用于多分包的情况下使用
        /// </summary>
        int SeqID { get; }
    }
}