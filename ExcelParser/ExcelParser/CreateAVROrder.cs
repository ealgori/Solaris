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
using DbModels.DomainModels.SAT;

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
        public static byte[] CreateOrderFile(List<SatMusItem> items, string number)
        {
            if (items == null || items.Count == 0)
                return null;
            EpplusService service = new EpplusService(new FileInfo(TemplatePath));
            using (Context context = new Context())
            {

                var ndsCoeff = 0.18M;
                var orderTable = new List<OrderRowModel>();
                var actTable = new List<ActRowModel>();
                var explTable = new List<ExpRow>();
                int count = 0;
                var testItem = items.FirstOrDefault();
                string region = string.Empty;
                var shAvr = context.ShAVRs.FirstOrDefault(a => a.AVRId == testItem.AVRId);
                if (shAvr == null)
                    return null;
                var netwRow = context.PORNetworks.FirstOrDefault(p => p.City == shAvr.Subregion);
                if (netwRow != null)
                    region = netwRow.RegionRus;
                foreach (var item in items)
                {
                    count++;

                    var orderRow = new OrderRowModel();
                    var actRow = new ActRowModel();
                    var explRow = new ExpRow();

                    string siteAddress = string.Empty;
                    string siteName = string.Empty;
                    string siteId = string.Empty;

                    if (item.AvrItemId.HasValue)
                    {
                        var shitem = context.ShAVRItems.FirstOrDefault(i=>i.AVRItemId== item.AvrItemId);
                        if (shitem != null)
                        {
                            var shSite = context.ShSITEs.FirstOrDefault(s => s.Site == shitem.SiteId);
                            if (shSite != null)
                            {
                                siteAddress = shSite.Address;
                                siteName = shSite.SiteName;
                                siteId = shSite.Site;

                            }
                            if (shitem.StartDate.HasValue && shitem.EndDate.HasValue)
                            {
                                orderRow.Period = string.Format("c {0} по {1}"
                               , shitem.StartDate.Value.ToString("dd.MM.yyyy")
                               , shitem.EndDate.Value.ToString("dd.MM.yyyy")
                               );
                            }

                        }
                    }

                  

                    orderRow.Id = count;
                    orderRow.Region = region;
                    orderRow.Address = siteAddress;
                   
                    orderRow.UseCoeff = item.UseCoeff;
                    orderRow.WorkReason = item.WorkReason;
                    orderRow.Quantity = item.Quantity.FinanceRound();
                    orderRow.Period = item.NoteVC;
                    if (item.CustomPos)
                    {
                        orderRow.Price = (item.Price* item.Quantity).FinanceRound();
                        actRow.Price = (item.Price).FinanceRound();

                        orderRow.Description = item.Description;
                        actRow.Description = item.Description;

                    }
                    else
                    {

                        if (item.PriceListRevisionItem != null)
                        {
                            var price = (item.PriceListRevisionItem.Price);
                            if (item.UseCoeff)
                            {
                                orderRow.Price = (price * _coeff * item.Quantity ).FinanceRound();
                                actRow.Price = (price * _coeff).FinanceRound();

                            }
                            else
                            {
                                orderRow.Price = (price * item.Quantity).FinanceRound();
                                actRow.Price = (price).FinanceRound();
                            }
                        }

                        orderRow.Description = item.PriceListRevisionItem.Name;
                        actRow.Description = item.PriceListRevisionItem.Name;
                    }
                   
                    orderTable.Add(orderRow);
                  


                    actRow.Id = count;
                   
                   
                    
                  

                    // 
                    actRow.PriceWNDS = ((actRow.Price ?? 0) + (actRow.Price ?? 0) * ndsCoeff).FinanceRound().ToString("F");
                    actRow.NDS = (actRow.Price ?? 0 * ndsCoeff).ToString("F");
                    actTable.Add(actRow);

                    explRow.Id = count;
                    // explRow.Empty1 = "#merger(1,2)";
                    if(!string.IsNullOrEmpty(siteId))
                    {
                        explRow.BigAddress = string.Format("БС(№: {0}) \"{1}\" {2} ", siteId, siteName, actRow.Description);
                        orderRow.SiteName = string.Format("БС(№: {0}) \"{1}\"", siteId, siteName);
                        actRow.Address = siteAddress;
                    }
                    
                    explRow.PriceWNDS = actRow.PriceWNDS;
                    explTable.Add(explRow);


                }

                var orderDT = orderTable.ToDataTable();
                orderDT.Columns.Remove("UseCoeff");
                service.InsertTableToPatternCellInWorkBook("OrderTable", orderDT, new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                service.InsertTableToPatternCellInWorkBook("ActTable", actTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                service.InsertTableToPatternCellInWorkBook("ExplTable", explTable.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = false, StyledHeaders = false, CopyFirstRowStyle = true, MinSeparatedRows = 0 });
                Dictionary<string, string> dict = new Dictionary<string, string>();

                //в ордер тэйбл цены уже умножены на количество и коэфф, если надо.
                decimal total = orderTable.Where(i=>!i.UseCoeff).Sum(i => (i.Price??0));
                decimal total14 = orderTable.Where(i => i.UseCoeff).Sum(i => (i.Price ?? 0));
                
                decimal totalFR = total.FinanceRound();
                decimal total14FR = total14.FinanceRound();
                //decimal totalWCoef = (total * 1.4M);
                //decimal totalWCoefFR = totalWCoef.FinanceRound();
                decimal nds = ((total+total14) * ndsCoeff);
                decimal ndsFR = nds.FinanceRound();
                decimal totalWNDS = (total+total14 + nds);
                decimal totalWNDSFR = totalWNDS.FinanceRound();
                string totalWNDSp = CommonFunctions.InWords.Валюта.Рубли.Пропись(totalWNDSFR, CommonFunctions.InWords.Заглавные.Первая);
                string ndsp = CommonFunctions.InWords.Валюта.Рубли.Пропись(ndsFR, CommonFunctions.InWords.Заглавные.Первая);

                string orderTotalTextFormat = "3. Общая стоимость Дополнительных работ составляет: {0} {1}., в том числе {2} {3}.";
                string actTotalTextFormat = "2. В соответствием с Заказом №: {0} Заказчик выплачивает Исполнителю стоимость работ в сумме {1} {2}, в том числе НДС {3} {4}  согласно п.5.1 Дополнения 3.1 к Приложению 3 Соглашения.";
                string explTotalTextFormat = "Всего в сумме {0} {1}, в том числе НДС  {2}  {3}.";

                string orderTotalText = string.Format(orderTotalTextFormat, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                string actTotalText = string.Format(actTotalTextFormat, number, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                string explTotalText = string.Format(explTotalTextFormat, totalWNDSFR, totalWNDSp, ndsFR, ndsp);
                var rowsToRemove = new List<string>();
                // работы только в рабочее время
                if (!items.Any(i => i.UseCoeff))
                {
                    // удалить лишнюю строчку 
                    dict.Add("Label", _labelWOCoeff);
                    rowsToRemove.Add("Label1.4");

                }
                else
                {
                    dict.Add("Label1.4", _labelWithCoeff);
                    // смешаные работы
                    if (!items.All(i => i.UseCoeff))
                    {
                        dict.Add("Label", _labelWOCoeff);
                    }
                    // только ночные
                    else
                    {
                        rowsToRemove.Add("Label");
                    }
                }
                service.RemoveRowsInWorkBook(rowsToRemove);

                dict.Add("OrderTotalText", orderTotalText);
                dict.Add("ActTotalText", actTotalText);
                dict.Add("ExplTotalText", explTotalText);
                dict.Add("TotalWONDS", totalFR.ToString());
                dict.Add("Total", totalFR.ToString());
                dict.Add("Total1.4", total14FR.ToString());
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
            public bool UseCoeff { get; set; }


        
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
