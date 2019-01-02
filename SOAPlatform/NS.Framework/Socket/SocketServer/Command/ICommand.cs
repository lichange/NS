
namespace NS.Framework.RocketSocket.Server.Command
{
    /// <summary>
    /// socket服务器支持的命令契约
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 获取命令名称
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// 命令接口
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public interface ICommand<TCommandInfo> : ICommand where TCommandInfo : ICommandInfo
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="connection">连接对象</param>
        /// <param name="commandInfo">命令对象</param>
        void ExecuteCommand(SocketBase.IConnection connection, TCommandInfo commandInfo);
    }
}