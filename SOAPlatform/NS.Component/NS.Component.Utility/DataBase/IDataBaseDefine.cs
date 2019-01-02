using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Component.Utility.DataBase
{
    /// <summary>
    /// 数据库定义
    /// </summary>
    public interface IDataBaseDefine
    {
        /// <summary>
        /// 获取所有数据库示例名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IList<string> GetDatabaseNames();

        /// <summary>
        /// 获取所有存储过程名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IList<string> GetDbProcedureNames();

        /// <summary>
        /// 获取所有函数名称
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IList<string> GetDbFunctionNames();

        /// <summary>
        /// 获取所欲表定义集合
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IList<string> GetDbTableNames();

        /// <summary>
        /// 获取所有视图集合
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IList<string> GetDbViewNames();

        /// <summary>
        /// 获取存储过程的定义信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ItemCode GetProcedureItem(string name);
        /// <summary>
        /// 获取函数的定义信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ItemCode GetFunctionItem(string name);
        /// <summary>
        /// 获取视图的定义信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ItemCode GetViewItem(string name);
        /// <summary>
        /// 根据表名获取表定义
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ItemCode GetTableItem(string name);

        #region 数据库操作

        /// <summary>
        /// 创建新表
        /// </summary>
        /// <param name="crateSql"></param>
        /// <param name="pkColumnName"></param>
        /// <returns></returns>
        bool CreateTable(string crateSql, string pkColumnName);

        /// <summary>
        /// 删除表-根据tableid或tablename
        /// </summary>
        /// <param name="tableid"></param>
        /// <returns></returns>
        bool DeleteTable(string tableid);

        /// <summary>
        /// 根据表名获取表包含的数据列
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        System.Data.DataTable GetColumnList(string tableName);

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        System.Data.DataTable GetDataList(string tableName);

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        System.Data.DataTable GetDataList(string tableName, string whereSql, JqGridParam jqgridparam);

        /// <summary>
        /// 编辑表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <param name="entityJons">实体参数</param>
        /// <returns></returns>
        bool EditDataTableRow(string tableName, string pk, string entityJons);

        /// <summary>
        /// 删除表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <returns></returns>
        bool DeleteDataTableRow(string tableName, string pk, string entityJons);
        #endregion
    }
}
