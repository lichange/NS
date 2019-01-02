using Microsoft.EntityFrameworkCore;
using NS.DDD.Data.BulkExtensions;

using System.Collections.Generic;
using System.Linq;

namespace NS.DDD.Data
{
    internal static class DbContextBulkOperationExtensions
    {
        public const int DefaultBatchSize = 10000;

        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, int batchSize = DefaultBatchSize)
        {
            var tempDbType = NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.DataBaseType;
            if (string.IsNullOrEmpty(tempDbType))
                throw new KeyNotFoundException("find data base settings error!");

            switch (tempDbType)
            {
                case "mysql":
                    var mySqlProvider = new MySQLBulkOperationProvider(context);
                    mySqlProvider.Insert(entities, batchSize);
                    break;
                case "sqlserver":
                    var provider = new BulkOperationProvider(context);
                    provider.Insert(entities, batchSize);
                    break;
                case "oracle":
                    break;
            }
        }
    }
}
