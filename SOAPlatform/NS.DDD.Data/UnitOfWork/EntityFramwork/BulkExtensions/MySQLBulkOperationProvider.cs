using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace NS.DDD.Data.BulkExtensions
{
    internal class MySQLBulkOperationProvider
    {
        private DbContext _context;
        private string _connectionString;
        private readonly string _tmpBasePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _tmpCSVFilePattern = "Temp\\{0}.csv";   //0表示文件名称

        public MySQLBulkOperationProvider(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            _connectionString = context.Database.GetDbConnection().ConnectionString;
            //ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings[context.GetType().Name];
            //_connectionString = cs.ConnectionString;
        }

        public void Insert<T>(IEnumerable<T> entities, int batchSize)
        {
            using (var dbConnection = new MySqlConnection(_connectionString))
            {
                dbConnection.Open();

                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        Insert(entities, transaction, batchSize);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        if (transaction.Connection != null)
                        {
                            transaction.Rollback();
                        }
                        throw;
                    }
                }
            }
        }

        public bool BulkInsert(DataTable dataTable)
        {
            bool result = false;
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                using (MySqlConnection mySqlCon = new MySqlConnection(_connectionString))
                {
                    mySqlCon.Open();
                    MySqlTransaction sqlTran = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted);
                    MySqlBulkLoader sqlBulkCopy = new MySqlBulkLoader(mySqlCon);
                    sqlBulkCopy.Timeout = 60;

                    result = BulkInsert(sqlBulkCopy, dataTable, sqlTran);
                }
            }
            return result;
        }

        public bool BulkInsert(DataSet dataSet)
        {
            bool result = false;
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                using (MySqlConnection mySqlCon = new MySqlConnection(_connectionString))
                {
                    mySqlCon.Open();
                    MySqlTransaction sqlTran = mySqlCon.BeginTransaction(IsolationLevel.ReadCommitted);
                    MySqlBulkLoader sqlBulkCopy = new MySqlBulkLoader(mySqlCon);
                    sqlBulkCopy.Timeout = 60;

                    if (dataSet.Tables.Count == 1)
                        result = BulkInsert(sqlBulkCopy, dataSet.Tables[0], sqlTran);
                    else
                    {
                        foreach (DataTable dt in dataSet.Tables)
                        {
                            result = BulkInsert(sqlBulkCopy, dt, sqlTran);
                            if (!result)
                                break;
                        }
                    }
                }
            }
            return result;
        }

        public bool BulkInsert<T, T1>(T sqlBulkCopy, DataTable dataTable, T1 sqlTrasaction)
        {
            bool result = false;
            string tmpCsvPath = this._tmpBasePath + string.Format(this._tmpCSVFilePattern, DateTime.Now.Ticks.ToString());
            string tmpFolder = tmpCsvPath.Remove(tmpCsvPath.LastIndexOf("\\"));

            if (!Directory.Exists(tmpFolder))
                Directory.CreateDirectory(tmpFolder);

            FileHelper.WriteDataTableToCSVFile(dataTable, tmpCsvPath);   //Write to csv File

            MySqlBulkLoader sqlBC = (MySqlBulkLoader)Convert.ChangeType(sqlBulkCopy, typeof(MySqlBulkLoader));
            MySqlTransaction sqlTran = (MySqlTransaction)Convert.ChangeType(sqlTrasaction, typeof(MySqlTransaction));
            try
            {
                sqlBC.TableName = dataTable.TableName.Replace("dbo.","");
                sqlBC.FieldTerminator = "|";
                sqlBC.LineTerminator = "\r\n";
                sqlBC.FileName = tmpCsvPath;
                sqlBC.NumberOfLinesToSkip = 0;

                List<string> strCollection = new List<string>();
                //Mapping Destination Field of Database Table
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    strCollection.Add(dataTable.Columns[i].ColumnName);
                }
                sqlBC.Columns.AddRange(strCollection);

                //Write DataTable
                sqlBC.Load();

                sqlTran.Commit();
                result = true;
            }
            catch (MySqlException mse)
            {
                result = false;
                sqlTran.Rollback();
                throw mse;
            }
            finally
            {
                //T、T1给默认值为Null, 由系统调用GC
                sqlBC = null;
                sqlBulkCopy = default(T);
            }
            File.Delete(tmpCsvPath);
            return result;
        }

        private void Insert<T>(IEnumerable<T> entities, MySqlTransaction transaction, int batchSize)
        {
            var tempEntityType = typeof(T).FullName.Replace("Domain", "EntityFramework");
            var allTableMappings = DbMapper.GetDbMapping(_context);
            TableMapping tableMapping = allTableMappings[tempEntityType];
            using (DataTable dataTable = CreateDataTable(tableMapping, entities))
            {
                this.BulkInsert(dataTable);
            }
        }

        private static DataTable CreateDataTable<T>(TableMapping tableMapping, IEnumerable<T> entities)
        {
            var dataTable = BuildDataTable<T>(tableMapping);

            foreach (var entity in entities)
            {
                DataRow row = dataTable.NewRow();

                foreach (var columnMapping in tableMapping.Columns)
                {
                    var @value = entity.GetPropertyValue(columnMapping.PropertyName);

                    if (columnMapping.IsIdentity)
                        continue;

                    if (@value == null)
                    {
                        row[columnMapping.ColumnName] = DBNull.Value;
                    }
                    else
                    {
                        row[columnMapping.ColumnName] = @value;
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private static DataTable BuildDataTable<T>(TableMapping tableMapping)
        {
            var entityType = typeof(T);
            string tableName = string.Join(@".", tableMapping.SchemaName, tableMapping.TableName);

            var dataTable = new DataTable(tableName);
            var primaryKeys = new List<DataColumn>();

            foreach (var columnMapping in tableMapping.Columns)
            {
                var propertyInfo = entityType.GetProperty(columnMapping.PropertyName, '.');
                columnMapping.Type = propertyInfo.PropertyType;

                var dataColumn = new DataColumn(columnMapping.ColumnName);

                Type dataType;
                if (propertyInfo.PropertyType.IsNullable(out dataType))
                {
                    dataColumn.DataType = dataType;
                    dataColumn.AllowDBNull = true;
                }
                else
                {
                    dataColumn.DataType = propertyInfo.PropertyType;
                    dataColumn.AllowDBNull = columnMapping.Nullable;
                }

                if (columnMapping.IsIdentity)
                {
                    dataColumn.Unique = true;
                    if (propertyInfo.PropertyType == typeof(int)
                      || propertyInfo.PropertyType == typeof(long))
                    {
                        dataColumn.AutoIncrement = true;
                    }
                    else
                        continue;
                }
                else
                {
                    dataColumn.DefaultValue = columnMapping.DefaultValue;
                }

                if (propertyInfo.PropertyType == typeof(string))
                {
                    dataColumn.MaxLength = columnMapping.MaxLength;
                }

                if (columnMapping.IsPk)
                {
                    primaryKeys.Add(dataColumn);
                }

                dataTable.Columns.Add(dataColumn);
            }

            dataTable.PrimaryKey = primaryKeys.ToArray();

            return dataTable;
        }
    }

    public class FileHelper
    {
        public static string ReadFileToString(string fileFullPath, Encoding codeType)
        {
            string result = "";
            if (string.IsNullOrEmpty(fileFullPath))
                throw new ArgumentNullException("fileFullPath", "File path can not be null or empty! ");
            using (FileStream fileStream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Read))
            {
                if (!File.Exists(fileFullPath))
                    throw new FileNotFoundException("File not found! ");
            }

            using (StreamReader sReader = new StreamReader(fileFullPath, codeType))
            {
                try
                {
                    result = sReader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    throw new IOException(ex.Message);
                }
            }
            return result;
        }

        public static string ReadFileToString(string fileFullPath)
        {
            return ReadFileToString(fileFullPath, Encoding.Default);
        }

        public static void WriteDataTableToCSVFile(DataTable dataTable, string fileFullPath, Encoding codeType)
        {
            using (Stream stream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter swriter = new StreamWriter(stream, codeType))
            {
                try
                {
                    int icolcount = dataTable.Columns.Count;
                    foreach (DataRow drow in dataTable.Rows)
                    {
                        for (int i = 0; i < icolcount; i++)
                        {
                            if (!Convert.IsDBNull(drow[i]))
                            {
                                swriter.Write(drow[i].ToString());
                            }
                            if (i < icolcount - 1)
                            {
                                swriter.Write("|");
                            }
                        }
                        swriter.Write(swriter.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    throw new IOException(ex.Message);
                }
            }
        }

        public static void WriteDataTableToCSVFile(DataTable dataTable, string fileFullPath)
        {
            WriteDataTableToCSVFile(dataTable, fileFullPath, Encoding.Default);
        }

        public static string[] GetFileFullPathList(string directoryPath, string fileType, bool IsRecursive)
        {
            return IsRecursive ? Directory.GetFiles(directoryPath, fileType, SearchOption.AllDirectories) : Directory.GetFiles(directoryPath, fileType, SearchOption.TopDirectoryOnly);
        }

        public static string[] GetSubDirectorys(string directoryPath, string containsName, bool IsRecursive)
        {
            return IsRecursive ? Directory.GetDirectories(directoryPath, containsName, SearchOption.AllDirectories) : Directory.GetDirectories(directoryPath, containsName, SearchOption.TopDirectoryOnly);
        }

        public static void WriteStringToFile(string fileFullPath, bool isAppend, string fileContent)
        {
            WriteStringToFile(fileFullPath, isAppend, fileContent, Encoding.Default);
        }

        public static void WriteStringToFile(string fileFullPath, bool isAppend, string fileContent, Encoding codeType)
        {
            //using (FileStream fileStream = new FileStream(fileFullPath, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter sWriter = new StreamWriter(fileFullPath, isAppend, codeType))
            {
                try
                {
                    if (!File.Exists(fileFullPath))
                        File.Create(fileFullPath);
                    sWriter.Write(fileContent);
                }
                catch (Exception ex)
                {
                    throw new IOException(ex.Message);
                }
            }
        }
    }
}
