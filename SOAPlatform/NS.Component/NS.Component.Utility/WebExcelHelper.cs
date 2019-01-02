using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Aspose.Cells;
using NS.Framework.Global;

namespace NS.Component.Utility
{
    /// <summary>
    /// ASP.Net 操作Excel的辅助类
    /// </summary>
    public class WebExcelHelper
    {
        #region 读取excel信息

        /// <summary>
        /// 该方法实现从Excel中导出数据到DataSet中，其中filepath为Excel文件的绝对路径，sheetname为表示那个Excel表；
        /// </summary>
        /// <param name="filepath">Excel文件的绝对路径</param>
        /// <param name="sheetname">sheetname为表示那个Excel表-工作区名称</param>
        /// <returns>返回dataset</returns>
        //public static DataSet ExcelDataSource(string filepath)
        //{
        //    if (string.IsNullOrEmpty(filepath))
        //        return new DataSet();

        //    var tempSheets = ExcelSheetName(filepath);

        //    if (tempSheets.Count == 0)
        //        return new DataSet();

        //    string strConn;
        //    strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0;", filepath);
        //    OleDbConnection conn = new OleDbConnection(strConn);

        //    DataSet ds = new DataSet();
        //    foreach (var sheetname in tempSheets)
        //    {
        //        OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "]", strConn);
        //        oada.Fill(ds, sheetname);
        //    }

        //    return ds;
        //}

        /// <summary>
        /// 获取excel中包含的所有工作区表的集合
        /// </summary>
        /// <param name="filepath">Excel文件完整路径</param>
        /// <returns></returns>
        //public static IList<string> ExcelSheetName(string filepath)
        //{
        //    IList<string> sheets = new List<string>();

        //    if (string.IsNullOrEmpty(filepath))
        //        return sheets;

        //    string strConn;
        //    strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0;", filepath);
        //    OleDbConnection conn = new OleDbConnection(strConn);
        //    conn.Open();
        //    DataTable sheetNames = conn.GetOleDbSchemaTable
        //    (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
        //    conn.Close();
        //    foreach (DataRow dr in sheetNames.Rows)
        //    {
        //        sheets.Add(dr[2].ToString());
        //    }
        //    return sheets;
        //}

        /// <summary>
        /// 获取excel中包含的所有工作区表的数据集合
        /// </summary>
        /// <param name="filepath">Excel文件完整路径</param>
        /// <returns></returns>
        public static IList<DataTable> GetDataTables(string filepath)
        {
            var dataTables = new List<DataTable>();

            if (string.IsNullOrEmpty(filepath))
                return dataTables;

            //采用微软的Excel套件
            //var tempDs = ExcelDataSource(filepath);
            //foreach (DataTable item in tempDs.Tables)
            //{
            //    dataTables.Add(item);
            //}

            //采用其他的开源套件
            AsposeExcel excel = new AsposeExcel(filepath);
            var tempDs = excel.ExcelToDatatalbe();

            //foreach (DataTable item in tempDs)
            //{
            //    dataTables.Add(item);
            //}

            return tempDs;
        }

        #endregion

        #region 上传Excel文件

        //public static IList<DataTable> ImportExcel(FileUpload fileUpload)
        //{
        //    string filename = DateTime.Now.ToString("yyyymmddhhMMss") + fileUpload.FileName;

        //    var extensionList = new List<string>()
        //    {
        //        ".xls",".xlsx"
        //    };

        //    if (!extensionList.Contains(System.IO.Path.GetExtension(fileUpload.FileName)))
        //        throw new Exception("文件不是excel文件");

        //    string savePath = Path.Combine(NSWebPath.HostEnv.WebRootPath, "upfiles", filename);//Server.MapPath 获得虚拟服务器相对路径
        //    fileUpload.SaveAs(savePath);

        //    var tempDataTables = GetDataTables(savePath);

        //    if (tempDataTables.Count == 0)
        //        throw new Exception("excel中不包含任何数据,无法上传");

        //    return tempDataTables;
        //}

        #endregion

        #region 导出Excel文件

