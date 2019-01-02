using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace NS.Framework.Exceptions
{
    [System.Serializable]
    public class FrameworkException : Exception
    {
        public FrameworkException()
        {
        }

        public FrameworkException(string Message)
            : base(Message)
        {
        }
    }
}