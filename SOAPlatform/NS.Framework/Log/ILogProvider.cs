using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Framework.Log
{
    public interface ILogProvider
    {
        void Debug(string message);

        void Debug(string message, Exception ex);

        void Info(string message);

        void Info(string message, Exception ex);

        void Warn(object message);

        void Warn(object message, Exception exception);

        void Error(object message);

        void Error(object message, Exception exception);

        void Fatal(object message);

        void Fatal(object message, Exception exception);

        bool IsDebugEnabled { get; }
        
        bool IsInfoEnabled { get; }
        
        bool IsWarnEnabled { get; }
        
        bool IsErrorEnabled { get; }
        
        bool IsFatalEnabled { get; }
    }
}