        //public static void ExportExcel<T>(IList<T> tempList) where T : class, new()
        //{
        //    SaveFileDialog tempSaveFileDialog = new SaveFileDialog();
        //    tempSaveFileDialog.Title = "保存的excel文件";
        //    tempSaveFileDialog.InitialDirectory = "c:\\";
        //    tempSaveFileDialog.Filter = "Excel97-2003 (*.xls)|*.xls|All Files (*.*)|*.*";
        //    tempSaveFileDialog.ShowDialog();
        //    if (tempSaveFileDialog.FileName == "" || tempSaveFileDialog.FileName == null)
        //    {
        //        MessageBox.Show("文件名不能为空!");
        //        return;
        //    }
        //    string path = tempSaveFileDialog.FileName;
        //    WriteExcel<T>(tempList, path);
        //}

        //private static void WriteExcel<T>(IList<T> tempList, string path) where T : class, new()
        //{
        //    try
        //    {
        //        long totalCount = tempList.Count;
        //        Thread.Sleep(1000);
        //        long rowRead = 0;
        //        //float percent = 0;

        //        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));
        //        StringBuilder sb = new StringBuilder();
        //        var tempPropertys = typeof(T).GetProperties();
        //        for (int k = 0; k < tempPropertys.Length; k++)
        //        {
        //            sb.Append(tempPropertys[k].Name.ToString() + "\t");
        //        }
        //        sb.Append(Environment.NewLine);

        //        for (int i = 0; i < tempList.Count; i++)
        //        {
        //            rowRead++;
        //            //percent = ((float)(100 * rowRead)) / totalCount;
        //            //Pbar.Maximum = (int)totalCount;
        //            //Pbar.Value = (int)rowRead;
        //            //lblTip.Text = "正在写入[" + percent.ToString("0.00") + "%]...的数据";
        //            System.Windows.Forms.Application.DoEvents();

        //            for (int j = 0; j < tempPropertys.Length; j++)
        //            {
        //                //Lamda方式取值
        //                //if (tempPropertys[j].PropertyType.FullName == "System.Int32")
        //                //{
        //                //    var objectValue= tempPropertys[j].GetValue(tempList[i], null);
        //                //    sb.Append((objectValue == null ? string.Empty : objectValue.ToString()) + "\t");
        //                //}
        //                //else if (tempPropertys[j].PropertyType.FullName == "System.DateTime")
        //                //{
        //                //    var objectValue = tempPropertys[j].GetValue(tempList[i], null);
        //                //    sb.Append((objectValue == null ? string.Empty : objectValue.ToString()) + "\t");
        //                //}
        //                //else if (tempPropertys[j].PropertyType.FullName == "System.Double")
        //                //{
        //                //    var objectValue = tempPropertys[j].GetValue(tempList[i], null);
        //                //    sb.Append((objectValue == null ? string.Empty : objectValue.ToString()) + "\t");
        //                //}
        //                //else
        //                //{
        //                //    var objectValue = tempPropertys[j].GetValue(tempList[i], null);
        //                //    sb.Append((objectValue == null ? string.Empty : objectValue.ToString()) + "\t");
        //                //}
        //                var objectValue = tempPropertys[j].GetValue(tempList[i], null);
        //                sb.Append((objectValue == null ? string.Empty : objectValue.ToString()) + "\t");
        //            }
        //            sb.Append(Environment.NewLine);
        //        }
        //        sw.Write(sb.ToString());
        //        sw.Flush();
        //        sw.Close();
        //        MessageBox.Show("已经生成指定Excel文件!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //#endregion

        //#region 导出Excel-根据数据源+属性过滤方式导出Excel-单对象导出

        //public static void ExportExcel<T>(IList<T> tempList, IDictionary<string, string> propertyDictionarys) where T : class, new()
        //{
        //    SaveFileDialog tempSaveFileDialog = new SaveFileDialog();
        //    tempSaveFileDialog.Title = "保存的excel文件";
        //    tempSaveFileDialog.InitialDirectory = "c:\\";
        //    tempSaveFileDialog.Filter = "Excel97-2003 (*.xls)|*.xls|All Files (*.*)|*.*";
        //    tempSaveFileDialog.ShowDialog();
        //    if (tempSaveFileDialog.FileName == "" || tempSaveFileDialog.FileName == null)
        //    {
        //        MessageBox.Show("文件名不能为空!");
        //        return;
        //    }
        //    string path = tempSaveFileDialog.FileName;
        //    WriteExcel<T>(tempList, propertyDictionarys, path);
        //}

