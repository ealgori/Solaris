using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpplusInteract;
using DbModels.DataContext;
using System.IO;
using CommonFunctions.Extentions;
using DbModels.DataContext.Repositories;
using CommonFunctions;
using DbModels.Models.Pors;
using OfficeOpenXml.Table;


namespace ExcelParser.EpplusInteract
{
   public class CreateTOPOR
    {
        //private static string TemplatePath = @"\\RU00112284\SolarisTemplates\POR.xlsx";
        private static string TemplatePath = @"\\RU00112284\p\OrderTemplates\PORTemplates\POR-POV2-Template.xlsx";

       /// <summary>
        /// Группировка сервисов убрана 05.08.2014 из за письма FW: ТО АУГПТ Нити-Прогресс от Подоруева.
       /// </summary>
       /// <param name="satToId"></param>
       /// <returns></returns>
 
       public static byte[] CreatePorFile(int satToId, bool test = false)
        {

            if (test)
                TemplatePath = @"\\RU00112284\p\OrderTemplates\PORTemplates\POR-POV2-Template.xlsx";
           EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {
            
                
               TORepository repository = new TORepository(context);
               var _satTo = repository.GetLastSATTOList().FirstOrDefault(t=>t.Id== satToId);

               if (_satTo != null)
               {
                   var satTo = context.SATTOs.Find(satToId);

                   List<PORTOItem> attachmentTable = null;
                   try
                   {
                       attachmentTable = repository.GetSATTOPORItemModelsGroupped(satTo.Id);
                       if (attachmentTable == null)
                       {
                           return null;
                       }
                   }
                   catch(Exception exc)
                   {
                       return null;
                   }

               
                 

                   Dictionary<string, string> dict = new Dictionary<string, string>();
                   dict.Add("Activity", satTo.Activity);
                   dict.Add("PorId", string.Format("SH TO Id:{0}",satTo.TO));
                   
                   dict.Add("SubContractorName", satTo.SubContractor);
                   dict.Add("VendorNameRus", satTo.SubContractor);

                   dict.Add("VendorNumber", satTo.SubContractorSapNumber);
                   dict.Add("SAPNumber", satTo.SubContractorSapNumber);

                   
                   dict.Add("VendorAddress", satTo.SubContractorAddress.CUnidecode());
                   dict.Add("SubContractorAddress", satTo.SubContractorAddress.CUnidecode());
                

                   dict.Add("PriceListNumbers", satTo.ProceListNumbers.CUnidecode());
                   dict.Add("VendorContractNo", satTo.ProceListNumbers.CUnidecode());
                   var startDate = attachmentTable.Min(a => a.Plandate);
                   var endDate = attachmentTable.Max(a => a.Plandate);
                   if (startDate.Value.Date == endDate.Value.Date)
                       startDate = startDate.Value.AddDays(-1);
                   dict.Add("StartDate", startDate.Value.ToString("dd.MM.yyyy"));

                   dict.Add("RequestorName", "Николай Евстафьев");
                   dict.Add("RequestorSignum", "enikevs");

                   dict.Add("WBS", "");

                   dict.Add("EndDate", endDate.Value.ToString("dd.MM.yyyy"));
                   dict.Add("WorkEnd", endDate.Value.ToString("dd.MM.yyyy"));

                   dict.Add("today", DateTime.Now.ToString("dd.MM.yyyy"));
                   //if (por is AVRPOR)
                   //{
                   //    if (((AVRPOR)por).AVRId[0] == '2')
                   //    {
                   //        dict.Add("Network", por.PORNetwork == null ? "" : por.PORNetwork.Network2014.ToString());
                   //    }
                   //    else
                   //    {
                   //        dict.Add("Network", por.PORNetwork == null ? "" : por.PORNetwork.Network.ToString());
                   //    }
                   //}
                   //else
                   //{
                   //    if (por.WorkStart.Year == 2014)
                   //    {
                   //        dict.Add("Network", por.PORNetwork == null ? "" : por.PORNetwork.Network2014.ToString());
                   //    }
                   //    else
                   //    {
                   //        dict.Add("Network", por.PORNetwork == null ? "" : por.PORNetwork.Network.ToString());
                   //    }
                   //}

                    dict.Add("Network", satTo.Network );

                   
                   dict.Add("Region", satTo.Region);
                   dict.Add("POType", satTo.ToType);
                   dict.Add("Signum", satTo.CreateUserName);
                   var networkNum = int.Parse(satTo.Network);
                   var pr = context.PurchaseRequests.FirstOrDefault(p => p.Activity.Activity == satTo.Activity && p.Network.Network2014 == networkNum);
                   if(pr!=null)
                   {
                    dict.Add("PurchaseRequest", pr.PurchReqNo);
                    dict.Add("PRItem", pr.PRItem);
                   }
                  
                   var siteList = satTo.SATTOItems.Select(s=>s.Site).ToList();
                   var sites = string.Join(", ", siteList );
                   //var fixes = string.Join(", ", por.PorItems.Select(p => p.FIX).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray());
                   //var fols = string.Join(", ", por.PorItems.Select(p => p.FOL).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray());
                   // var all = string.Join(", ", new List<string>() { sites, fixes, fols }.Where(t => !string.IsNullOrWhiteSpace(t)));
                   var all = string.Join(", ", new List<string>() { sites, }.Where(t => !string.IsNullOrWhiteSpace(t)));
                   List<string> addresses = new List<string>();
                   foreach (var site in siteList)
                   {
                       var shSite = context.ShSITEs.FirstOrDefault(s => s.Site==site);
                       if (shSite != null && !string.IsNullOrEmpty(shSite.Address))
                           addresses.Add(shSite.Address);

                   }
                   var addressesString = string.Join(", ", addresses.Distinct());
                   dict.Add("Site", all);
                   dict.Add("Address", addressesString);

                   //var items = context.PORItems.Where(pi => pi.POR.Id == porId);
                   //var summ = 0M;
                   //foreach (var item in items)
                   //{
                   //    summ = summ + item.NetQty * item.Price;
                   //}
                   //var total = items.Sum(pit => pit.NetQty * pit.Price);
                   dict.Add("Total", satTo.Total.ToString());


                   string note = string.Format("POType:{0}; PORId:{1}; PurchReqNo:{2}; PRItm: {3}; Creator:{4}",
                   string.IsNullOrEmpty(satTo.ToType) ? "_____" : satTo.ToType,
                   string.Format("SH TO Id:{0}", satTo.TO),
                   "_____",
                   "_____",
                   satTo.CreateUserName
                   );
                   dict.Add("Notes", note);

                   foreach (var attach in attachmentTable)
                   {
                       attach.Description = string.Format("{0} ({1})",attach.Description, attach.Description.CUnidecode());
                   }
                   var dataTable = attachmentTable.ToList().ToDataTable();
                   dataTable.Columns.Remove("POR");
                   dataTable.Columns.Remove("Id");
                   dataTable.Columns.Remove("PriceListRevisionItem");
                   dataTable.Columns.Remove("IsCustom");

                 
                   service.ReplaceDataInBook(dict);
                   service.InsertTableToPatternCellInWorkBook("table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Medium7, ShowRowStripes = true, EmptyRowAfterHeaders = true });
                   service.InsertTableToPatternCellInWorkBook("Table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Medium7, ShowRowStripes = true, EmptyRowAfterHeaders = true });
                   var stream = new MemoryStream();

                   service.app.SaveAs(stream);
                   return StaticHelpers.ReadToEnd(stream);
               }
               else
                   return null;
            }
        }
    }
}
