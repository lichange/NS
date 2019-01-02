
namespace NS.Framework.RocketSocket.Server.Command
{
    /// <summary>
    /// 命令信息接口
    /// </summary>
    public interface ICommandInfo
    {
        /// <summary>
        /// 获取命令的名称
        /// </summary>
        string CmdName { get; }
    }
}