        //private static void WriteExcel<T>(IList<T> tempList, IDictionary<string, string> propertyDictionarys, string path) where T : class, new()
        //{
        //    try
        //    {
        //        long totalCount = tempList.Count;
        //        Thread.Sleep(1000);
        //        long rowRead = 0;
        //        //float percent = 0;

        //        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));
        //        StringBuilder sb = new StringBuilder();
        //        var tempPropertys = typeof(T).GetProperties();
        //        foreach (var propertyKeyValue in propertyDictionarys)
        //        {
        //            sb.Append(propertyKeyValue.Value.ToString() + "\t");
        //        }

        //        //for (int k = 0; k < propertyDictionarys.Count; k++)
        //        //{
        //        //    sb.Append(propertyDictionarys.i[k].Name.ToString() + "\t");
        //        //}
        //        sb.Append(Environment.NewLine);

        //        for (int i = 0; i < tempList.Count; i++)
        //        {
        //            rowRead++;
        //            //percent = ((float)(100 * rowRead)) / totalCount;
        //            //Pbar.Maximum = (int)totalCount;
        //            //Pbar.Value = (int)rowRead;
        //            //lblTip.Text = "正在写入[" + percent.ToString("0.00") + "%]...的数据";
        //            System.Windows.Forms.Application.DoEvents();

        //            foreach (var propertyKeyValue in propertyDictionarys)
        //            {
        //                var tempProperty = tempPropertys.Where(pre => pre.Name == propertyKeyValue.Key).FirstOrDefault();

        //                if (tempProperty == null)
        //                {
        //                    sb.Append("" + "\t");
        //                    continue;
        //                }

        //                //Emit方式取值
        //                var objectValue = NS.Framework.Utility.Reflection.ReflectHelper.GetPropertyValueEmit(tempList[i], tempProperty);
        //                sb.Append(objectValue.ToString() + "\t");
        //            }

        //            //for (int j = 0; j < tempPropertys.Length; j++)
        //            //{
        //            //    //Emit方式取值
        //            //    var objectValue = NS.Framework.Utility.Reflection.ReflectHelper.GetPropertyValueEmit(tempList[i], tempPropertys[j]);
        //            //    sb.Append(objectValue.ToString() + "\t");
        //            //}
        //            sb.Append(Environment.NewLine);
        //        }
        //        sw.Write(sb.ToString());
        //        sw.Flush();
        //        sw.Close();
        //        MessageBox.Show("已经生成指定Excel文件!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //#endregion

        //#region 导出Excel-根据数据源+属性过滤方式导出Excel-复合导出

        //public static void ExportExcel(ExportExcelContract tempList)
        //{
        //    SaveFileDialog tempSaveFileDialog = new SaveFileDialog();
        //    tempSaveFileDialog.Title = "保存的excel文件";
        //    tempSaveFileDialog.InitialDirectory = "c:\\";
        //    tempSaveFileDialog.Filter = "Excel97-2003 (*.xls)|*.xls|All Files (*.*)|*.*";
        //    tempSaveFileDialog.ShowDialog();
        //    if (tempSaveFileDialog.FileName == "" || tempSaveFileDialog.FileName == null)
        //    {
        //        MessageBox.Show("文件名不能为空!");
        //        return;
        //    }
        //    string path = tempSaveFileDialog.FileName;
        //    WriteExcel(tempList, path);
        //}

        //private static void WriteExcel(ExportExcelContract tempList, string path)
        //{
        //    try
        //    {
        //        long totalCount = tempList.DataSource.Max(pre => pre.Value.Count);
        //        Thread.Sleep(1000);
        //        long rowRead = 0;
        //        //float percent = 0;

        //        StreamWriter sw = new StreamWriter(path, false, Encoding.GetEncoding("gb2312"));
        //        StringBuilder sb = new StringBuilder();

