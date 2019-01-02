using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core.EventBus
{
    /// <summary>
    /// 判断当前的对象的泛型接口中是否有目标类型作为参数的接口类型
    /// </summary>
    internal class EventHandlerFinderUtility
    {
        public static Type TryFindEventTypeOfImplementedHandlerInterface(Type type, Type handlerInterfaceOpenGenericType)
        {
            foreach (var inter in type.GetInterfaces())
            {
                if (inter.IsGenericType)
                {
                    var def = inter.GetGenericTypeDefinition();
                    var eventType = inter.GetGenericArguments().FirstOrDefault();

                    if (def == handlerInterfaceOpenGenericType)
                    {
                        return eventType;
                    }
                }
            }

            return null;
        }
    }
}
