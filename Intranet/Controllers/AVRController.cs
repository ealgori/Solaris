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
                        shQuantity = i.Quantity.HasValue ? i.Quantity.Value : 0,
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
                        List<AVRItemModel> musItemModels = new List<AVRItemModel>();
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
                            if (musItem.AvrItemId.HasValue)
                            {
                                var shItem = context.ShAVRItems.FirstOrDefault(i => i.AVRItemId == musItem.AvrItemId);
                                if (shItem != null)
                                {
                                    itemModel.avrItemId = shItem.AVRItemId;
                                    itemModel.shQuantity = shItem.Quantity ?? 0;
                                    itemModel.shDescription = shItem.Description;
                                    itemModel.shPrice = shItem.Price;
                                }
                            }

                            musItemModels.Add(itemModel);
                        }
                        return Json(musItemModels, JsonRequestBehavior.AllowGet);
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


                return Json(avrs.Select(a => new
                {
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


            using (Context context = new Context())
            {
                var shAVRs = context.ShAVRs.FirstOrDefault(a => a.AVRId == model.avrId);
                if (shAVRs == null)
                    return Json(false);
                string shVCRequestName = string.Format("{0}:{1}", model.avrId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                //TODO: Теперь работаем с другими объектами
                foreach (var item in model.items)
                {
                    var musItem = new SatMusItem();
                    if (item.avrItemId != null)
                    {
                        var shItem = context.ShAVRItems.FirstOrDefault(i => i.AVRItemId == item.avrItemId);
                        if (shItem != null)
                        {
                            musItem.AvrItemId = item.avrItemId;
                            musItem.ShDescription = shItem.Description;
                        }
                    }
                    if (item.vcCustomPos)
                    {
                        musItem.Price = item.price ?? 0;
                        musItem.CustomPos = item.vcCustomPos;
                        musItem.Description = item.description;
                    }
                    else
                    {
                        if (item.priceListRevisionItemId.HasValue)
                        {
                            var plri = context.PriceListRevisionItems.FirstOrDefault(i => i.Id == item.priceListRevisionItemId);
                            if (plri != null)
                                musItem.PriceListRevisionItem = plri;
                        }
                        else
                        {
                            continue;
                        }
                        musItem.UseCoeff = item.vcUseCoeff;
                    }
                    musItem.Quantity = item.quantity ?? 0;
                    musItem.WorkReason = item.workReason;
                    musItem.NoteVC = item.noteVC;
                    musItem.VCRequestNumber = shVCRequestName;
                    musItem.AVRId = model.avrId;
                    context.SatMusItems.Add(musItem);
                }
                var vcRequestToUpload = new VCRequestToCreate
                {
                    AVRId = shAVRs.AVRId,
                    CreateDate = DateTime.Now,
                    VCRequestNumber = shVCRequestName,
                    UserName = User.Identity.Name
                };
                context.VCRequestsToCreate.Add(vcRequestToUpload);



                context.SaveChanges();
            }
            return Json(true);
        }
    }
}