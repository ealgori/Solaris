using DbModels.Models.Pors;
using EpplusInteract;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelParser.Extentions;
using CommonFunctions.Extentions;
namespace ExcelParser.ExcelParser
{
    public class CreateTOGR
    {
        //private static  string TemplatePath = @"\\RU00112284\SolarisTemplates\POR.xlsx";
        //private static string TemplatePath = @"\\RU00112284\p\OrderTemplates\GRTemplates\GRTemplate.xlsx";

        public static byte[] CreateGRFile(List<PORTOItem> items, string poNumber, string templatePath)
        {

            if (string.IsNullOrEmpty(templatePath))
            {
                return null;
            }
            EpplusService service = new EpplusService(new FileInfo(templatePath));
          
               


               

                //foreach (var item in items)
                //{
                //    item.Description = string.Format("{0} ({1})", item.Description, item.Description.CUnidecode());


                //}

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("PO", poNumber);
                var dataTable = items.ToDataTable(typeof(PORTOItem));
                dataTable.Columns.Remove("POR");
                dataTable.Columns.Remove("Id");
                dataTable.Columns.Remove("PriceListRevisionItem");
                dataTable.Columns.Remove("IsCustom");
                //dataTable.Columns.Remove("Coeff");

                service.ReplaceDataInBook(dict);

                service.InsertTableToPatternCellInWorkBook("Table", dataTable, new EpplusService.InsertTableParams()
                {
                    PrintHeaders = true
                    ,
                    StyledHeaders = true,
                    TableStyle = TableStyles.Medium8,
                    ShowRowStripes = true
                   ,
                    EmptyRowAfterHeaders = true
                });
                var stream = new MemoryStream();
                service.app.SaveAs(stream);
                return CommonFunctions.StaticHelpers.ReadToEnd(stream);
            }
        }
}
