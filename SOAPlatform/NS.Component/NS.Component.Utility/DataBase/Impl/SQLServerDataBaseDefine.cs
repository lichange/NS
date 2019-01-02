using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NS.Component.Utility.DataBase
{
    public class SQLServerDataBaseDefine : DataBaseDefineBase, IDataBaseDefine
    {
        private static readonly string s_QueryDatabaseListScript =
    "SELECT dtb.name AS [Database_Name] FROM master.sys.databases AS dtb " +
    "WHERE (CAST(case when dtb.name in ('master','model','msdb','tempdb') then 1 else dtb.is_distributor end AS bit)=0 " +
    "    and CAST(isnull(dtb.source_database_id, 0) AS bit)=0) " +
    "ORDER BY [Database_Name] ASC";

        private static readonly string s_GetObjectNamesFormat =
            "select name from ( SELECT obj.name AS [Name],  " +
            "CAST( case when obj.is_ms_shipped = 1 then 1 " +
            "    when ( select major_id from sys.extended_properties  " +
            "        where major_id = obj.object_id and  minor_id = 0 and class = 1 and name = N'microsoft_database_tools_support')  " +
            "        is not null then 1  else 0 " +
            "end  AS bit) AS [IsSystemObject] " +
            "FROM sys.all_objects AS obj where obj.type in ({0}) )as tables where [IsSystemObject] = 0 ORDER BY [Name] ASC ";

        private static readonly string s_QueryTableDefinitionScription =
            "SELECT clmns.name AS [Name], " +
            "baset.name AS [DataType], " +
            "CAST(CASE WHEN baset.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1 THEN clmns.max_length/2 ELSE clmns.max_length END AS int) AS [Length], " +
            "CAST(clmns.precision AS int) AS [NumericPrecision], " +
            "clmns.is_identity AS [Identity], " +
            "clmns.is_nullable AS [Nullable] " +
            "FROM sys.tables AS tbl " +
            "INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id " +
            "LEFT OUTER JOIN sys.types AS baset ON baset.user_type_id = clmns.system_type_id and baset.user_type_id = baset.system_type_id " +
            "LEFT OUTER JOIN sys.schemas AS sclmns ON sclmns.schema_id = baset.schema_id " +
            "LEFT OUTER JOIN sys.identity_columns AS ic ON ic.object_id = clmns.object_id and ic.column_id = clmns.column_id " +
            "WHERE (tbl.name=N'{0}' ) ORDER BY clmns.column_id ASC";

        private static readonly string s_DeleteObjectFormat =
            "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in ({1})) " +
            "   DROP {2} [{0}] ";

        private static readonly string s_CreateObjectFormat = "EXEC dbo.sp_executesql @statement = N'{0}' ";

        public SQLServerDataBaseDefine()
        {
        }

        public override string GetQueryTableDefinitionScription()
        {
            return s_QueryTableDefinitionScription;
        }

        public override string GetDeleteObjectFormat()
        {
            return s_DeleteObjectFormat;
        }
        public override string GetCreateObjectFormat()
        {
            return s_CreateObjectFormat;
        }

        public override string GetQueryDatabaseListScript()
        {
            return s_QueryDatabaseListScript;
        }

        public override string GetObjectNamesFormat()
        {
            return s_GetObjectNamesFormat;
        }

        private static readonly string procedureItemScript="SELECT definition FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type in ({1}) and name = '{0}'";
        public override string GetProcedureItemScript()
        {
            return procedureItemScript;
        }

        private static readonly string functionItemScript = "SELECT definition FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type in ({1}) and name = '{0}'";

        public override string GetFunctionItemScript()
        {
            return functionItemScript;
        }

        private static readonly string viewItemScript = "SELECT definition FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type in ({1}) and name = '{0}'";

        public override string GetViewItemScript()
        {
            return viewItemScript;
        }
    }
}
