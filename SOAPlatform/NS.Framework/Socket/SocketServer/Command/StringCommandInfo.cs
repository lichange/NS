using System;

namespace NS.Framework.RocketSocket.Server.Command
{
    /// <summary>
    /// 字符串命令定义
    /// </summary>
    public class StringCommandInfo : ICommandInfo
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName">命令名称</param>
        /// <param name="parameters">参数</param>
        /// <exception cref="ArgumentNullException">cmdName is null or empty</exception>
        public StringCommandInfo(string cmdName, params string[] parameters)
        {
            if (string.IsNullOrEmpty(cmdName)) throw new ArgumentNullException("cmdName");

            this.CmdName = cmdName;
            this.Parameters = parameters;
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
        /// 获取当前命令附带的参数
        /// </summary>
        public string[] Parameters
        {
            get;
            private set;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 应答-请求处理完毕后发送结果给请求端
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="value"></param>
        public void Reply(SocketBase.IConnection connection, string value)
        {
            connection.BeginSend(PacketBuilder.ToCommandLine(value));
        }
        #endregion
    }
}