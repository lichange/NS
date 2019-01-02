using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;

namespace NS.DDD.Data
{
    public abstract class EntityFrameworkDbContext : DbContext
    {
        private string key;

        /// <summary>
        /// 注入机制
        /// </summary>
        /// <param name="options"></param>
        protected EntityFrameworkDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// 通过new创建实例机制
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected EntityFrameworkDbContext(string key) : base()
        {
            // do something
            this.key = key;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var tempString = NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.GetConnectionString(key);
            switch (key)
            {
                case "mySql":
                    optionsBuilder.UseMySQL(tempString);
                    break;
                case "Oracle":
                    //optionsBuilder.UseOracle(tempString);
                    break;
                case "Sqlite":
                    //optionsBuilder.UseSqlite(tempString);
                    break;
                default:
                    break;
            }


            base.OnConfiguring(optionsBuilder);
        }
    }
}
