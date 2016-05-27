using DbModels.DataContext;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFunctions.Extentions;
using DbModels.DomainModels.Solaris.Pors;
using DbModels.DataContext.Repositories;
using DbModels.Models.Pors;
using ExcelParser.Extentions;
using CommonFunctions;
using OfficeOpenXml.Table;

namespace ExcelParser.ExcelParser
{
    public static class CreateGR
    {
        //private static  string TemplatePath = @"\\RU00112284\SolarisTemplates\POR.xlsx";
        private static string TemplatePath = @"\\RU00112284\p\OrderTemplates\GRTemplates\GRTemplate.xlsx";

        public  static byte[] CreateGRFile(int porId, string poNumber, string templatePath)
        {

            if(string.IsNullOrEmpty(templatePath))
            {
                templatePath = TemplatePath;
            }
            EpplusService service = new EpplusService(new FileInfo(templatePath));
            using (Context context = new Context())
            {
                var por = context.AVRPORs.Find(porId);

              
                var pitems = por.PorItems.ToList();

                foreach (var item in pitems)
                {
                    item.Description = string.Format("{0} ({1})", item.Description, item.Description.CUnidecode());


                }

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("PO", poNumber);
                var dataTable = pitems.ToDataTable(typeof(PORItem));
                dataTable.Columns.Remove("POR");
                dataTable.Columns.Remove("Id");
                dataTable.Columns.Remove("PriceListRevisionItem");
                dataTable.Columns.Remove("IsCustom");
                dataTable.Columns.Remove("Coeff");

                service.ReplaceDataInBook(dict);

                service.InsertTableToPatternCellInWorkBook("Table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true
                    ,StyledHeaders = true, TableStyle = TableStyles.Medium8, ShowRowStripes = true
                   ,EmptyRowAfterHeaders = true
                });
                var stream = new MemoryStream();
                service.app.SaveAs(stream);
                return StaticHelpers.ReadToEnd(stream);
            }

        }
    }
}
