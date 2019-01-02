using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.DDD.Core
{
    public class InstanceFactory
    {
        public static T CreateInstance<T>() where T : class, new()
        {
            return new T();
        }
    }
}
