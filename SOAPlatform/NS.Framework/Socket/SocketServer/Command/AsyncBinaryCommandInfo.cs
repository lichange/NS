using System;

namespace NS.Framework.RocketSocket.Server.Command
{
    /// <summary>
    /// 异步二进制流命令对象定义 command info.
    /// </summary>
    public class AsyncBinaryCommandInfo : ICommandInfo
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <param name="seqID"></param>
        /// <param name="buffer">数据内容</param>
        /// <exception cref="ArgumentNullException">cmdName is null or empty.</exception>
        public AsyncBinaryCommandInfo(string cmdName, int seqID, byte[] buffer)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");

            this.CmdName = cmdName;
            this.SeqID = seqID;
            this.Buffer = buffer;
        }
        #endregion

        #region 公共属性
        /// <summary>
        /// 获取当前命令名称
        /// </summary>
        public string CmdName
        {
            get;
            private set;
        }
        /// <summary>
        /// 顺序编号
        /// </summary>
        public int SeqID
        {
            get;
            private set;
        }
        /// <summary>
        /// 主体内容
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 命令处理完的应答方法
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="payload">响应数据</param>
        public void Reply(SocketBase.IConnection connection, byte[] payload)
        {
            var packet = PacketBuilder.ToAsyncBinary(this.CmdName, this.SeqID, payload);
            connection.BeginSend(packet);
        }
        #endregion
    }
}