using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using Intranet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using DbModels.DomainModels.SAT;
using DbModels.DomainModels.ShClone;

namespace Intranet.Controllers
{
    public class AVRController : Controller
    {
        public ActionResult Preprice()
        {
            return View();
        }

        public JsonResult GetAVRItems(string avrId)
        {
            using(Context context = new Context())
            {
                var items = AVRItemRepository.GetAVRItems(avrId, context);
                var shAVR = context.ShAVRs.FirstOrDefault(a => a.AVRId == avrId);
                if (shAVR == null)
                    return null;
               
                //все позиции поров по этому авр
                var SATPorItems = context.AVRPORs.Where(a => a.AVRId == avrId).SelectMany(i => i.PorItems).ToList();
                var resultItems = items.Select(i => new AVRItemModel
                {
                    id = i.AVRItemId
                    //, shDesc = i.Limit!=null?i.Limit.Description: i.Description??"Не указан"
                    ,
                    shDesc = i.Description ?? "Не указан",
                    //,
                    shPrice = i.Price
                    ,
                    shQuantity = i.Quantity.HasValue ? i.Quantity : 0
                    ,
                    vcPriceListRevisionItemId = i.VCPriceListRevisionItemId
                    ,
                    vcQuantity = i.VCQuantity
                    ,
                    vcDescription = i.VCDescription
                    ,
                    vcCustomPos = i.VCCustomPos
                    ,
                    vcPrice = i.VCPrice
                    ,
                    vcUseCoeff = i.VCUseCoeff,

                    itemId = i.AVRItemId,

                    noteVC = i.NoteVC
                }).ToList();

                foreach (var item in resultItems)
                {
                    if (shAVR.Subcontractor == Constants.EricssonSubcontractor || shAVR.SubcontractorRef == Constants.EricssonSubcontractor)
                    {
                       // var avtItem = shAVRItems.FirstOrDefault(i=>i.AVRItemId== item.)
                    }
                    else
                    {
                        var satItem = SATPorItems.FirstOrDefault(i => i.ItemId == item.id);
                        if (satItem != null)
                        {
                            item.shPrice = satItem.Price;
                        }
                    }
                }


                return Json(resultItems ,JsonRequestBehavior.AllowGet);
             //   System.Web.Script.Serialization.JavaScriptSerializer
            }
        }

        public string PrintOrder()
        {
            using (Context context = new Context())
            {
               

                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("order.xlsm"));
                    var items = context.ShAVRItems.Where(a => a.Limit != null).Take(10).ToList();
                    var items2 = context.ShAVRItems.Where(a => !string.IsNullOrEmpty(a.ECRType)).Take(20).ToList();
               //   return File(ExcelParser.EpplusInteract.CreateAVROrder.CreateOrderFile(items.Union(items2).ToList(),"test1234Number"),".xlsm");
               
                  //  var path =  ExcelParser.ExcelParser.GenerateAVREmail.Generate("205779");//.CreateOrderFile(items.Union(items2).ToList(),"test1234Number"),".xlsm");

                    return "f";
            }

            return null;

            // return File(ExcelParser.EpplusInteract.CreatePor.CreatePorFile(Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POR-" + Id.ToString() + DateTime.Now.ToString("(yyyyMMddHHmmss)"));
        }

        public ActionResult GetAVRForPreprice()
        {
            using(Context context = new Context())
            {

                var avrs = context.ShAVRs.Where(AVRRepository.ReadyForPrePricedComp).ToList(); //.Where(a => a.Items.Any(AVRItemRepository.IsAddonSalesOrExceedComp)).ToList();
                var avrForPreprice = new List<ShAVRs>();
                foreach (var avr in avrs)
                {
                    if (avr.Subcontractor == Constants.EricssonSubcontractor||avr.SubcontractorRef== Constants.EricssonSubcontractor)
                    {
                        avrForPreprice.Add(avr);
                    }
                    else
                    {
                        var avrPor = context.AVRPORs.FirstOrDefault(p => p.AVRId == avr.AVRId);
                        if (avrPor != null)
                            avrForPreprice.Add(avr);
                    }

                }
                return Json(avrForPreprice.Select(a => new { avr = a.AVRId, workStart = a.WorkStart, workEnd = a.WorkEnd, rukOtdelaBy = a.BranchManagar, priority = a.Priority, city = a.Subregion }),JsonRequestBehavior.AllowGet);

            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult PostPreprice(PrepriceModel model)
        {
            var now = DateTime.Now;
            ShAVRs shAvr=null;
           // string shVCRequestName = string.Format("{0}:{1}", model.avrId, DateTime.Now.ToString("yyyyMMddHHmmss"));
            using(Context context = new Context())
            {
                foreach (var item in model.items)
                {
                   
                   
                    var shItem = context.ShAVRItems.Find(item.itemId);
                    var isCustomItem = item.vcCustomPos??false;
                    
                    shItem.VCCustomPos = isCustomItem;
                    if (shItem != null)
                    {
                        //if (shAvr == null)
                        //    shAvr = shItem.AVRS;
                        if (isCustomItem)
                        {
                            context.SATPrepricedItems.Add(new SATPrepricedItem { AVRId = model.avrId, PrepriceDate = now, AVRItemId = item.itemId, vcQuantity = item.vcQuantity, IsCustomItem = isCustomItem, VCDescription = item.vcDescription,  vcPrice = item.vcCustomPrice  });
                            shItem.VCCustomPos = true;
                            shItem.VCPriceListRevisionItemId = null;
                            shItem.VCDescription = item.vcDescription;
                            shItem.VCPrice = item.vcCustomPrice;
                            shItem.NoteVC = item.noteVC;
                        }
                        else
                        {
                            var plItem = context.PriceListRevisionItems.Find(item.vcPriceListRevisionItemId);

                            if (plItem != null)
                            {
                                //var shVCRequestNumber =
                                context.SATPrepricedItems.Add(new SATPrepricedItem { AVRId = model.avrId, PrepriceDate = now, AVRItemId = item.itemId, Item = plItem, vcQuantity = item.vcQuantity, VCUseCoeff = (item.vcUseCoeff.HasValue?item.vcUseCoeff.Value:false), VCCoeff = item.vcCoeff });
                                shItem.VCPriceListRevisionItemId = plItem.Id;
                                shItem.VCDescription = plItem.Name;
                                if (item.vcUseCoeff.HasValue&&item.vcUseCoeff.Value)
                                {
                                    shItem.VCPrice = plItem.Price * (item.vcCoeff.HasValue ? item.vcCoeff : 1);
                                }
                                else
                                {
                                    shItem.VCPrice = plItem.Price;
                                }
                            }
                            
                        }
                        
                        
                        // для ускорения теста
                        shItem.VCQuantity = item.vcQuantity;
                        
                       



                    }
                }
                // ускорение для теста
              

                context.SaveChanges();
            }
            return Json(true);
        }
    }
}