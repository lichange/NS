using System;

namespace NS.Framework.RocketSocket.SocketBase
{
    /// <summary>
    /// 数据包定义
    /// </summary>
    public class Packet
    {
        #region Members
        /// <summary>
        /// 获取或设置已发送的数据的大小
        /// </summary>
        internal int SentSize = 0;
        /// <summary>
        ///获取发送的起始时间
        /// </summary>
        public readonly DateTime BeginTime = DateTime.UtcNow;
        /// <summary>
        /// 数据包的内容
        /// </summary>
        public readonly byte[] Payload;
        #endregion

        #region Constructors
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="payload">数据包的内容</param>
        /// <exception cref="ArgumentNullException">数据包内容为null</exception>
        public Packet(byte[] payload)
        {
            if (payload == null) throw new ArgumentNullException("payload");
            this.Payload = payload;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// 数据包对象
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// 获取一个值，该值指示当前packet是否已发送完毕.
        /// </summary>
        /// <returns>true表示已发送完毕</returns>
        public bool IsSent()
        {
            return this.SentSize >= this.Payload.Length;
        }
        #endregion
    }
}