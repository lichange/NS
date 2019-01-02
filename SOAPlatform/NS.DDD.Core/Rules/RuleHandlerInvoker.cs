using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    internal class RuleHandlerInvoker
    {
        public static object Invoke(object handler, object[] evnt)
        {
            try
            {
                var method = handler.GetType().GetMethod("Excute", BindingFlags.Instance | BindingFlags.Public);
                return method.Invoke(handler, new object[] { evnt });
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                    throw exception.InnerException;
                else
                    throw exception;
            }
        }
    }
}
