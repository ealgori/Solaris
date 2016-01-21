using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using ExcelParser.Extentions;

using DbModels;
using CommonFunctions.Extentions;
using OfficeOpenXml.Table;
using DbModels.DataContext;
using DbModels.Models.Pors;
using DbModels.DomainModels.Solaris.Pors;
using CommonFunctions;
using EpplusInteract;



namespace ExcelParser.EpplusInteract
{
    public class CreatePor
    {

        //private static  string TemplatePath = @"\\RU00112284\SolarisTemplates\POR.xlsx";
        private static string TemplatePath = @"\\RU00112284\OrderTemplates\PORTemplates\POR-POV2-Template.xlsx";
        
        public static byte[] CreatePorFile(int porId, bool test = false)
        {
            if(test)
                TemplatePath = @"\\RU00112284\p\OrderTemplates\PORTemplates\POR-POV2-Template.xlsx";
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {
                var por = context.PORs.Find(porId);
                
              //  var por1 = context.PORs.Include("PORNetwork").FirstOrDefault(p => p.Id == porId);
                var pitems = por.PorItems.ToList();
                foreach (var item in pitems)
                {
                     item.Description = string.Format("{0} ({1})",item.Description, item.Description.CUnidecode());
                }
                var dataTable = pitems.ToDataTable(typeof(PORItem));
                dataTable.Columns.Remove("POR");
                dataTable.Columns.Remove("Id");
                dataTable.Columns.Remove("PriceListRevisionItem");
                dataTable.Columns.Remove("IsCustom");
                dataTable.Columns.Remove("Coeff");
                Dictionary<string,string> dict = new Dictionary<string,string>();
                string porIds = string.Empty;
                if (por is AVRPOR)
                {
                    porIds = string.Format("SH AVR Id:{0}",((AVRPOR)por).AVRId).ToString();
                    dict.Add("PorId", porIds );

                }
                dict.Add("VendorNameRus", por.SubContractorName);
                dict.Add("SubContractorName", por.SubContractorName);
               

                dict.Add("VendorNumber", por.SubContractorSAPNumber);
                dict.Add("SAPNumber", por.SubContractorSAPNumber);

                dict.Add("VendorAddress", por.SubContractorAddress);
                dict.Add("SubContractorAddress", por.SubContractorAddress);

                dict.Add("PriceListNumbers",por.PriceListNumbers);
                dict.Add("VendorContractNo", por.PriceListNumbers);

                dict.Add("StartDate",por.WorkStart.ToShortDateString());
                dict.Add("WorkEnd", por.WorkEnd.ToShortDateString());
                dict.Add("EndDate", por.WorkEnd.ToShortDateString());

                dict.Add("today", DateTime.Now.ToShortDateString());

                 dict.Add("WBS", "");
                
                dict.Add("RequestorName", "Николай Евстафьев");
                dict.Add("RequestorSignum", "enikevs");
               
                if (por is AVRPOR)
                {
                    dict.Add("Network", por.Network);
                }
                //{
                //    if(((AVRPOR)por).AVRId[0]=='2')
                //    {
                //        dict.Add("Network", por.Network==null?"": por.PORNetwork.Network2014.ToString());
                //    }
                //    else
                //    {
                //         dict.Add("Network", por.Network==null?"": por.PORNetwork.Network.ToString());
                //    }
                //}
                //else
                //{
                //    if (por.WorkStart.Year==2014)
                //    {
                //         dict.Add("Network", por.PORNetwork==null?"": por.PORNetwork.Network2014.ToString());
                //    }
                //    else
                //    {
                //          dict.Add("Network", por.PORNetwork==null?"": por.PORNetwork.Network.ToString());
                //    }
                //}
                
                
                dict.Add("Activity", por.Activity);
                var porNetwork = context.PORNetworks.FirstOrDefault(r => r.City == por.SubRegion);
                if(porNetwork!=null)
                    dict.Add("Region", porNetwork.Region);
                dict.Add("POType", por.POType);
                dict.Add("Signum", por.UserName);

                string note = string.Format("POType:{0}; PORId:{1}; PurchReqNo:{2}; PRItm: {3}; Creator:{4}", 
                    string.IsNullOrEmpty(por.POType)?"_____":por.POType,
                    porIds ,
                    "_____",
                    "_____",
                    por.UserName
                    );
                dict.Add("Notes", note);

                var sites = string.Join(", ", por.PorItems.Select(p => p.Site).Where(p =>!string.IsNullOrWhiteSpace(p)).ToArray());
                var fixes = string.Join(", ", por.PorItems.Select(p => p.FIX).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray());
                var fols = string.Join(", ", por.PorItems.Select(p => p.FOL).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray());
                var all = string.Join(", ", new List<string>() { sites, fixes, fols }.Where(t=>!string.IsNullOrWhiteSpace(t)));
                dict.Add("Site", all);
                dict.Add("Address", "");
                var items = context.PORItems.Where(pi => pi.POR.Id == porId);
                var summ = 0M;
                foreach (var item in items)
                {
                    summ = summ + item.NetQty * item.Price;
                }
                var total = items.Sum(pit=>pit.NetQty*pit.Price);
                dict.Add("Total", total.ToString("0.00"));
                service.ReplaceDataInBook(dict);
                
                service.InsertTableToPatternCellInWorkBook( "Table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders=true, StyledHeaders=true ,TableStyle = TableStyles.Medium7, ShowRowStripes=true, EmptyRowAfterHeaders=true });
                service.InsertTableToPatternCellInWorkBook("table", dataTable, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Medium7, ShowRowStripes = true, EmptyRowAfterHeaders = true });
                var stream = new MemoryStream();

                service.app.SaveAs(stream);
                return StaticHelpers.ReadToEnd(stream);
            }
        }
   }
}
