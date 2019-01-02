
namespace NS.Framework.RocketSocket.Server.Command
{
    /// <summary>
    /// thrift 命令对象定义：该命令其实是消息流-未经过处理和解析
    /// </summary>
    public sealed class ThriftCommandInfo : ICommandInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="buffer"></param>
        public ThriftCommandInfo(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        /// <summary>
        /// 获取当前命令名称：始终返回空
        /// </summary>
        public string CmdName
        {
            get { return null; }
        }
        /// <summary>
        /// 内容数据
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }        
    }
}