        //        IDictionary<Type, IList<System.Reflection.PropertyInfo>> tempPropertyDictionary = new Dictionary<Type, IList<System.Reflection.PropertyInfo>>();

        //        //输出属性
        //        foreach (var propertyKeyValue in tempList.DataPropertys)
        //        {
        //            foreach (var itemKeyValue in propertyKeyValue.Value)
        //            {
        //                sb.Append(itemKeyValue.Value.ToString() + "\t");
        //            }

        //            if (tempPropertyDictionary.ContainsKey(propertyKeyValue.Key))
        //                continue;

        //            tempPropertyDictionary.Add(propertyKeyValue.Key, propertyKeyValue.Key.GetProperties());
        //        }

        //        sb.Append(Environment.NewLine);

        //        //输出值
        //        for (int i = 0; i < totalCount; i++)
        //        {
        //            rowRead++;
        //            //percent = ((float)(100 * rowRead)) / totalCount;
        //            //Pbar.Maximum = (int)totalCount;
        //            //Pbar.Value = (int)rowRead;
        //            //lblTip.Text = "正在写入[" + percent.ToString("0.00") + "%]...的数据";
        //            System.Windows.Forms.Application.DoEvents();

        //            DataItem firstDataItem = null;
        //            foreach (var propertyKeyValue in tempList.DataPropertys)
        //            {
        //                var tempDataSource = tempList.DataSource[propertyKeyValue.Key];
        //                var tempProperties = tempPropertyDictionary[propertyKeyValue.Key];
        //                DataItem tempDataItem = null;
        //                if (firstDataItem == null)
        //                {
        //                    firstDataItem = tempDataSource[i];
        //                    tempDataItem = firstDataItem;
        //                }
        //                else
        //                {
        //                    tempDataItem = tempDataSource.Where(pre => pre.PrimaryKey == firstDataItem.PrimaryKey).FirstOrDefault();
        //                }

        //                foreach (var ItemKeyValue in propertyKeyValue.Value)
        //                {
        //                    var tempProperty = tempProperties.Where(pre => pre.Name == ItemKeyValue.Key).FirstOrDefault();

        //                    if (tempDataSource.Count <= i || tempProperty == null || tempDataItem == null)
        //                    {
        //                        sb.Append("" + "\t");
        //                        continue;
        //                    }

        //                    if (tempProperty.PropertyType == typeof(DateTime))
        //                    {
        //                        var tempTime = (DateTime)tempProperty.GetValue(tempDataItem.Value, null);
        //                        sb.Append(tempTime.ToString("yyyy-MM-dd HH:mm:ss") + "\t");
        //                        continue;
        //                    }

        //                    //Emit方式取值
        //                    var objectValue = NS.Framework.Utility.Reflection.ReflectHelper.LmdGet(propertyKeyValue.Key, ItemKeyValue.Key).Invoke(tempDataItem.Value);

        //                    if (objectValue == null)
        //                    {
        //                        sb.Append("" + "\t");
        //                        continue;
        //                    }

        //                    sb.Append(objectValue.ToString() + "\t");
        //                }
        //            }

        //            sb.Append(Environment.NewLine);
        //        }
        //        sw.Write(sb.ToString());
        //        sw.Flush();
        //        sw.Close();
        //        MessageBox.Show("已经生成指定Excel文件!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        #endregion
    }

    /// <summary>
    /// Aspose 操控Excel的套件
    /// </summary>
    public class AsposeExcel
    {
        private string outFileName = "";
        private string fullFilename = "";
        private Workbook book = null;
        private Worksheet sheet = null;


        public AsposeExcel(string outfilename, string tempfilename)//导出构造数
        {
            outFileName = outfilename;
            book = new Workbook();
            // book.Open(tempfilename);这里我们暂时不用模板
            sheet = book.Worksheets[0];
        }
        public AsposeExcel(string fullfilename)//导入构造数
        {
            fullFilename = fullfilename;
            // book = new Workbook();
            //book.Open(tempfilename);
            //sheet = book.Worksheets[0];
        }

