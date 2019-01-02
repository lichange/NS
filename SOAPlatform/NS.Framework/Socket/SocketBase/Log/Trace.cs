using System;
using System.Collections.Generic;

namespace NS.Framework.RocketSocket.SocketBase.Log
{
    /// <summary>
    /// 跟踪器
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// 跟踪器集合
        /// </summary>
        static private readonly List<ITraceListener> _list = new List<ITraceListener>();

        /// <summary>
        /// 启用控制台监控器
        /// </summary>
        static public void EnableConsole()
        {
            _list.Add(new ConsoleListener());
        }
        /// <summary>
        /// 启用.net跟踪器
        /// </summary>
        static public void EnableDiagnostic()
        {
            _list.Add(new DiagnosticListener());
        }

        /// <summary>
        /// 添加监听器
        /// </summary>
        /// <param name="listener"></param>
        /// <exception cref="ArgumentNullException">listener is null</exception>
        static public void AddListener(ITraceListener listener)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            _list.Add(listener);
        }

        /// <summary>
        /// 调试信息
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException">message is null</exception>
        static public void Debug(string message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _list.ForEach(c => c.Debug(message));
        }

        /// <summary>
        /// 内容
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException">message is null</exception>
        static public void Info(string message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _list.ForEach(c => c.Info(message));
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <exception cref="ArgumentNullException">message is null</exception>
        static public void Error(string message, Exception ex)
        {
            if (message == null) throw new ArgumentNullException("message");
            _list.ForEach(c => c.Error(message, ex));
        }
    }
}