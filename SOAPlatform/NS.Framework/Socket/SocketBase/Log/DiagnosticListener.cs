using System;

namespace NS.Framework.RocketSocket.SocketBase.Log
{
    /// <summary>
    /// 诊断信息监听器-通过.net提供的跟踪器进行跟踪
    /// </summary>
    public sealed class DiagnosticListener : ITraceListener
    {
        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Error(string message, Exception ex)
        {
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }
    }
}