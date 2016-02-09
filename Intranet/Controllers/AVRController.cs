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
            using (Context context = new Context())
            {
                var items = AVRItemRepository.GetAVRItems(avrId, context);
                var shAVR = context.ShAVRs.FirstOrDefault(a => a.AVRId == avrId);
                if (shAVR == null)
                    return null;
                // если подрядчик эрикссон, то ксюша этот авр не опрайсовывала и нам нужны позиции за рамками или аос из сх
                if (AVRRepository.HasEricssonSubcontractor(shAVR))
                {
                    var erItems = items.Select(i => new AVRItemModel
                    {
                        avrItemId = i.AVRItemId,
                        noteVC = i.NoteVC,
                        shQuantity = i.Quantity.HasValue?i.Quantity.Value:0,
                        shDescription = i.Description,
                        shPrice = i.Price,
                        workReason = i.WorkReason,
                     
                     


                    });
                    return Json(erItems, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    // последняя Катина порайсовка
                    var avrMusItems = context.
                        SatMusItems.Where(i => i.AVRId == avrId).ToList();

                    if (avrMusItems.Count == 0)
                    {
                        // если мусАйтемов еще нет, то нужно выбрать позиции опрайсованные Ксюшей
                        var avrPOR = context.AVRPORs.Where(p => p.AVRId == avrId).OrderByDescending(p => p.PrintDate).FirstOrDefault();
                        if (avrPOR != null)
                        {
                            var porItems = new List<AVRItemModel>();
                            foreach (var porItem in avrPOR.PorItems.ToList())
                            {
                                var shItem = items.FirstOrDefault(i => i.AVRItemId == porItem.ItemId);
                                if (shItem != null)
                                {

                                    porItems.Add(new AVRItemModel
                                    {
                                        avrItemId = porItem.ItemId,
                                        shPrice = porItem.Price,
                                        noteVC = shItem.NoteVC,
                                        shQuantity = shItem.Quantity.HasValue ? shItem.Quantity.Value : 0,
                                        shDescription = porItem.Description,
                                        workReason = shItem.WorkReason,
                                    });

                                }
                            }
                            return Json(porItems, JsonRequestBehavior.AllowGet);
                        }



                    }
                    else
                    {
                        // если же Катя уже опрайсовала, то выбираем последнюю опрайсовку и из нее позиции
                        var lastMusItems = avrMusItems.GroupBy(a => a.VCRequestNumber).OrderByDescending(g => g.Key).FirstOrDefault();
                        foreach (var musItem in lastMusItems)
                        {
                            var itemModel = new AVRItemModel()
                            {
                                description = musItem.Description,
                                noteVC = musItem.NoteVC,
                                quantity = musItem.Quantity,
                                price = musItem.Price,
                                vcCustomPos = musItem.CustomPos,
                                workReason = musItem.WorkReason,
                                vcUseCoeff = musItem.UseCoeff,
                                priceListRevisionItemId = musItem.PriceListRevisionItem != null ? musItem.PriceListRevisionItem.Id : (int?)null
                            };
                            if(musItem.AvrItemId.HasValue)
                            {
                                var shItem = context.ShAVRItems.FirstOrDefault(i => i.AVRItemId == musItem.AvrItemId);
                                if(shItem!=null)
                                {
                                    itemModel.avrItemId = shItem.AVRItemId;
                                    itemModel.shQuantity = shItem.Quantity ?? 0;
                                    itemModel.shDescription = shItem.Description;
                                    itemModel.shPrice = shItem.Price;
                                }
                            }

                        }
                    }
                }
          

                return Json(null, JsonRequestBehavior.AllowGet);
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
            using (Context context = new Context())
            {

                var avrs = AVRRepository.GetNeedVCPriceAvrs(context);
             
              
                return Json(avrs.Select(a => new {
                    avr = a.AVRId,
                    workStart = a.WorkStart,
                    workEnd = a.WorkEnd,
                    rukOtdelaBy = a.BranchManagar,
                    priority = a.Priority,
                    city = a.Subregion,
                    total = a.TotalVCReexpose
                }), JsonRequestBehavior.AllowGet);

            }
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult PostPreprice(PrepriceModel model)
        {
            var now = DateTime.Now;
            ShAVRs shAvr = null;
            // string shVCRequestName = string.Format("{0}:{1}", model.avrId, DateTime.Now.ToString("yyyyMMddHHmmss"));
            using (Context context = new Context())
            {
                //TODO: Теперь работаем с другими объектами
                //foreach (var item in model.items)
                //{


                //    var shItem = context.ShAVRItems.Find(item.itemId);
                //    var isCustomItem = item.vcCustomPos??false;

                //    shItem.VCCustomPos = isCustomItem;
                //    if (shItem != null)
                //    {
                //        //if (shAvr == null)
                //        //    shAvr = shItem.AVRS;
                //        if (isCustomItem)
                //        {
                //            context.SATPrepricedItems.Add(new SATPrepricedItem { AVRId = model.avrId, PrepriceDate = now, AVRItemId = item.itemId, vcQuantity = item.vcQuantity, IsCustomItem = isCustomItem, VCDescription = item.vcDescription,  vcPrice = item.vcCustomPrice , NoteVC = item.noteVC, WorkReason = item.workReason });
                //            shItem.VCCustomPos = true;
                //            shItem.VCPriceListRevisionItemId = null;
                //            shItem.VCDescription = item.vcDescription;
                //            shItem.VCPrice = item.vcCustomPrice;

                //        }
                //        else
                //        {
                //            var plItem = context.PriceListRevisionItems.Find(item.vcPriceListRevisionItemId);

                //            if (plItem != null)
                //            {
                //                //var shVCRequestNumber =
                //                context.SATPrepricedItems.Add(new SATPrepricedItem { AVRId = model.avrId, PrepriceDate = now, AVRItemId = item.itemId, Item = plItem, vcQuantity = item.vcQuantity, VCUseCoeff = (item.vcUseCoeff.HasValue?item.vcUseCoeff.Value:false), VCCoeff = item.vcCoeff , NoteVC = item.noteVC, WorkReason = item.workReason });
                //                shItem.VCPriceListRevisionItemId = plItem.Id;
                //                shItem.VCDescription = plItem.Name;
                //                if (item.vcUseCoeff.HasValue&&item.vcUseCoeff.Value)
                //                {
                //                    shItem.VCPrice = plItem.Price * (item.vcCoeff.HasValue ? item.vcCoeff : 1);
                //                }
                //                else
                //                {
                //                    shItem.VCPrice = plItem.Price;
                //                }
                //            }

                //        }
                //        shItem.NoteVC = item.noteVC;
                //        if (!string.IsNullOrEmpty(item.workReason))
                //            shItem.WorkReason = item.workReason;


                //        // для ускорения теста
                //        shItem.VCQuantity = item.vcQuantity;





                //    }
                //}
                //// ускорение для теста


                //context.SaveChanges();
            }
            return Json(true);
        }
    }
}