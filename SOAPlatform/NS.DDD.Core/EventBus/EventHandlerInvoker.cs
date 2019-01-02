using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.EventBus
{
    internal class EventHandlerInvoker
    {
        public static void Invoke(object handler, object evnt)
        {
            try
            {
                var method = handler.GetType().GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(handler, new object[] { evnt });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw ex;
            }
        }
    }
}
