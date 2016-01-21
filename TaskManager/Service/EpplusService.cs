using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Table;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.Style;

namespace TaskManager.Service
{
    public class EpplusService : IDisposable
    {
        public ExcelPackage app { get; set; }
        public EpplusService()
        {
            app = new ExcelPackage();
        }
        public EpplusService(FileInfo File)
        {
            app = new ExcelPackage(File, true);
        }
        //Сохранение эксельки в любую желаемую(несуществующую) папку
        public void CreateFolderAndSaveBook(string DesiredPath, string FileName)
        {
            if (!Directory.Exists(DesiredPath))
            {
                Directory.CreateDirectory(DesiredPath);
            }
            FileInfo Output = new FileInfo(Path.Combine(DesiredPath, FileName));
            app.SaveAs(Output);
        }

        public void ReplaceDataInSheet(ExcelWorksheet sht, Dictionary<string, string> dict, bool ReplaceInColontitules = false)
        {
            foreach (var item in dict.Keys)
            {
                if (ReplaceInColontitules)
                {
                    //Заменяем поля в колонтитулах
                    if (sht.HeaderFooter.OddFooter != null && sht.HeaderFooter.OddHeader != null)
                    {
                        if (sht.HeaderFooter.OddFooter.LeftAlignedText != null && sht.HeaderFooter.OddFooter.RightAlignedText != null && sht.HeaderFooter.OddHeader.LeftAlignedText != null && sht.HeaderFooter.OddHeader.RightAlignedText != null)
                        {
                            sht.HeaderFooter.OddFooter.LeftAlignedText = sht.HeaderFooter.OddFooter.LeftAlignedText.Replace(string.Format("#{0}#", item), dict[item]);
                            sht.HeaderFooter.OddFooter.RightAlignedText = sht.HeaderFooter.OddFooter.RightAlignedText.Replace(string.Format("#{0}#", item), dict[item]);
                            sht.HeaderFooter.OddHeader.LeftAlignedText = sht.HeaderFooter.OddHeader.LeftAlignedText.Replace(string.Format("#{0}#", item), dict[item]);
                            sht.HeaderFooter.OddHeader.RightAlignedText = sht.HeaderFooter.OddHeader.RightAlignedText.Replace(string.Format("#{0}#", item), dict[item]);
                        }
                    }  
                }
                
                try
                {
                    var cellList = sht.FindCells(item);

                    foreach (var cell in cellList)
                    {
                        cell.Value = cell.Text.Replace(string.Format("#{0}#", item), dict[item]);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void ReplaceDataInBook(Dictionary<string, string> dict, bool ReplaceInColontitules = false)
        {
            foreach (var sht in app.Workbook.Worksheets)
            {
                ReplaceDataInSheet(sht, dict, ReplaceInColontitules);
            }
        }
        public class InsertTableParams
        {

            public bool EmptyRowAfterHeaders { get; set; }
            /// <summary>
            /// Вставлять ли заголовки из дата тейбла. Если заголовки не вставляются, то никакое визуальное оформление к таблице не будет применено.
            /// </summary>
            public bool PrintHeaders { get; set; }
            /// <summary>
            /// Стилизованный заголовок. тип берется из стиля. подразумевается автофильтр на заголовках. 
            /// </summary>
            public bool StyledHeaders { get; set; }
            /// <summary>
            /// Жирный шрифт в заголовке.
            /// </summary>
            public bool BoldHeaders { get; set; }
            /// <summary>
            /// Полосатые колонки
            /// </summary>
            public bool ShowColumnStripes { get; set; }
            /// <summary>
            /// Полосатые строки
            /// </summary>
            public bool ShowRowStripes { get; set; }
            /// <summary>
            /// Минимальное количество строк, отделяющее полную таблицу от данных находящихся под ней. если данных будет больше то вставяться новые строки. Если меньше нуля, то с перезаписью
            /// </summary>
            public int MinSeparatedRows { get; set; }
            /// <summary>
            /// Перенести стиль с первой строки на все вставляемые
            /// </summary>
            public bool CopyFirstRowStyle { get; set; }
            /// <summary>
            /// Стиль таблицы
            /// </summary>
            public OfficeOpenXml.Table.TableStyles TableStyle { get; set; }

            public List<DrawParam> DrawParams { get; set; }
            public InsertTableParams()
            {
                MinSeparatedRows = -1;
                TableStyle = OfficeOpenXml.Table.TableStyles.None;
                StyledHeaders = false;
                DrawParams = new List<DrawParam>();
            }


        }

        public abstract class DrawParam
        {
            public System.Drawing.Color Color { get; set; }
        }
        public class FillRow : DrawParam
        {
            public int RowNum { get; set; }
        }
        public class FillColumn : DrawParam
        {
            public int ColumnNum { get; set; }
        }
        public class FillCellByColumnNum : DrawParam
        {
            public int RowNum { get; set; }
            public int ColumnNum { get; set; }
        }
        public class FillCellByColumnName : DrawParam
        {
            public int RowNum { get; set; }
            public string ColumnName { get; set; }
        }




        /// <summary>
        /// вставка талицы в именованную ячейку по указанному имени шита
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="namedRange"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        /// <returns></returns>
        public string InsertTableToNamedRange(string sheetName, string namedRange, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            try
            {
                var sheet = app.Workbook.Worksheets[sheetName];
                if (sheet != null)
                    return InsertTableToNamedRange(sheet, namedRange, dataTable, insertParameters);
                else
                    return null;
            }
            catch (Exception exc)
            {
                return null;
            }

        }

        /// <summary>
        /// Вставляет таблицу в указанный патерн во всей рабочей книге
        /// </summary>
        /// <param name="namedRange"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        public void InsertTableToPatternCellInWorkBook(string patternText, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            foreach (var sheet in app.Workbook.Worksheets)
            {
                InsertTableToPatternCell(sheet, patternText, dataTable, insertParameters);
            }
        }


        /// <summary>
        /// Вставляет таблицу в указанный именованный диапазон во всей рабочей книге
        /// </summary>
        /// <param name="namedRange"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        public void InsertTableToNamedRangeInWorkBook(string patternText, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            foreach (var sheet in app.Workbook.Worksheets)
            {
                InsertTableToNamedRange(sheet, patternText, dataTable, insertParameters);
            }
        }


        /// <summary>
        /// вставка таблицы в именованную ячейку в данном воркшите
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startPattern"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        /// <returns>Если вернулась пустая строка, значит нет паттерна такого, если null значит эксепшн при работе</returns>
        public string InsertTableToNamedRange(ExcelWorksheet sheet, string namedRange, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            var startCell = sheet.Names[namedRange];
            if (startCell != null)
            {
                InsertTable(sheet, startCell, dataTable, insertParameters);
            }
            return string.Empty;
        }
        /// <summary>
        /// Общий метод вставки таблицы
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startCell"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        /// <returns></returns>
        /// 

        private string InsertTable(ExcelWorksheet sheet, ExcelRangeBase startCell, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            try
            {
                int width = dataTable.Columns.Count;
                int height = dataTable.Rows.Count + (insertParameters.PrintHeaders ? 1 : 0) + (insertParameters.EmptyRowAfterHeaders ? 1 : 0);





                var headerCell = startCell.Offset(0, 0, 1, width);
                // чтобы не затереть данные под таблицей, если это требуется, вставляем пустые строки, которые потом заполняться таблицей
                if (insertParameters.MinSeparatedRows > 0)
                {
                    var loweredRows = sheet.Cells.Where(c => c.Start.Row > startCell.Start.Row && !string.IsNullOrWhiteSpace(c.Text));
                    var lowerRow = sheet.Cells.Where(c => c.Start.Row > startCell.Start.Row).OrderBy(ce => ce.Start.Row).FirstOrDefault();
                    if (lowerRow != null)
                    {
                        //                ячейка с флагом       высота   минимум перед текстом
                        int forecastEnd = startCell.Start.Row + height;
                        // расстояние между текстом и началом
                        int distance = lowerRow.Start.Row -  // тексто под таблицей
                            startCell.Start.Row;// начало таблицы(+1 значит что считая и эту ячейку)


                        // если между текстом и началом таблицы больше места чем надо, то вставляем просто. если нет, то идем в иф
                        if (distance < height + insertParameters.MinSeparatedRows)
                        {
                            //                     тот пробел что уже есть, минимальное количество,             то что еще надо
                            int requiredRowCount = insertParameters.MinSeparatedRows + (height - distance);
                            sheet.InsertRow(headerCell.Start.Row + 1, requiredRowCount);
                        }




                    }
                }
                // собственно вставка даных
                startCell.LoadFromDataTable(dataTable, insertParameters.PrintHeaders);
                // пробел после заголовков
                if (insertParameters.EmptyRowAfterHeaders)
                {
                    sheet.InsertRow(headerCell.Start.Row + 1, 1);
                }
                // стили
                var firstRow = startCell.Offset(0, 0, 1, width);
                if (insertParameters.CopyFirstRowStyle)
                {
                    
                    for (int row = 1; row < height; row++)
                    {
                        for (int cel = 0; cel < width; cel++)
                        {
                            sheet.Cells[firstRow.Start.Row + row, firstRow.Start.Column + cel].StyleID = sheet.Cells[firstRow.Start.Row, firstRow.Start.Column + cel].StyleID;
                        }
                    }
                }
                // раскрашивание ячеек
                if (insertParameters.DrawParams.Count > 0)
                {
                    foreach (var drawParam in insertParameters.DrawParams)
                    {
                        ExcelRangeBase range = null;
                        // если тип раскрашивания ячейки по имени
                        if (drawParam is FillCellByColumnName)
                        {
                            string columnName = ((FillCellByColumnName)drawParam).ColumnName;
                            int colIndex = dataTable.Columns.IndexOf(columnName);
                            if (colIndex > -1)
                            {
                                // мы и так находимся в первой ячейке. поэтому относительный рад должен быть минус один.
                                int rowNum = ((FillCellByColumnName)drawParam).RowNum + (insertParameters.PrintHeaders ? 1 : 0) + (insertParameters.EmptyRowAfterHeaders ? 1 : 0)-1;
                                range = firstRow.Offset(rowNum, colIndex, 1, 1);
                            }
                        }
                        if (drawParam is FillRow)
                        {
                            int rowNum = ((FillRow)drawParam).RowNum + (insertParameters.PrintHeaders ? 1 : 0)+ (insertParameters.EmptyRowAfterHeaders ? 1 : 0)-1;
                            range = firstRow.Offset(rowNum, 0);
                        }


                        if (range != null)
                        {
                            var color = drawParam.Color;
                            if (color != null)
                            {
                                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                range.Style.Fill.BackgroundColor.SetColor(color);
                            }
                        }
                    }
                }

                // участок добавления элемента таблица. добавляетяс только если есть хедеры. иначе неприятные последствия могут быть
                var tableCells = startCell.Offset(0, 0, height, width);
                if (insertParameters.PrintHeaders)
                {
                    var table = sheet.Tables.Add(tableCells, string.Format("table{0}", DateTime.Now.ToString("yyyyMMddHHmmssfffffff")));

                    table.ShowColumnStripes = insertParameters.ShowColumnStripes;
                    if (insertParameters.BoldHeaders)
                    {

                        headerCell.Style.Font.Bold = true;

                    }
                    else
                    {
                        headerCell.Style.Font.Bold = false;
                    }
                    table.TableStyle = insertParameters.TableStyle;
                    table.ShowHeader = insertParameters.StyledHeaders;
                    table.ShowRowStripes = insertParameters.ShowRowStripes;

                    return table.Name;
                }
                return string.Empty;
            }
            catch (Exception exc)
            {
                return null;
            }
        }


        /// <summary>
        /// вставка таблицы в шаблонную ячейку по имени воркшита
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="patternText"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        /// <returns></returns>
        public string InsertTableToPatternCell(string sheetName, string patternText, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            try
            {
                var sheet = app.Workbook.Worksheets[sheetName];
                if (sheet != null)
                {
                    return InsertTableToPatternCell(sheet, patternText, dataTable, insertParameters);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exc)
            {
                return null;
            }
        }

        /// <summary>
        /// вставка таблицы в шаблонную ячейку в указанный воркшит
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="startPattern"></param>
        /// <param name="dataTable"></param>
        /// <param name="insertParameters"></param>
        /// <returns>Если вернулась пустая строка, значит нет паттерна такого, если null значит эксепшн при работе</returns>
        public string InsertTableToPatternCell(ExcelWorksheet sheet, string patternText, System.Data.DataTable dataTable, InsertTableParams insertParameters)
        {
            string pattern = string.Format("#{0}#", patternText);
            var startCell = sheet.Cells.FirstOrDefault(c => c.Text == pattern);
            if (startCell != null)
            {
                return InsertTable(sheet, startCell, dataTable, insertParameters);
            }
            return string.Empty;
        }









        //public void SaveReport()
        //{
        //    DataContext context = new DataContext();
        //    Report rep = context.Reports.Where(r => r.Id == reportData.ReportId).FirstOrDefault();
        //    if (rep != null)
        //    {
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            package.SaveAs(ms);
        //            rep.File = ms.ToArray();
        //            rep.FileExt = Constants.XlsxExt;
        //        }
        //        rep.Status = Constants.Statuses.Completed.ToString();
        //        context.SaveChanges();
        //    }
        //}
        //public void FillReportData(DataTable exportData, string sheetName)
        //{
        //    try
        //    {
        //        ExcelWorksheet sht = GetSheet(package.Workbook, sheetName);
        //        for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
        //        {
        //            sht.Cells[1, colIndex + 1].Value = exportData.Columns[colIndex].ColumnName;
        //        }

        //        for (var rowIndex = 0; rowIndex < exportData.Rows.Count; rowIndex++)
        //        {
        //            for (var colIndex = 0; colIndex < exportData.Columns.Count; colIndex++)
        //            {
        //                sht.Cells[rowIndex + 2, colIndex + 1].Value = exportData.Rows[rowIndex][colIndex];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        /// <summary>
        /// Получение заданного листа
        /// </summary>
        /// 
        /// <param name="shtName">Название листа</param>
        /// <returns></returns>
        public ExcelWorksheet GetSheet(string shtName)
        {
            //Бежим по всем листам и проверяем, есть ли среди них нужный нам
            foreach (var item in app.Workbook.Worksheets)
            {
                if (item.Name == shtName)
                {
                    return item;
                }
            }
            //если нужный не нашелся, создаем его и возвращаем
            return app.Workbook.Worksheets.Add(shtName);
        }
        public ExcelNamedRange GetRange(string RangeName)
        {
            return app.Workbook.Names.Where(s => s.Name == RangeName).FirstOrDefault();
        }
        public void AutoFit(ExcelWorksheet sht)
        {

            for (int i = sht.Dimension.Start.Column; i <= sht.Dimension.End.Column; i++)
            {
                sht.Column(i).AutoFit();
            }
        }
        public bool FormatColumn(ExcelWorksheet sht, string colname, string format)
        {
            for (int i = sht.Dimension.Start.Column; i <= sht.Dimension.End.Column; i++)
            {
                if (sht.Cells[1, i].Value == colname)
                {
                    sht.Column(i).Style.Numberformat.Format = format;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public int GetColumn(ExcelWorksheet sht, string colname, int row)
        {
            for (int i = sht.Dimension.Start.Column; i <= sht.Dimension.End.Column; i++)
            {
                if (sht.Cells[row, i].Text == colname)
                {

                    return i;
                }
            }
            return int.MinValue;
        }

        public void Dispose()
        {
            app.Dispose();
        }

       
    }
    public static class Excel2PDF
    {
        public static bool XLSConvertToPDF(string sourceFile, string targetFile)
        {
            bool result = false;

            Microsoft.Office.Interop.Excel.XlFixedFormatType targetType = Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF;// Excel.XlFixedFormatType.xlTypePDF;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Application application = null;
            Microsoft.Office.Interop.Excel.Workbook workBook = null;

            try
            {
                application = new Microsoft.Office.Interop.Excel.Application();
              //  application.AutomationSecurity= Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityByUI;
               //application.Visible = true;
                object target = targetFile;
                object type = targetType;
                workBook = application.Workbooks.Open(sourceFile
                    , missing
                    , missing
                    , missing
                    , missing
                    , missing
                    , missing
                    , missing
                    , missing
                    , true
                    , missing
                    , missing
                    , missing
                    , missing
                    , missing);

                // принудительный запуск макроса
                try
                {
                    workBook.Worksheets[1].Activate();
                    application.Run("WorkbookOpen");
             //       RunMacro(application, new Object[] {"WorkbookOpen" });
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.WriteLine(exc.Message);
                }
                if (workBook != null)
                {
                    XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
                    XlFixedFormatQuality paramExportQuality =
                        XlFixedFormatQuality.xlQualityStandard;
                    bool paramOpenAfterPublish = false;
                    bool paramIncludeDocProps = false;
                    bool paramIgnorePrintAreas = false;
                    object paramFromPage = 1;
                    object paramToPage = 100;
                    
                    //workBook.ExportAsFixedFormat(targetType
                    //    , target
                    //    , Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard
                    //    , true
                    //    , true
                    //    , 1
                    //    , 3
                    //    , false
                    //    , missing);
                    for (int i = 1; i < workBook.Worksheets.Count; i++)
                    {
                        var ws = workBook.Worksheets[i];
                        ((Microsoft.Office.Interop.Excel.Worksheet)ws).PageSetup.PaperSize = XlPaperSize.xlPaperA4;
                        ((Microsoft.Office.Interop.Excel.Worksheet)ws).PageSetup.CenterHorizontally = true;
                        
                    }
                   
                   workBook.ExportAsFixedFormat(paramExportFormat,
                   targetFile, paramExportQuality,
                   paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                   paramToPage, paramOpenAfterPublish,
                   Type.Missing);
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(true, missing, missing);
                    workBook = null;
                }

                if (application != null)
                {
                    application.Quit();
                    application = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return result;
        }

        private static void RunMacro(object oApp, object[] oRunArgs)
        {
            oApp.GetType().InvokeMember("Run", System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, oApp, oRunArgs);
        }

    }
    public static class EpplusExtensions
    {
        public static List<ExcelRangeBase> FindCells(this ExcelWorksheet sheet, string text)
        {
            return sheet.Cells.Where(c => (c.Value ?? "").ToString().Contains(string.Format("#{0}#", text))).ToList();
        }
        public static ExcelRangeBase FindCell(this ExcelWorksheet sheet, string text)
        {
            return sheet.FindCells(text).FirstOrDefault();
        }
        public static List<ExcelRangeBase> FindCellsinEntireBook(this ExcelWorkbook book, string text)
        {
            List<ExcelRangeBase> List = new List<ExcelRangeBase>();
            foreach (var sheet in book.Worksheets)
            {
                var cells = sheet.FindCells(text);
                if (cells != null)
                {
                    List.AddRange(cells);
                }
            }
            return List;
        }
        public static void CopyHeaders(this ExcelWorksheet targetSheet, ExcelWorksheet fromSheet)
        {
            targetSheet.HeaderFooter.OddFooter.LeftAlignedText = fromSheet.HeaderFooter.OddFooter.LeftAlignedText;
            targetSheet.HeaderFooter.OddFooter.RightAlignedText = fromSheet.HeaderFooter.OddFooter.RightAlignedText;
            targetSheet.HeaderFooter.OddHeader.LeftAlignedText = fromSheet.HeaderFooter.OddHeader.LeftAlignedText;
            targetSheet.HeaderFooter.OddHeader.RightAlignedText = fromSheet.HeaderFooter.OddHeader.RightAlignedText;
        }
        public static void AutoFitRows(this ExcelWorksheet sheet)
        {

            for (int row = 1; row <= sheet.Dimension.End.Row; row++)
            {
                double coeff = 1;
                float MaxFontSize = 8;
                for (int col = 1; col <= sheet.Dimension.End.Column; col++)
                {
                    try
                    {


                        sheet.Cells[row, col].Style.WrapText = false;
                        var column = sheet.Column(col);
                        var oldWidth = column.Width;
                        sheet.Cells[row, col].AutoFitColumns();
                        var newWidth = column.Width;
                        column.Width = oldWidth;
                        double newCoeff = newWidth / oldWidth;
                        if (newCoeff != 1)
                        {
                            sheet.Cells[row, col].Style.WrapText = true;
                        }
                        if (coeff < newCoeff)
                        {
                            coeff = newCoeff;
                            var MaxFontSizeNew = sheet.Cells[row, col].Style.Font.Size;
                            MaxFontSize = MaxFontSizeNew;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                sheet.Row(row).Height = (MaxFontSize * 2.2 * coeff);
            }
        }
    }

}
