using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DbModels;
using System.Web.Mvc;
using DbModels.DataContext;
using Intranet.Models;
using DbModels.Repository;
using DbModels.DomainModels.Solaris.Pors;
using CommonFunctions.Extentions;
using System.Diagnostics;

namespace Intranet.Service
{
    public static class DataService
    {
        public static List<SelectListItem> ProjectList()
        {
            using (Context context = new Context())
            {
                List<SelectListItem> List = new List<SelectListItem>();
                foreach (var item in context.Projects)
                {
                    List.Add(new SelectListItem()
                    {
                        Text = item.Name,
                        Value = item.Id.ToString(),
                    });
                }
                return List;
            }
        }
        public static string GetLastSHUpdateLog()
        {
            using (Context context = new Context())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;
                context.Configuration.AutoDetectChangesEnabled = false;
                
                var last = context.ShCloneUpdateLogs.Where(l => l.Task.Name == "ShCloneUpdate"&& l.EndTime< DateTime.Now).OrderByDescending(sh => sh.Id).FirstOrDefault();
                DateTime now = DateTime.Now;
                if (last == null || !last.EndTime.HasValue)
                    return "<never>";
                else

                    return last.EndTime.Value.ToString("dd-MM-yyyy HH:mm");

            }
        }

        public static string GetServerTime()
        {
            return DateTime.Now.ToString("dd-MM-yyyy HH:mm");
        }

