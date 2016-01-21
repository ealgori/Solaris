using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;



namespace ExcelParser.ExcelExport
{
    public class NPOIReportService
    {
    public HSSFWorkbook Workbook { get; set; }
    private string WorkSheetName { get; set; }
   
   
    public byte[] GetBytes()
    {
        using (var buffer = new MemoryStream())
        {
            this.Workbook.Write(buffer);
            return buffer.GetBuffer();
        }
    }
    public void CopyDataToRange(List<string> DataList, string RangeName)
    {

        HSSFSheet sht = (HSSFSheet)Workbook.GetSheet(WorkSheetName);
        HSSFName range = (HSSFName)Workbook.GetName(RangeName);
        AreaReference AR = new AreaReference(range.RefersToFormula);
        int RangeColumn = AR.FirstCell.Col;
        //Номер строчки, с которой начинаем копировать, в Npoi коллекция начинается с нулевого элемента.
        //Учти это.
        int RangeRow = 1;
        for (int i = 0; i < DataList.Count; i++)
        {
            //Пытаемся получить строку
            var row = sht.GetRow(RangeRow);
            //Если строка полностью пустая(нет данных ни в одной ячейке, мы создаем новую строку
            if (row != null)
            {
                row.CreateCell(RangeColumn).SetCellValue(DataList[i]);
            }
            else
            {
                sht.CreateRow(RangeRow).CreateCell(RangeColumn).SetCellValue(DataList[i]);
            }
            RangeRow++;
        }
    }
       
        //public ReportData reportData { get; set; }
        public NPOIReportService()
        {
            Workbook = new HSSFWorkbook();
           // reportData = _reportData;
        }
        public void SaveReport(string ImportPath, string ImportName)
        {
            string ReportPath = Path.Combine(ImportPath, ImportName);

            if (!Directory.Exists(Path.GetDirectoryName(ReportPath)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName((ReportPath)));
                }
                catch (System.Exception ex)
                {

                }
            }
            using (FileStream fileData = new FileStream(ReportPath, FileMode.Create, FileAccess.ReadWrite))
            {
             
                Workbook.Write(fileData);
               
            }
            //DataContext context = new DataContext();
            //Report rep = context.Reports.Where(r => r.Id == reportData.ReportId).FirstOrDefault();
            //rep.Path = ReportPath;
            //rep.Status = Constants.Statuses.Completed.ToString();
            //context.SaveChanges();
        }
        public void FillReportData(DataTable exportData, string sheetName)
        {
            // Create the header row cell style
            var headerLabelCellStyle = Workbook.CreateCellStyle();
            var headerLabelFont = Workbook.CreateFont();
            headerLabelCellStyle.SetFont(headerLabelFont);

            var sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName, (HSSFCellStyle)headerLabelCellStyle);
            var currentNPOIRowIndex = 1;
            var sheetCount = 1;

            for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
            {
                // о разделении по страницам, вернее по документам уже позаботился нпоиэксельэкспорт
                //if (currentNPOIRowIndex >= objectsPerPage)
                //{
                //    sheetCount++;
                //    currentNPOIRowIndex = 1;

                //    sheet = CreateExportDataTableSheetAndHeaderRow(exportData, sheetName + " - " + sheetCount, (HSSFCellStyle)headerLabelCellStyle);
                //}

                var row = sheet.CreateRow(currentNPOIRowIndex++);

                for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
                {
                    var cell = row.CreateCell(colIndex);
                    // теоретически сюда мы никогда больше не попадем... так как все ToDataTable которые связаны с экселем, теперь должны быть ToDataTableText
                    if (exportData.Rows[rowIndex][colIndex].GetType() == typeof(DateTime))
                    {
                        cell.SetCellValue(((DateTime)exportData.Rows[rowIndex][colIndex]).ToString("dd-MM-yyyy"));
                    }
                    else
                        cell.SetCellValue(exportData.Rows[rowIndex][colIndex].ToString());
                }
            }
        }

        #region Вспомогательные методы

        //protected string EscapeSheetName(string sheetName)
        //{
        //    var escapedSheetName = sheetName
        //                                .Replace("/", "-")
        //                                .Replace("\\", " ")
        //                                .Replace("?", string.Empty)
        //                                .Replace("*", string.Empty)
        //                                .Replace("[", string.Empty)
        //                                .Replace("]", string.Empty)
        //                                .Replace(":", string.Empty);

        //    if (escapedSheetName.Length > ImportParams.MaximumSheetNameLength)
        //        escapedSheetName = escapedSheetName.Substring(0, ImportParams.MaximumSheetNameLength);

        //    return escapedSheetName;
        //}

        protected HSSFSheet CreateExportDataTableSheetAndHeaderRow(DataTable exportData, string sheetName, HSSFCellStyle headerRowStyle)
        {
            var sheet = Workbook.CreateSheet(sheetName);

            // Create the header row
            var row = sheet.CreateRow(0);

            for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
            {
                var cell = row.CreateCell(colIndex);
                cell.SetCellValue(exportData.Columns[colIndex].ColumnName);

                if (headerRowStyle != null)
                    cell.CellStyle = headerRowStyle;
            }

            return (HSSFSheet)sheet;
        }
        #endregion


    }
}
