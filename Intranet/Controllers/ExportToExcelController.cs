using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using ExcelParser.Extentions;

using DbModels.DataContext;
using DbModels.Repository;
using EpplusInteract;

namespace Intranet.Controllers
{
    public class ExportToExcelController : Controller
    {
        //
        // GET: /ExportToExcel/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllPriceLists()
        {
            var maxCount = 0;
            using (Context context = new Context())
            {
                List<GetAllPriceListsModel> model = new List<GetAllPriceListsModel>();
                foreach (var subcontractor in context.SubContractors)
                {
                    foreach (var priceList in context.PriceLists.Where(pl => pl.SubContractor.Id == subcontractor.Id))
                    {
                        foreach (var revision in context.PriceListRevisions.Where(plr => plr.PriceList.Id == priceList.Id))
                        {




                            foreach (var item in context.PriceListRevisionItems.Where(plri => plri.PriceListRevision.Id == revision.Id))
                            {
                                maxCount++;
                                //if (maxCount < 14120)
                                {
                                    try
                                    {


                                        var mod = new GetAllPriceListsModel()
                                        {
                                            SubContractorName = subcontractor.Name,
                                            SubContractorVendor = subcontractor.SAPNumber,
                                            SubContractorAddress = subcontractor.Address,
                                            PriceListNumber = priceList.PriceListNumber,
                                            PriceListAdditionalNumber = priceList.PriceListAdditionalNumber,
                                            PriceListId = priceList.Id,
                                            PriceListRevisionId = revision.Id,
                                            PriceListSignDate = priceList.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().SignDate,
                                            PriceListExpiryDate = priceList.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().ExpiryDate,
                                            PriceListRevisionUploaded = revision.Uploaded,
                                            PriceListRevisionItemSAPCode = item.SAPCode.Code,
                                            PriceListRevisionItemName = item.Name,
                                            PriceListRevisionItemUnit = item.Unit,
                                            PriceListRevisionItemPrice = item.Price,
                                            PriceListRevisionItemExcistedInSAP = item.SAPCode.ExistedInSAP

                                        };
                                        model.Add(mod);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        model.Add(new GetAllPriceListsModel() { SubContractorName = "ERROR" });
                                    }
                                }
                                //break;
                            }
                        }
                    }
                }

                EpplusService service = new EpplusService();
                service.InsertTableToWorkBook(model.ToDataTable(typeof(GetAllPriceListsModel)), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, ShowRowStripes = true });
                var wbookbyteArray = service.GetBytes();
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("AllItems({0}).xlsx", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                return File(wbookbyteArray, ".xlsx");
            }

        }
        public ActionResult GetActivePriceLists()
        {
            var maxCount = 0;
            using (Context context = new Context())
            {
                PriceListRepository reposit = new PriceListRepository(context);
                List<GetAllPriceListsModel> model = new List<GetAllPriceListsModel>();
                foreach (var subcontractor in context.SubContractors)
                {
                    foreach (var priceList in reposit.GetWorkablePriceLists(subcontractor.Id))
                    {
                        //foreach (var revision in context.PriceListRevisions.Where(plr => plr.PriceList.Id == priceList.Id))
                        var allrevisions = context.PriceListRevisions.Where(plr => plr.PriceList.Id == priceList.Id).OrderByDescending(o => o.Id);
                        var lastRevision = allrevisions.FirstOrDefault();
                        if (lastRevision != null && (!lastRevision.ExpiryDate.HasValue || lastRevision.ExpiryDate.Value >= DateTime.Now)&& !lastRevision.PriceList.Comparable)
                       // var activeRevision = context.PriceListRevisions.Where(plr => plr.PriceList.Id == priceList.Id).OrderBy(o=>o.Id).FirstOrDefault(r => r.ExpiryDate == null || r.ExpiryDate < DateTime.Now);
                        {


                         //   if (lastRevision != null)
                            {

                                foreach (var item in context.PriceListRevisionItems.Where(plri => plri.PriceListRevision.Id == lastRevision.Id))
                                {

                                    try
                                    {


                                        var mod = new GetAllPriceListsModel()
                                        {
                                            SubContractorName = subcontractor.Name,
                                            SubContractorVendor = subcontractor.SAPNumber,
                                            SubContractorAddress = subcontractor.Address,
                                            PriceListNumber = priceList.PriceListNumber,
                                            PriceListAdditionalNumber = priceList.PriceListAdditionalNumber,
                                            PriceListId = priceList.Id,
                                            PriceListRevisionId = lastRevision.Id,
                                            PriceListSignDate = priceList.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().SignDate,
                                            PriceListExpiryDate = priceList.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().ExpiryDate,
                                            PriceListRevisionUploaded = lastRevision.Uploaded,
                                            PriceListRevisionItemSAPCode = item.SAPCode.Code,
                                            PriceListRevisionItemName = item.Name,
                                            PriceListRevisionItemUnit = item.Unit,
                                            PriceListRevisionItemPrice = item.Price,
                                            PriceListRevisionItemExcistedInSAP = item.SAPCode.ExistedInSAP,
                                            PriceListRevisionItemId = item.Id


                                        };
                                        model.Add(mod);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        model.Add(new GetAllPriceListsModel() { SubContractorName = "ERROR" });
                                    }

                                    //break;
                                }
                            }
                        }
                    }
                }

                EpplusService service = new EpplusService();
                service.InsertTableToWorkBook(model.ToDataTable(typeof(GetAllPriceListsModel)), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, ShowRowStripes = true });
                var wbookbyteArray = service.GetBytes();
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("ActiveItems({0}).xlsx", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                return File(wbookbyteArray, ".xlsx");
            }

        }

        



        private class GetAllPriceListsModel
        {
            //public SubContractor SubContractor { get; set; }
            //public PriceList PriceList { get; set; }
            //public PriceListRevision PriceListRevision { get; set; }
            //public PriceListRevisionItem PriceListRevisionItem { get; set; }

            public string SubContractorVendor { get; set; }
            public string SubContractorName { get; set; }
            public string SubContractorAddress { get; set; }
            public string PriceListNumber { get; set; }
            public string PriceListAdditionalNumber { get; set; }
            public int PriceListId { get; set; }
            public int PriceListRevisionId { get; set; }
            public DateTime? PriceListSignDate { get; set; }
            public DateTime? PriceListExpiryDate { get; set; }
            public DateTime? PriceListRevisionUploaded { get; set; }
            public string PriceListRevisionItemSAPCode { get; set; }
            public string PriceListRevisionItemName { get; set; }
            public string PriceListRevisionItemUnit { get; set; }
            public decimal PriceListRevisionItemPrice { get; set; }
            public bool PriceListRevisionItemExcistedInSAP { get; set; }
            public int PriceListRevisionItemId { get; set; }

        }

    }
}
