using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.Util;
using System.Collections;

using NLog;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.ImportHandler.Models.ExcelExport
{
    public class NPOIExcelExport
    {
        public static class ImportToSH
        {

            private static Logger logger = LogManager.GetCurrentClassLogger();
            public static  List<byte[]> ImportParamsToListWorkbooks(ImportParams importParams, int objectsPerPage)
            {

                // Узнать количество объектов в эррэй лист
                if (objectsPerPage <= 0)
                    objectsPerPage = 65500;
                int pageCount = importParams.Objects.Count / objectsPerPage + 1;

                List<byte[]> files = new List<byte[]>();
                for (int page = 0; page < pageCount; page++)
                {
                    var partDataList = importParams.Objects.Count > objectsPerPage ?
                        importParams.Objects.GetRange(page * objectsPerPage, objectsPerPage) : importParams.Objects;
                    var dataTable = ArrayListToDataTableText(partDataList);

                    try
                    {

                       
                        // сашин сервис
                        NPOIReportService service = new NPOIReportService();
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();
                        // вставляет данные в воркбук. Виртуальная структура
                        service.FillReportData(dataTable, "Sheet1");
                        sw.Stop();
                        logger.Info("Готовили данные: " + sw.Elapsed.TotalSeconds + " сек");
                        sw.Restart();

                      
                        files.Add(GetBytes(service.Workbook));

                       // service.SaveReport(ImportParams.TempPath, fileName);
                        sw.Stop();
                        System.Diagnostics.Debug.WriteLine("Писали в файл : " + sw.Elapsed.TotalSeconds + " сек");
                       
                    }
                    catch (System.Exception ex)
                    {
                        logger.Error(ex.Message);
                       
                    }

                    importParams = null;
                    //GC.SuppressFinalize(importParams.Objects);
                    //GC.SuppressFinalize(importParams);
                    //GC.Collect(2);
                }
                return files;
            }
        }
        public static DataTable ArrayListToDataTable(ArrayList alist)
        {
            DataTable dt = new DataTable();
            if (alist == null || alist.Count == 0)
            {
                return dt;
            }
            if (alist[0] == null)
                throw new FormatException("Parameter ArrayList empty");
            dt.TableName = alist[0].GetType().Name;
            DataRow dr;
            System.Reflection.PropertyInfo[] propInfo = alist[0].GetType().GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                //dt.Columns.Add(propInfo[i].Name, propInfo[i].PropertyType);
                dt.Columns.Add(propInfo[i].Name, Nullable.GetUnderlyingType(propInfo[i].PropertyType) ?? propInfo[i].PropertyType); 
            }

            for (int row = 0; row < alist.Count; row++)
            {
                dr = dt.NewRow();
                for (int i = 0; i < propInfo.Length; i++)
                {
                    object tempObject = alist[row];

                    object t = propInfo[i].GetValue(tempObject, null);
                    /*object t =tempObject.GetType().InvokeMember(propInfo[i].Name,
                             R.BindingFlags.GetProperty , null,tempObject , new object [] {});*/
                    if (t != null)
                        dr[i] = t.ToString();
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// Только для записи в эксель. Изменяет все типы полей на стринг. 
        /// </summary>
        /// <param name="alist"></param>
        /// <returns></returns>
        public static DataTable ArrayListToDataTableText(ArrayList alist)
        {
            DataTable dt = new DataTable();
            if (alist == null || alist.Count == 0)
            {
                return dt;
            }
            if (alist[0] == null)
                throw new FormatException("Parameter ArrayList empty");
            dt.TableName = alist[0].GetType().Name;
            DataRow dr;
            System.Reflection.PropertyInfo[] propInfo = alist[0].GetType().GetProperties();
            for (int i = 0; i < propInfo.Length; i++)
            {
                //dt.Columns.Add(propInfo[i].Name, propInfo[i].PropertyType);
                dt.Columns.Add(propInfo[i].Name, typeof(string));
            }

            for (int row = 0; row < alist.Count; row++)
            {
                dr = dt.NewRow();
                for (int i = 0; i < propInfo.Length; i++)
                {
                    object tempObject = alist[row];

                    object t = propInfo[i].GetValue(tempObject, null);
                    /*object t =tempObject.GetType().InvokeMember(propInfo[i].Name,
                             R.BindingFlags.GetProperty , null,tempObject , new object [] {});*/
                    if (t != null)
                    {
                        
                        if (t.GetType() == typeof(DateTime) || t.GetType() == typeof(Nullable<DateTime>))
                        {
                            dr[i] = ((DateTime)t).ToString("dd-MM-yyyy");
                        }
                        else
                        {
                            dr[i] = t.ToString();
                        }
                       
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static byte[] GetBytes(HSSFWorkbook workbook)
        {
            using (var buffer = new MemoryStream())
            {
                workbook.Write(buffer);
                return buffer.GetBuffer();
            }
        }
    }
   
}

    
//        const int MaximumNumberOfRowsPerSheet = 65500;
//        const int MaximumSheetNameLength = 25;
//        // public HSSFWorkbook Workbook { get; set; }
//        private string WorkSheetName { get; set; }
//        public NPOIExcelExport(HSSFWorkbook _Workbook)
//        {
//            //Workbook = _Workbook;
//        }
//        public NPOIExcelExport(FileStream _FileStream, string _WorkSheetName)
//        {
//            //Workbook = new HSSFWorkbook(_FileStream, true);
//            WorkSheetName = _WorkSheetName;
//        }

//       
//        public List<MemoryStream> ExportArrayListToMemoryStreamsList(ArrayList exportData, string sheetName)
//        {
//            List<MemoryStream> memoryStreams = new List<MemoryStream>();

//            exportData.ta


//            var sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName);
//            var currentNPOIRowIndex = 1;
//            var sheetCount = 1;


//            int count = 3;

//            List<int> Data = new List<int>();

//            for (int index = 0; index < count; index++)
//            {
//                //HSSFWorkbook workBook = new HSSFWorkbook();
//                //var sht = workBook.CreateSheet();
//                //#region HeaderRow

//                //var row = sht.CreateRow(0);
//                //for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//                //{
//                //    var cell = row.CreateCell(colIndex);
//                //    cell.SetCellValue(exportData.Columns[colIndex].ColumnName);
//                //}

//                //#endregion




//                for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//                {
//                    var cell = row.CreateCell(colIndex);
//                    cell.SetCellValue(exportData.Rows[rowIndex][colIndex].ToString());

//                    for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
//                    {

//                    }

//                }

//            }


//            for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
//            {
//                if (currentNPOIRowIndex >= MaximumNumberOfRowsPerSheet)
//                {
//                    sheetCount++;
//                    currentNPOIRowIndex = 1;

//                    sheet = CreateExportDataTableSheetAndHeaderRow(exportData,
//                                                                    sheetName + " - " + sheetCount
//                                                                    );
//                }

//                var row = sheet.CreateRow(currentNPOIRowIndex++);

//                for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//                {
//                    var cell = row.CreateCell(colIndex);
//                    cell.SetCellValue(exportData.Rows[rowIndex][colIndex].ToString());
//                }
//            }

//            HSSFWorkbook Workbook = new HSSFWorkbook();



//        }



//        public void ExportDataTableToWorkbook(DataTable exportData, string sheetName)
//        {
//            // Create the header row cell style


//            var sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName);
//            var currentNPOIRowIndex = 1;
//            var sheetCount = 1;

//            for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
//            {
//                if (currentNPOIRowIndex >= MaximumNumberOfRowsPerSheet)
//                {
//                    sheetCount++;
//                    currentNPOIRowIndex = 1;

//                    sheet = CreateExportDataTableSheetAndHeaderRow(exportData,
//                                                                    sheetName + " - " + sheetCount
//                                                                    );
//                }

//                var row = sheet.CreateRow(currentNPOIRowIndex++);

//                for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//                {
//                    var cell = row.CreateCell(colIndex);
//                    cell.SetCellValue(exportData.Rows[rowIndex][colIndex].ToString());
//                }
//            }
//        }

//        protected string EscapeSheetName(string sheetName)
//        {
//            var escapedSheetName = sheetName
//                                        .Replace("/", "-")
//                                        .Replace("\\", " ")
//                                        .Replace("?", string.Empty)
//                                        .Replace("*", string.Empty)
//                                        .Replace("[", string.Empty)
//                                        .Replace("]", string.Empty)
//                                        .Replace(":", string.Empty);

//            if (escapedSheetName.Length > MaximumSheetNameLength)
//                escapedSheetName = escapedSheetName.Substring(0, MaximumSheetNameLength);

//            return escapedSheetName;
//        }

//        public void FillReportData(DataTable exportData, string sheetName)
//        {
//            // Create the header row cell style
//            var headerLabelCellStyle = Workbook.CreateCellStyle();
//            var headerLabelFont = Workbook.CreateFont();
//            headerLabelCellStyle.SetFont(headerLabelFont);

//            var sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName, (HSSFCellStyle)headerLabelCellStyle);
//            var currentNPOIRowIndex = 1;
//            var sheetCount = 1;

//            for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
//            {
//                if (currentNPOIRowIndex >= MaximumNumberOfRowsPerSheet)
//                {
//                    sheetCount++;
//                    currentNPOIRowIndex = 1;

//                    sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName + " - " + sheetCount, (HSSFCellStyle)headerLabelCellStyle);
//                }

//                var row = sheet.CreateRow(currentNPOIRowIndex++);

//                for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//                {
//                    var cell = row.CreateCell(colIndex);
//                    cell.SetCellValue(exportData.Rows[rowIndex][colIndex].ToString());
//                }
//            }
//        }

//        protected HSSFWorkbook CreateExportDataTableSheetAndHeaderRow(DataTable exportData, string sheetName)
//        {
//            HSSFWorkbook workBook = new HSSFWorkbook();
//            var sheet = workBook.CreateSheet(EscapeSheetName(sheetName));

//            // Create the header row
//            var row = sheet.CreateRow(0);

//            for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
//            {
//                var cell = row.CreateCell(colIndex);
//                cell.SetCellValue(exportData.Columns[colIndex].ColumnName);


//            }

//            return workBook;
//        }
//    }
//}