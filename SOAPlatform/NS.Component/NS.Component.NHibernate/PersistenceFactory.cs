using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS.Component.Data
{
    public class PersistenceFactory
    {
        public static IPersistenceDAL CreatePersistence(string connectionString)
        {
            return new Impl.PersistenceDAL(connectionString);
        }
    }
}
