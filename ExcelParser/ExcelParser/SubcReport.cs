using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DbModels.DataContext;
using DbModels.Models.Pors;
using System.IO;
using ExcelParser.Extentions;

using OfficeOpenXml.Table;
using OfficeOpenXml;
using CommonFunctions;
using System.Data;
using NPOI.SS.UserModel;
namespace ExcelParser
{
    public class SubcRep
    {
        private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\ExportPLForSubc.xls";
       // List<SubcReportRow> list = data.context.Database.SqlQuery<SubcReportRow>(data.SPName).ToList();
        public static byte[] SubcReport(List<SubcReportRow> data)
        {
            var wb = NpoiInteract.ConnectExlFile(TemplatePath);
            if (wb != null)
            {
                //var dataTable = data.ToDataTable(typeof(SubcReportRow));
                //dataTable.SetColumnsOrder(new string[] { "Id", "Site", "Address", "Code", "Name", "Quantity", "Price" });

                //  NpoiInteract.InsertDataTableToNamedRange(dataTable,"SowStart",wb);
                NpoiInteract.SetNamedRangeCellValue("SubContractor", data.FirstOrDefault().SubName, wb);
                NpoiInteract.SetNamedRangeCellValue("shSubContractor", data.FirstOrDefault().ShName, wb);
                NpoiInteract.SetNamedRangeCellValue("SubContractorSAPNumber", data.FirstOrDefault().SAPNumber, wb);

                //EpplusService service = new EpplusService(new FileInfo(TemplatePath));
                //using (Context context = new Context())
                //{


                //    //  var por1 = context.PORs.Include("PORNetwork").FirstOrDefault(p => p.Id == porId);
                //    ExcelNamedRange RowName = service.GetRange("SowName");
                //    ExcelNamedRange RangeStart = service.GetRange("SowStart");

                //    int CurrentRow = RangeStart.Start.Row ;
                //    int CurrentColumn = RangeStart.Start.Column;
                int CurID = 1;
                //    service.GetSheet("Price").InsertRow(RangeStart.Start.Row, data.Count, RangeStart.Start.Row);
                //    var dict = new Dictionary<string,string>();
                var startCell = NpoiInteract.GetCellByNamedRange("SowStart", wb);
                int CurrentRow = startCell.RowIndex;

                foreach (var item in data)
                {
                    //for (int i = 1; i < 10; i++)
                    //{
                    //    service.GetSheet("Price").Cells[RowName.Start.Row + data.Count, i].Copy(service.GetSheet("Price").Cells[CurrentRow, i]);
                    //}
                    IRow row = null;
                    if (CurID != 1)
                    {
                        row = NpoiInteract.GetCopyRow(startCell.Sheet, CurrentRow++, CurrentRow);
                    }
                    else
                    {
                        row = startCell.Sheet.GetRow(startCell.RowIndex);
                    }
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex), CurID++.ToString());
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 1), item.Site);
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 2), item.Address);
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 3), item.Code);
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 5), item.Name);
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 6), item.Unit);
                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 8), item.Price);

                    String myFormula = "(" + NpoiInteract.getColumnName(startCell.ColumnIndex + 7) + (row.RowNum+1) + "*" +
                                        NpoiInteract.getColumnName(startCell.ColumnIndex + 8) + (row.RowNum+1) + ")";
                    row.GetCell(startCell.ColumnIndex + 9).SetCellFormula(myFormula);

                    NpoiInteract.SetCellValue(row.GetCell(startCell.ColumnIndex + 10), item.Id.ToString());


                }
                //    dict.Add("Subcontractor", data.FirstOrDefault().SubName);
                //    service.ReplaceDataInBook(dict);
                //    service.GetSheet("Price").DeleteRow(CurrentRow, 1);
                //    var str1 = "=SUM(I15:" + "I" + CurrentRow.ToString()+ ")";
                //    service.GetSheet("Price").Cells[CurrentRow + 1, CurrentColumn + 8].Formula = str1;
                //    //var dataTable = data.ToDataTable(typeof(SubcReportRow));
                //    //dataTable.Columns.Remove("POR");
                //    //dataTable.Columns.Remove("Id");
                //    //dataTable.Columns.Remove("PriceListRevisionItem");
                //    //dataTable.Columns.Remove("IsCustom");
                //    //Dictionary<string, string> dict = new Dictionary<string, string>();
                //    //dict.Add("SubContractorName", por.SubContractorName);
                //    //dict.Add("SAPNumber", por.SubContractorSAPNumber);
                //    //dict.Add("SubContractorAddress", por.SubContractorAddress);
                //    //dict.Add("PriceListNumbers", por.PriceListNumbers);
                //    //dict.Add("StartDate", por.WorkStart.ToShortDateString());
                //    //dict.Add("EndDate", por.WorkEnd.ToShortDateString());
                //    //dict.Add("today", DateTime.Now.ToShortDateString());
                //    //dict.Add("Network", por.PORNetwork == null ? "" : por.PORNetwork.Network.ToString());
                //    //dict.Add("Activity", por.Activity);
                //    //dict.Add("Region", por.PORNetwork.Region);
                //    //dict.Add("POType", por.POType);

                //    //service.ReplaceDataInBook(dict);

                //   // service.InsertTableToPatternCellInWorkBook("table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Medium7, ShowRowStripes = true, EmptyRowAfterHeaders = true });
                //    var stream = new MemoryStream();

                //    service.app.SaveAs(stream);
                //    return StaticHelpers.ReadToEnd(stream);
                return NpoiInteract.GetWorkBookBytes(wb);
            }
            return null;
            }
        }
    public class SubcReportRow
    {
        public string SubName { get; set; }
        public string Site { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShName { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string SAPNumber { get; set; }
        public int Id { get; set; }

    }

    public static class DataTableExtensions
    {
        public static void SetColumnsOrder(this DataTable table, params String[] columnNames)
        {
            for (int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++)
            {
                table.Columns[columnNames[columnIndex]].SetOrdinal(columnIndex);
            }
        }
    }
      
    
}
