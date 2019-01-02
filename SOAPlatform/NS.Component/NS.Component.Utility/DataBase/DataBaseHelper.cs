using NS.Component.Utility.DataBase;
using NS.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Component.Utility
{
    public class DataBaseHelper
    {
        private static readonly object flag = new object();

        public static DataBaseHelper instance;

        #region 内部方法

        public static DataBaseHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (flag)
                    {
                        if (instance == null)
                            instance = new DataBaseHelper();
                    }
                }
                return instance;
            }
        }
        private static IDataBaseDefine DataBase()
        {
            IDataBaseDefine tempDatabase = new SQLServerDataBaseDefine();
            switch (NS.Framework.Config.PlatformConfig.ServerConfig.DataBaseSetting.DataBaseType)
            {
                case "sqlserver":
                    tempDatabase = new SQLServerDataBaseDefine();
                    break;
                case "sqlite":
                    tempDatabase = new SqliteDataBaseDefine();
                    break;
                case "oracle":
                    tempDatabase = new OracleDataBaseDefine();
                    break;
                case "mysql":
                    tempDatabase = new MySQLDataBaseDefine();
                    break;
                case "access":
                    tempDatabase = new AccessDataBaseDefine();
                    break;
                default:
                    tempDatabase = new SQLServerDataBaseDefine();
                    break;
            }

            return tempDatabase;
        }

        /// <summary>
        /// 获取所有数据库示例名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetDatabaseNames()
        {
            return DataBase().GetDatabaseNames();
        }

        /// <summary>
        /// 获取所有存储过程名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetDbProcedureNames()
        {
            return DataBase().GetDbProcedureNames();
        }

        /// <summary>
        /// 获取所有函数名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetDbFunctionNames()
        {
            return DataBase().GetDbFunctionNames();
        }

        /// <summary>
        /// 获取所欲表定义集合
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetDbTableNames()
        {
            return DataBase().GetDbTableNames();
        }

        /// <summary>
        /// 获取所有视图集合
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IList<string> GetDbViewNames()
        {
            return DataBase().GetDbViewNames();
        }

        #endregion

        /// <summary>
        /// 创建新表
        /// </summary>
        /// <param name="crateSql"></param>
        /// <param name="pkColumnName"></param>
        /// <returns></returns>
        public bool CreateTable(string crateSql, string pkColumnName)
        {
            return DataBase().CreateTable(crateSql,pkColumnName);
        }

        /// <summary>
        /// 删除表-根据tableid或tablename
        /// </summary>
        /// <param name="tableid"></param>
        /// <returns></returns>
        public bool DeleteTable(string tableid)
        {
            return DataBase().DeleteTable(tableid);
        }

        /// <summary>
        /// 根据表名获取表包含的数据列
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetColumnList(string tableName)
        {
            return DataBase().GetColumnList(tableName);
        }

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataList(string tableName)
        {
            return DataBase().GetDataList(tableName);
        }

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataList(string tableName, string whereSql, JqGridParam jqgridparam)
        {
            return DataBase().GetDataList(tableName,whereSql,jqgridparam);
        }

        /// <summary>
        /// 编辑表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <param name="entityJons">实体参数</param>
        /// <returns></returns>
        public bool EditDataTableRow(string tableName, string pk, string entityJons)
        {
            return DataBase().EditDataTableRow(tableName,pk,entityJons);
        }

        /// <summary>
        /// 删除表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <returns></returns>
        public bool DeleteDataTableRow(string tableName, string pk, string entityJons)
        {
            return DataBase().DeleteDataTableRow(tableName, pk, entityJons);
        }
    }
}