        private void AddTitle(string title, int columnCount)
        {
            sheet.Cells.Merge(0, 0, 1, columnCount);
            sheet.Cells.Merge(1, 0, 1, columnCount);

            Cell cell1 = sheet.Cells[0, 0];
            cell1.PutValue(title);
            cell1.GetStyle().HorizontalAlignment = TextAlignmentType.Center;
            cell1.GetStyle().Font.Name = "黑体";
            cell1.GetStyle().Font.Size = 14;
            cell1.GetStyle().Font.IsBold = true;

            Cell cell2 = sheet.Cells[1, 0];
            cell1.PutValue("查询时间：" + DateTime.Now.ToLocalTime());
            cell2.SetStyle(cell1.GetStyle());
        }

        private void AddHeader(DataTable dt)
        {
            Cell cell = null;
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                cell = sheet.Cells[0, col];
                cell.PutValue(dt.Columns[col].ColumnName);
                cell.GetStyle().Font.IsBold = true;
            }
        }

        private void AddBody(DataTable dt)
        {
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    sheet.Cells[r + 1, c].PutValue(dt.Rows[r][c].ToString());
                }
            }
        }
        //导出------------下一篇会用到这个方法
        public Boolean DatatableToExcel(DataTable dt)
        {
            Boolean yn = false;
            try
            {
                //sheet.Name = sheetName;

                //AddTitle(title, dt.Columns.Count);
                //AddHeader(dt);
                AddBody(dt);

                sheet.AutoFitColumns();
                //sheet.AutoFitRows();

                book.Save(outFileName);
                yn = true;
                return yn;
            }
            catch (Exception e)
            {
                return yn;
                // throw e;
            }
        }

        public IList<DataTable> ExcelToDatatalbe()//导入
        {
            IList<DataTable> tempTables = new List<DataTable>();

            Workbook book = new Workbook(fullFilename);
            //book.Open(fullFilename);

            if (book.Worksheets.Count == 0)
                return tempTables;

            foreach (Worksheet itemWorksheet in book.Worksheets)
            {
                Cells cells = itemWorksheet.Cells;
                //获取excel中的数据保存到一个datatable中
                DataTable dt_Import = cells.ExportDataTableAsString(0, 0, cells.MaxDataRow + 1, cells.MaxDataColumn + 1, false);

                dt_Import.TableName = itemWorksheet.Name + "$";

                tempTables.Add(dt_Import);
            }

            IList<DataTable> newDataTables = new List<DataTable>();

            foreach (var tempTable in tempTables)
            {
                //var newDataTable = new DataTable();

                var currentRow = tempTable.Rows[0];

                IDictionary<string, string> tempMapper = new Dictionary<string, string>();

                var columns = tempTable.Columns;

                foreach (System.Data.DataColumn column in columns)
                {
                    //column.ColumnName = (currentRow[column.ColumnName]!=null && string.IsNullOrEmpty(currentRow[column.ColumnName].ToString())) ? currentRow[column.ColumnName].ToString(): column.ColumnName;
                    tempMapper.Add(column.ColumnName, currentRow[column.ColumnName].ToString());
                }

                foreach (var item in tempMapper)
                {
                    var tempColumn = tempTable.Columns[item.Key];
                    tempColumn.ColumnName = string.IsNullOrEmpty(item.Value) ? tempColumn.ColumnName : item.Value;
                }

                tempTable.Rows.Remove(currentRow);
                //newDataTables.Add(newDataTable);
            }

            return tempTables;
        }
    }

    /// <summary>
    /// 导出Excel数据契约
    /// </summary>
    public class ExportExcelContract
    {
        public ExportExcelContract()
        {
            this.DataPropertys = new Dictionary<Type, IDictionary<string, string>>();
            this.DataSource = new Dictionary<Type, List<DataItem>>();
        }

        /// <summary>
        /// 待导出的对象及数据源
        /// </summary>
        public Dictionary<Type, List<DataItem>> DataSource
        {
            get;
            set;
        }

        /// <summary>
        /// 待导出的数据对象的属性集合
        /// </summary>
        public Dictionary<Type, IDictionary<string, string>> DataPropertys
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 数据项定义信息
    /// </summary>
    public class DataItem
    {
        public string PrimaryKey
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }
}
