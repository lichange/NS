using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NS.Component.Utility.DataBase
{

    /// <summary>
    /// 数据库操作基础抽象类
    /// </summary>
    public abstract class DataBaseDefineBase : IDataBaseDefine
    {
        public IList<string> GetDatabaseNames()
        {
            var tempTables = DbHelper.GetDataSet(System.Data.CommandType.Text, GetQueryDatabaseListScript());

            return this.GetList(tempTables);
        }

        private IList<string> GetList(DataSet tempTables)
        {
            // 定义集合  
            IList<string> ts = new List<string>();

            //定义一个临时变量  
            string tempName = string.Empty;
            var dt = tempTables.Tables[0];

            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                //对象添加到泛型集合中  
                ts.Add(dr[0].ToString());
            }

            return ts;
        }

        public abstract string GetQueryDatabaseListScript();

        public abstract string GetObjectNamesFormat();

        public abstract string GetQueryTableDefinitionScription();

        public abstract string GetDeleteObjectFormat();

        public abstract string GetCreateObjectFormat();

        public abstract string GetProcedureItemScript();

        public abstract string GetFunctionItemScript();

        public abstract string GetViewItemScript();

        private static readonly string s_ProcedureType = " N'P', N'PC' ";
        private static readonly string s_FunctionType = " N'FN', N'IF', N'TF', N'FS', N'FT' ";
        private static readonly string s_TableType = " N'U' ";
        private static readonly string s_ViewType = " N'V' ";

        public IList<string> GetDbProcedureNames()
        {
            //string sql = "select name from sys.objects where type='P' order by name";
            string sql = string.Format(GetObjectNamesFormat(), s_ProcedureType);
            var tempTables = DbHelper.GetDataSet(System.Data.CommandType.Text, sql);
            return this.GetList(tempTables);
        }

        public IList<string> GetDbFunctionNames()
        {
            //string sql = "select name from sys.objects where type='FN' order by name";
            string sql = string.Format(GetObjectNamesFormat(), s_FunctionType);
            var tempTables = DbHelper.GetDataSet(System.Data.CommandType.Text, sql);
            return this.GetList(tempTables);
        }

        public IList<string> GetDbTableNames()
        {
            //string sql = "select name from sys.objects where type='U' where name != 'sysdiagrams' order by name";
            string sql = string.Format(GetObjectNamesFormat(), s_TableType);
            var tempTables = DbHelper.GetDataSet(System.Data.CommandType.Text, sql);
            return this.GetList(tempTables);
        }

        public IList<string> GetDbViewNames()
        {
            //string sql = "select name from sys.objects where type='V' order by name";
            string sql = string.Format(GetObjectNamesFormat(), s_ViewType);
            var tempTables = DbHelper.GetDataSet(System.Data.CommandType.Text, sql);
            return this.GetList(tempTables);
        }

        #region 获取DDL定义明细信息


        public ItemCode GetProcedureItem(string name)
        {
            string query = string.Format(GetProcedureItemScript(), name, s_ProcedureType);
            var script = DbHelper.ExecuteScalar(CommandType.Text, query);
            return new ItemCode(name, ItemType.Procedure, script.ToString());
        }


        public ItemCode GetFunctionItem(string name)
        {
            string query = string.Format(GetFunctionItemScript(), name, s_FunctionType);
            var script = DbHelper.ExecuteScalar(CommandType.Text, query);
            return new ItemCode(name, ItemType.Function, script.ToString());
        }

        public ItemCode GetViewItem(string name)
        {
            string query = string.Format(GetViewItemScript(), name, s_ViewType);
            //string script = TryExecuteQuery(connection, query);
            var script = DbHelper.ExecuteScalar(CommandType.Text, query);
            return new ItemCode(name, ItemType.View, script.ToString());
        }

        public ItemCode GetTableItem(string name)
        {
            string script = null;
            try
            {
                //script = SmoHelper.ScriptTable(DbHelper.ConnectionString, null, name);
                //if (string.IsNullOrEmpty(script))
                //    script = s_CannotGetScript;
            }
            catch (Exception ex)
            {
                script = ex.Message;
            }
            return new ItemCode(name, ItemType.Table, script);
        }

        #endregion

        #region DDL操作

        /// <summary>
        /// 创建新表
        /// </summary>
        /// <param name="crateSql"></param>
        /// <param name="pkColumnName"></param>
        /// <returns></returns>
        public bool CreateTable(string crateSql, string pkColumnName)
        {
            var script = DbHelper.ExecuteNonQuery(CommandType.Text, crateSql);

            return script > 0;
        }

        /// <summary>
        /// 删除表-根据tableid或tablename
        /// </summary>
        /// <param name="tableid"></param>
        /// <returns></returns>
        public bool DeleteTable(string tableid)
        {
            var stringSql = string.Format(GetDeleteObjectFormat(), tableid);
            var script = DbHelper.ExecuteNonQuery(CommandType.Text, stringSql);
            return script > 0;
        }

        /// <summary>
        /// 根据表名获取表包含的数据列
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetColumnList(string tableName)
        {
            return new DataTable();
        }

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataList(string tableName)
        {
            return new DataTable();
        }

        /// <summary>
        /// 根据表名获取该表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public System.Data.DataTable GetDataList(string tableName, string whereSql, JqGridParam jqgridparam)
        {
            return new DataTable();
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
            return true;
        }

        /// <summary>
        /// 删除表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <returns></returns>
        public bool DeleteDataTableRow(string tableName, string pk, string entityJons)
        {
            return true;
        }

        #endregion
    }
}
