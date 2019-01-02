using System;

namespace NS.Framework.RocketSocket.SocketBase.Log
{
    /// <summary>
    /// 控制台监听器-输出信息到控制台
    /// </summary>
    public sealed class ConsoleListener : ITraceListener
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message">调试信息</param>
        public void Debug(string message)
        {
            Console.WriteLine(string.Concat(message, Environment.NewLine));
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <param name="ex">异常信息对象</param>
        public void Error(string message, Exception ex)
        {
            Console.WriteLine(string.Concat(message, Environment.NewLine, ex.ToString(), Environment.NewLine));
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">信息内容</param>
        public void Info(string message)
        {
            Console.WriteLine(string.Concat(message, Environment.NewLine));
        }
    }
}