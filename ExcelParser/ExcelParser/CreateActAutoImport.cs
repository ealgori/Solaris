using DbModels.DomainModels.SAT;
using EpplusInteract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFunctions.Extentions;

namespace ExcelParser.ExcelParser
{
    /// <summary>
    /// Генерирует файл для последующего автоимпорта. 
    /// </summary>
    public static class CreateActAutoImport
    {
        static public byte[] Create(SATAct act, List<SATActService> satServices, List<SATActMaterial> satMaterials )
        

        
        {
            var template = @"\\RU00112284\Solaris\AutoImportTemplates\ActCreateTemplate.xlsx";
            using(EpplusService service = new EpplusService(template))
            {
                var dict = new Dictionary<string,string>();
                dict.Add("TO", act.TO);
                dict.Add("StartDate", act.StartDate.ToString("dd-MM-yyyy"));
                dict.Add("EndDate", act.EndDate.ToString("dd-MM-yyyy"));

                service.ReplaceDataInBook(dict);
                var servicesDt = satServices.ToDataTable();
                servicesDt.Columns.Remove("Id");
                servicesDt.Columns.Remove("SATAct");
                servicesDt.Columns.Remove("Description");
                servicesDt.Columns.Remove("Unit");
                servicesDt.Columns["ShId"].SetOrdinal(0);
                servicesDt.Columns["FactDate"].SetOrdinal(1);




                service.InsertTableToPatternCellInWorkBook("services", servicesDt, new EpplusService.InsertTableParams() { PrintHeaders = true, DateFormat="dd.MM.yyyy" });
               // материалы отключены, потому что непонятно как их считывать из файла. На новой странице, или как то разделять две таблицы... когда понадобится, тогда и будем разбираться.
                // service.InsertTableToPatternCellInWorkBook("materials", satMaterials.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders=true});
                var bytes = service.GetBytes();
                return bytes;
            }
            return null;
        }
    }
}
