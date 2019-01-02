using System;
using System.Collections.Generic;

namespace NS.Framework.RocketSocket.Server
{
    /// <summary>
    /// Socket服务器端处理命令的处理服务实现
    /// </summary>
    /// <typeparam name="TCommandInfo">命令类型</typeparam>
    public abstract class CommandSocketService<TCommandInfo> : ISocketService<TCommandInfo>
        where TCommandInfo : class, Command.ICommandInfo
    {
        #region 私有成员
        /// <summary>
        /// 命令字典-当前命令服务可以处理的命令集合
        /// </summary>
        private readonly Dictionary<string, Command.ICommand<TCommandInfo>> _dicCommand =
            new Dictionary<string, Command.ICommand<TCommandInfo>>();
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        public CommandSocketService()
        {
            //通过反射加载命令
            var assembly = this.GetType().Assembly;
            var commands = SocketBase.Utils.ReflectionHelper.GetImplementObjects<Command.ICommand<TCommandInfo>>(assembly);
            if (commands != null && commands.Length > 0)
            {
                foreach (var cmd in commands) this.AddCommand(cmd);
            }
        }
        #endregion

        #region ISocketService 方法
        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public virtual void OnConnected(SocketBase.IConnection connection)
        {
            //开始异步接收数据.
            connection.BeginReceive();
        }

        /// <summary>
        /// 开始发送数据包方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        public void OnStartSending(SocketBase.IConnection connection, SocketBase.Packet packet)
        {
        }

        /// <summary>
        /// 发送完成后的回调方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        public virtual void OnSendCallback(SocketBase.IConnection connection, SocketBase.SendCallbackEventArgs e)
        {
        }

        /// <summary>
        /// 当接收到客户端新消息时，会调用此方法.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdInfo"></param>
        public virtual void OnReceived(SocketBase.IConnection connection, TCommandInfo cmdInfo)
        {
            if (connection == null || cmdInfo == null || string.IsNullOrEmpty(cmdInfo.CmdName)) return;

            Command.ICommand<TCommandInfo> cmd = null;
            if (this._dicCommand.TryGetValue(cmdInfo.CmdName, out cmd)) cmd.ExecuteCommand(connection, cmdInfo);
            else this.HandleUnKnowCommand(connection, cmdInfo);
        }
        /// <summary>
        /// 当断开连接时的事件处理
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public virtual void OnDisconnected(SocketBase.IConnection connection, Exception ex)
        {
        }
        /// <summary>
        /// 当通信过程中出现异常时的处理方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public virtual void OnException(SocketBase.IConnection connection, Exception ex)
        {
        }
        #endregion

        #region 保护成员
        /// <summary>
        /// 向命令处理服务中添加命令处理器
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="ArgumentNullException">cmd is null</exception>
        /// <exception cref="ArgumentNullException">cmd.Name is null</exception>
        protected virtual void AddCommand(Command.ICommand<TCommandInfo> cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");
            if (string.IsNullOrEmpty(cmd.Name)) throw new ArgumentNullException("cmd.name");
            this._dicCommand[cmd.Name] = cmd;
        }
        /// <summary>
        /// 处理未知的命令-当接收的命令类型没有对应的命令处理器时执行的方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected abstract void HandleUnKnowCommand(SocketBase.IConnection connection, TCommandInfo commandInfo);
        #endregion
    }
}