        public static PorAnalyzerResultModel ComparePorByBasePL(PorAnalyzerModel model, Context context)
        {
            PorAnalyzerResultModel result = new PorAnalyzerResultModel();
            result.CompareReportModelList = new List<CompareReportModel>();
            result.UncomparablePLS = new List<string>();
            result.UnApprovedRevisions = new List<string>();
            result.UncomparableItems = new List<string>();
            //using (Context contex = new Context())
            {
                //if (!model.sourceSubc.HasValue)
                //{
                //    model.sourceSubc = model.subcontractor;

                //}
                PriceListRepository repository = new PriceListRepository(context);
                var activeItems = repository.GetActivePriceListsRevisionItems().ToList();
                //result.SourceSubontractor = context.SubContractors.Find(model.sourceSubc);
                //if (result.SourceSubontractor == null)
                //{
                //    result.Errors.Add(string.Format("Не найден подрядчик с Id:'{0}'", model.sourceSubc));
                //}
                //else
                {
                    //var comparablePl = repository.GetComparablePriceList(model.sourceSubc.Value);
                    //if (comparablePl == null)
                    //{
                    //    result.Errors.Add(string.Format("У подрядчика '{0}' нет опорного прайслиста", result.SourceSubontractor.Name));
                    //}
                    //else
                    {


                        result.Subcontractor = context.SubContractors.Find(model.subcontractor);
                        if (result.Subcontractor == null)
                            result.Errors.Add(string.Format("Не найден подрядчик с Id:'{0}'", model.subcontractor));
                        else
                        {
                            var avrPors = context.PORs.Include("PriceListRevisions").Where(p => p.SubContractor.Id == model.subcontractor).ToList();
                            var toPors = context.SATTOs.Where(p => p.SubContractor == result.Subcontractor.Name).ToList();
                            //var toPors = contex.SATTOs.Include("PriceListRevisions").Where(p => p.SubContractor == subc.Name).ToList();
                            if (model.start.HasValue)
                            {
                                avrPors = avrPors.Where(p => p.PrintDate >= model.start).ToList();
                                toPors = toPors.Where(t => t.CreateUserDate >= model.start).ToList();
                            }
                            if (model.end.HasValue)
                            {
                                model.end = model.end.Value.AddDays(1);
                                avrPors = avrPors.Where(p => p.PrintDate <= model.end).ToList();
                                toPors = toPors.Where(t => t.CreateUserDate <= model.end).ToList();
                            }

                            if (avrPors != null && toPors != null && avrPors.Count() == 0 && toPors.Count() == 0)
                            {
                                result.Errors.Add(string.Format("В системе нет ни одного пора по подрядчику:'{0}'", result.Subcontractor.Name));
                            }
                            else
                            {

                                var maps = context.PriceListMaps.ToList();
                                //var comparamblePlRevision = comparablePl.PriceListRevisions.OrderByDescending(r => r.CreationDate).FirstOrDefault();
                                //if (!comparamblePlRevision.Approved)
                                //{
                                //    result.Errors.Add(string.Format("Последняя ревизия опорного прайслиста '{0}' не апрувлена", string.Format("{0} {1}", comparablePl.PriceListNumber, comparablePl.PriceListAdditionalNumber ?? "")));
                                //}
                                //else
                                {
                                    //var comparableItems = comparamblePlRevision.PriceListRevisionItems.ToList();
                                    //var testAvrItems = avrPors.SelectMany(s => s.PorItems).ToList();
                                    //testAvrItems.FirstOrDefault().
                                    //var testJoin = comparableItems.Join(avrPors.SelectMany(s => s.PorItems).ToList(), pli => pli.Name, poi => poi.Description, (pli, poi) => new { pli, poi }).ToList();
                                    //var joinedPlsAVRItems = comparableItems.Join(avrPors.SelectMany(s => s.PorItems).ToList(), pli => pli.Name, poi => poi.Description, (pli, poi) => new { pli, poi }); ;
                                    //   .Where(p => p.poi.PriceListRevisionItem.PriceListRevision.PriceList.Id != comparablePl.Id).ToList();
                                    //if (joinedPlsAVRItems != null && joinedPlsAVRItems.Count() == 0)
                                    //{
                                    //    result.Errors.Add(string.Format("Нет пересекающихся элементов между '{0}' и АВР ПОР'ами подрядчика '{1}'", comparablePl.PriceListNumber + comparablePl.PriceListAdditionalNumber ?? "", result.Subcontractor.Name));

                                    //}
                                    //else
                                    {
                                        //Группируем аврайтемы по одинаковым ревизиям
                                        var grouppedAvrItemsByPLR = avrPors.SelectMany(s => s.PorItems).Where(a=>a.PriceListRevisionItem!=null).GroupBy(g => g.PriceListRevisionItem.PriceListRevision).ToList();
                                        // Группируем их с мапами для того чтоб отфильтровать те, у которых нету опорных пл
                                        
                                        var grJoinPLandMaps = grouppedAvrItemsByPLR.GroupJoin(maps, r=>r.Key.PriceList.Id,m=>m.PriceList.Id,(gr,map)=>new {gr,map}).ToList();
                                        // список тех, для которых нет опорных прайслистов
                                        var uncpomparablePLsGroup = grJoinPLandMaps.Where(g => g.map.Count() == 0).ToList();
                                        
                                        var testUncomparablePLS = uncpomparablePLsGroup.Select(c => string.Format("{0}{1}",c.gr.Key.PriceList.PriceListNumber,c.gr.Key.PriceList.PriceListAdditionalNumber??"")).ToList();
                                        result.UncomparablePLS.AddRange(testUncomparablePLS);
                                        var comparablePLsGroup = grJoinPLandMaps.Where(g => g.map.Count() == 1).ToList();

                                        // цикл по группам с компаралб прайслистами
                                        foreach (var group in comparablePLsGroup)
                                        {

                                            var map = group.map.FirstOrDefault();
                                            var comparablePls = map.ComparablePriceList;
                                            var lastRevision = comparablePls.PriceListRevisions.OrderByDescending(r => r.CreationDate).FirstOrDefault();
                                            if (!lastRevision.Approved)
                                            {
                                                result.UnApprovedRevisions.Add(string.Format("{0}{1}",comparablePls.PriceListNumber,comparablePls.PriceListAdditionalNumber??""));
                                                continue;
                                            }
                                            var lastRevisionItems = lastRevision.PriceListRevisionItems;
                                            // цикл по айтемам, сгруппированным по прайслистам
                                            foreach (var item in group.gr)
                                            {

                                                var comparableItem = lastRevisionItems.FirstOrDefault(i => i.Name == item.PriceListRevisionItem.Name);
                                                Debug.WriteLine(string.Join(", ",lastRevisionItems.Select(s=>s.Name)));
                                                Debug.WriteLine(item.PriceListRevisionItem.Name);
                                                if (comparableItem != null)
                                                {

                                                    // непонятно пока как посчитать, и что это такое
                                                    decimal middlePrice = 0;
                                                    var sameItems = activeItems.Where(i => i.Name == item.Description && i.PriceListRevision.Id != item.PriceListRevisionItem.Id);
                                                    if (sameItems.Count() == 0)
                                                    {
                                                        middlePrice = comparableItem.Price;
                                                    }
                                                    else
                                                    {
                                                        middlePrice = sameItems.Average(i => i.Price);
                                                    }
                                                    var md = new CompareReportModel();
                                                    if (item.PriceListRevisionItem != null)
                                                    {
                                                        md.Date = item.PriceListRevisionItem.PriceListRevision.SignDate;
                                                        md.PriceListFrom = string.Format("{0} {1}", item.PriceListRevisionItem.PriceListRevision.PriceList.PriceListNumber, item.PriceListRevisionItem.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "");
                                                        md.RevisionFrom = item.PriceListRevisionItem.PriceListRevision.Id;
                                                        md.ItemIdFrom = item.PriceListRevisionItem.Id;
                                                        md.PriceListIdFrom = item.PriceListRevisionItem.PriceListRevision.PriceList.Id;
                                                        md.PorPriceCoef = item.Coeff;

                                                    }
                                                    // ТО - из прайс листа. FROM из поров
                                                    md.PriceListTo = string.Format("{0} {1}", comparablePls.PriceListNumber, comparablePls.PriceListAdditionalNumber ?? "");
                                                    md.RevisionTo = lastRevision.Id;
                                                   // md.SubcontractorTo = result.SourceSubontractor.SAPName;
                                                    md.SubcontractorFrom = result.Subcontractor.SAPName;
                                                    md.Value = comparableItem.Price * item.NetQty - item.Price /(item.Coeff.HasValue?item.Coeff.Value:1)* item.NetQty;
                                                    md.Por = item.POR is AVRPOR ? ((AVRPOR)item.POR).AVRId : "";
                                                    md.ItemIdTo = comparableItem.Id;
                                                    md.PriceFrom =  item.Price/(item.Coeff.HasValue?item.Coeff.Value:1);
                                                    md.PriceTo = comparableItem.Price;
                                                    md.MiddlePrice = (middlePrice == 0 ? (decimal?)null : middlePrice).Value.FinanceRound(); ;
                                                    md.DescriptionTo = comparableItem.Name;
                                                    md.DescriptionFrom = item.Description;
                                                    md.SapCodeTo = comparableItem.SAPCode.Code;
                                                    md.SapCodeFrom = item.Code;
                                                    md.PriceListIdTo = comparablePls.Id;
                                                    md.PorQuantity = item.NetQty;
                                                    md.PorDate = item.POR.PrintDate;
                                                    string PO = string.Empty ;
                                                    if(item.POR is AVRPOR)
                                                    {
                                                        string avr = ((AVRPOR)item.POR).AVRId;
                                                        
                                                        //var avrf = context.ShAVRf.FirstOrDefault(a=>a.AVRId==avr);
                                                        //if(avrf!=null)
                                                        //{
                                                        //    PO = avrf.PurchaseOrderNumber;
                                                        //}
                                                        //else
                                                        //{
                                                            var avrs = context.ShAVRs.FirstOrDefault(a=>a.AVRId==avr);
                                                            if(avrs!=null)
                                                            {
                                                                PO = avrs.PurchaseOrderNumber;
                                                            }
                                                        //}
                                                    }

                                                    md.PO = PO;
                                                    result.CompareReportModelList.Add(md);

                                                }
                                                 else
                                                {
                                                    result.UncomparableItems.Add(string.Format("{0}({1} {2}({3}))", item.Description, item.PriceListRevisionItem.PriceListRevision.PriceList.PriceListNumber, item.PriceListRevisionItem.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "", item.PriceListRevisionItem.PriceListRevision.PriceList.Id));
                                                }
                                            }
                                           
                                        }

                                    }


                                    //var joinedPlToPORItems = comparableItems.Join(toPors.SelectMany(p => p.SATTOItems), ci => ci.Name, toi => toi.PriceListRevisionItem.Name, (pli, poi) => new { pli, poi });
                                    ////  .Where(p => p.poi.PriceListRevisionItem.PriceListRevision.PriceList.Id != comparablePl.Id).ToList();
                                    //if (joinedPlToPORItems.Count() == 0)
                                    //{
                                    //    result.Errors.Add(string.Format("Нет пересекающихся элементов между прайслистом '{0}' и ТО ПОР'ами подрядчика '{1}'", comparablePl.PriceListNumber + comparablePl.PriceListAdditionalNumber ?? "", result.Subcontractor.Name));

                                    //}
                                    var grouppedPORItemsByPLR = toPors.SelectMany(p=>p.SATTOItems).Where(pl=>pl.PriceListRevisionItem!=null).GroupBy(p=>p.PriceListRevision).ToList();
                                
                                    // Группируем их с мапами для того чтоб отфильтровать те, у которых нету опорных пл

                                    var grJoinPORPLandMaps = grouppedPORItemsByPLR.GroupJoin(maps, r => r.Key.PriceList.Id, m => m.PriceList.Id, (gr, map) => new { gr, map }).ToList();
                                    // список тех, для которых нет опорных прайслистов
                                    var uncpomparablePORPLsGroup = grJoinPORPLandMaps.Where(g => g.map.Count() == 0).ToList();
                                    var testUncomporable = uncpomparablePORPLsGroup.Select(u => string.Format("{0}{1}",u.gr.Key.PriceList.PriceListNumber,u.gr.Key.PriceList.PriceListAdditionalNumber??"")).ToList();
                                    result.UncomparablePLS.AddRange(testUncomporable);
                                    var comparablePORPLsGroup = grJoinPORPLandMaps.Where(g => g.map.Count() == 1).ToList();
                                    foreach (var group in comparablePORPLsGroup)
                                    {


                                        var map = group.map.FirstOrDefault();
                                        var comparablePls = map.ComparablePriceList;
                                        var lastRevision = comparablePls.PriceListRevisions.OrderByDescending(r => r.CreationDate).FirstOrDefault();
                                        if (!lastRevision.Approved)
                                        {
                                            result.UnApprovedRevisions.Add(string.Format("{0}{1}", comparablePls.PriceListNumber, comparablePls.PriceListAdditionalNumber ?? ""));
                                            continue;
                                        }
                                        var lastRevisionItems = lastRevision.PriceListRevisionItems;

                                        foreach (var item in group.gr)
                                        {
                                            
                                            var comparableItem = lastRevisionItems.FirstOrDefault(i => i.Name == item.PriceListRevisionItem.Name);
                                            Debug.WriteLine(string.Join(", ", lastRevisionItems.Select(s => s.Name)));
                                            Debug.WriteLine(item.PriceListRevisionItem.Name);
                                            if (comparableItem != null)
                                            {
                                                decimal _middlePrice = 0;
                                                var _sameItems = activeItems.Where(i => i.Name == item.Description && i.PriceListRevision.Id != item.PriceListRevision.Id);
                                                if (_sameItems.Count() == 0)
                                                    _middlePrice = 0;
                                                else
                                                    _middlePrice = _sameItems.Average(i => i.Price);

                                                var md = new CompareReportModel();
                                                if (item.PriceListRevisionItem != null)
                                                {
                                                    md.Date = item.PriceListRevision.SignDate;
                                                    md.RevisionFrom = item.PriceListRevision.Id;
                                                    md.PriceListFrom = string.Format("{0} {1}", item.PriceListRevision.PriceList.PriceListNumber, item.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "");
                                                    md.PriceListIdFrom = item.PriceListRevisionItem.PriceListRevision.PriceList.Id;
                                                }

                                                md.PriceListTo = string.Format("{0} {1}", comparableItem.PriceListRevision.PriceList.PriceListNumber, comparableItem.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "");

                                                md.RevisionTo = comparableItem.PriceListRevision.Id;
                                                md.SubcontractorFrom = result.Subcontractor.SAPName;
                                                //md.SubcontractorTo = result.SourceSubontractor.SAPName;
                                                md.Value = comparableItem.Price * item.Quantity - item.Price * item.Quantity;
                                                md.Por = item.SATTO.TO;
                                                md.ItemIdFrom =  item.PriceListRevisionItem.Id;
                                                md.ItemIdTo = comparableItem.Id;
                                                md.PriceFrom = item.Price;
                                                md.PriceTo =  comparableItem.Price;
                                                md.MiddlePrice = (_middlePrice == 0 ? (decimal?)0 : _middlePrice).Value.FinanceRound();
                                                md.DescriptionFrom = item.Description;
                                                md.DescriptionTo = comparableItem.Name;
                                                md.SapCodeFrom = item.PriceListRevisionItem.SAPCode.Code;
                                                md.SapCodeTo = comparableItem.SAPCode.Code;
                                                md.PriceListIdTo = comparablePls.Id;
                                                md.PorQuantity = item.Quantity;
                                                md.PorDate = item.SATTO.CreateUserDate;
                                                string PO = string.Empty;
                                                if (!string.IsNullOrEmpty(item.SATTO.TO))
                                                {
                                                    string to = item.SATTO.TO;

                                                    var shTO = context.ShTOes.FirstOrDefault(a => a.TO == to);
                                                    if (shTO != null)
                                                    {
                                                        PO = shTO.PONumber;
                                                    }
                                                }

                                                md.PO = PO;


                                                result.CompareReportModelList.Add(md);
                                            }
                                            else
                                            {
                                                result.UncomparableItems.Add(string.Format("{0}({1} {2}({3}))",item.Description, item.PriceListRevision.PriceList.PriceListNumber,item.PriceListRevision.PriceList.PriceListAdditionalNumber??"",item.PriceListRevision.PriceList.Id));

                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}