using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EpplusInteract;
using DbModels.DataContext;
using System.IO;
using DbModels.DataContext.Repositories;
using CommonFunctions.Extentions;
using CommonFunctions;
using DbModels.DomainModels.ShClone;

namespace ExcelParser.EpplusInteract
{
   public static  class CreateAVROrder
    {
      private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\AVROrderTemplate.xlsm";
        private static readonly string _labelWOCoeff = "Стоимость Дополнительных Работ без учета коэффициента 1,4";
        private static readonly string _labelWithCoeff = "Стоимость Дополнительных Работ с учетом коэффициента 1,4";
        private  static readonly decimal _coeff = 1.4M;



        //private static readonly string TemplatePath = @"C:\Temp\test.xlsm";
        //private static readonly string TemplatePath = @"\\RU00112284\SolarisTemplates\AKT.xlsm";
        //private static readonly string SavePathTest = @"C:\Temp\AVROrderTest.xlsm";
        public static byte[] CreateOrderFile(List<ShAVRItem> items, string number)
        {
            if (items == null || items.Count == 0)
                return null;
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {


                var orderTable = new List<OrderRowModel>();
                var actTable = new List<ActRowModel>();
                var explTable = new List<ExpRow>();
                int count = 0;
                var testItem = items.FirstOrDefault();
                string region = string.Empty;
                var shAvr = context.ShAVRs.FirstOrDefault(a => a.AVRId == testItem.AVRS.AVRId);
                if (shAvr == null)
                    return null;
                var netwRow = context.PORNetworks.FirstOrDefault(p => p.City == shAvr.Subregion);
                if (netwRow != null)
                    region = netwRow.RegionRus;
                foreach (var item in items)
                {
                    count++;

                    string siteAddress = string.Empty;
                    string siteName = string.Empty;
                    string siteId = string.Empty;
                    var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == item.SiteId);
                    if (shSite != null)
                    {
                        siteAddress = shSite.Address;
                        siteName = shSite.SiteName;
                        siteId = shSite.Site;

                    }

                    var orderRow = new OrderRowModel();
                    var actRow = new ActRowModel();
                    var explRow = new ExpRow();

                    orderRow.Id = count;
                    orderRow.Region = region;
                    orderRow.Address = siteAddress;
                    orderRow.SiteName = string.Format("БС(№: {0}) \"{1}\"", siteId, siteName);
                    orderRow.Description = item.VCDescription;
                    orderRow.WorkReason = item.WorkReason;
                    orderRow.Quantity = item.VCQuantity ?? 0;
                    orderRow.Note = item.NoteVC;
                    if(item.VCUseCoeff)
                    {
                        orderRow.Price = (item.VCPrice.HasValue ? item.VCPrice.Value / _coeff : 0) * (orderRow.Quantity);
                       
                    } 
                    else
                    {
                        orderRow.Price = (item.VCPrice ?? 0) * (orderRow.Quantity);
                    }
                   
                    orderRow.Note = item.Note;
                    orderTable.Add(orderRow);
                    if (item.StartDate.HasValue && item.EndDate.HasValue)
                    {
                        orderRow.Period = string.Format("c {0} по {1}"
                       , item.StartDate.Value.ToString("dd.MM.yyyy")
                       , item.EndDate.Value.ToString("dd.MM.yyyy")
                       );
                    }


                    actRow.Id = count;
                    actRow.Description = item.VCDescription;
                    actRow.Address = siteAddress;
                    if (item.VCUseCoeff)
                    {
                        actRow.Price = item.VCPrice/_coeff;
                    }
                    else
                    {
                        actRow.Price = item.VCPrice;
                    }


                    actRow.PriceWNDS = ((item.VCPrice ?? 0) + (item.VCPrice ?? 0) * 0.18M).ToString("F");
                    actRow.NDS = (item.VCPrice ?? 0 * 0.18M).ToString("F");
                    actTable.Add(actRow);

                    explRow.Id = count;
                   // explRow.Empty1 = "#merger(1,2)";
                    explRow.BigAddress = string.Format("БС(№: {0}) \"{1}\" {2} ", siteId, siteName, item.VCDescription);
                    explRow.PriceWNDS = (item.VCPrice ?? 0 + item.VCPrice ?? 0 * 0.18M).ToString("F");
                    explTable.Add(explRow);


                }


                service.InsertTableToPatternCellInWorkBook("OrderTable", orderTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                service.InsertTableToPatternCellInWorkBook("ActTable", actTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                service.InsertTableToPatternCellInWorkBook("ExplTable", explTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                Dictionary<string, string> dict = new Dictionary<string, string>();

                decimal total = items.Sum(i => (i.VCPrice ?? 0) * (i.VCQuantity ?? 0));
                decimal totalFR = total.FinanceRound();
                //decimal totalWCoef = (total * 1.4M);
                //decimal totalWCoefFR = totalWCoef.FinanceRound();
                decimal nds = (total * 0.18M);
                decimal ndsFR = nds.FinanceRound();
                decimal totalWNDS = (total + nds);
                decimal totalWNDSFR = totalWNDS.FinanceRound();
                string totalWNDSp = CommonFunctions.InWords.Валюта.Рубли.Пропись(totalWNDSFR, CommonFunctions.InWords.Заглавные.Первая);
                string ndsp = CommonFunctions.InWords.Валюта.Рубли.Пропись(ndsFR, CommonFunctions.InWords.Заглавные.Первая);

                string orderTotalTextFormat = "3. Общая стоимость Дополнительных работ составляет: {0} {1}., в том числе {2} {3}.";
                string actTotalTextFormat = "2. В соответствием с Заказом №: {0} Заказчик выплачивает Исполнителю стоимость работ в сумме {1} {2}, в том числе НДС {3} {4}  согласно п.5.1 Дополнения 3.1 к Приложению 3 Соглашения.";
                string explTotalTextFormat = "Всего в сумме {0} {1}, в том числе НДС  {2}  {3}.";

                string orderTotalText = string.Format(orderTotalTextFormat, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                string actTotalText = string.Format(actTotalTextFormat, number, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                string explTotalText = string.Format(explTotalTextFormat, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                if(items.Any(i=>i.VCUseCoeff))
                {
                    dict.Add("Label", _labelWithCoeff);
                }
                else
                {
                    dict.Add("Label", _labelWOCoeff);
                }
                dict.Add("OrderTotalText", orderTotalText);
                dict.Add("ActTotalText", actTotalText);
                dict.Add("ExplTotalText", explTotalText);
                dict.Add("TotalWONDS", totalFR.ToString());
                dict.Add("Total", totalFR.ToString());
                //dict.Add("TotalWCoef", totalWCoefFR.ToString());
                dict.Add("NDS", ndsFR.ToString());
                dict.Add("TotalWNDS", totalWNDSFR.ToString());
                dict.Add("OrderNumber", number);
                service.ReplaceDataInBook(dict, true);
                service.CellsMerger();

                var stream = new MemoryStream();
                //service.app.SaveAs(new FileInfo(SavePathTest));
                service.app.SaveAs(stream);
                return StaticHelpers.ReadToEnd(stream);

            }
            return null;
           
       }

        private class OrderRowModel 
        {
            public int Id { get; set; }
            public string Region { get; set; }
            public string Address { get; set; }
            public string SiteName { get; set; }
            public string Description { get; set; }
            public string WorkReason { get; set; }
            public decimal? Quantity { get; set; }
            public decimal? Price { get; set; }
            public string Period { get; set; }
            public string Note { get; set; }


        
        }

       private class ActRowModel
       {
           public int Id { get; set; }
           public string Description { get; set; }
           public string Address { get; set; }
           public decimal? Price { get; set; }
           public string NDS { get; set; }
           public string PriceWNDS { get; set; }


       }

       private class ExpRow
       {
           public int Id { get; set; }
           public string BigAddress { get; set; }
           public string PriceWNDS { get; set; }
       }

    }
}
