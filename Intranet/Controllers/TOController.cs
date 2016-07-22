using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using Intranet.Models;
using CommonFunctions.Extentions;
using Intranet.ActionFilters;
using DbModels.DomainModels.SAT;
using System.Net;
using DbModels.Repository;
using System.Net.Http;
//using Intranet.Filters;
//using System.Web.Http;

namespace Intranet.Controllers
{
    public class TOController : Controller
    {
        //
        // GET: /TO/

        public ActionResult Index()
        {
            return View();
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult CreateTODoc()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            //  breadCrumbs.Add(new BreadCrumbModel() { Name = "POR's", Path = Url.Action("Index", "POR", null, Request.Url.Scheme) });
            breadCrumbs.Add(new BreadCrumbModel() { Name = "Create TO" });
            ViewBag.Path = breadCrumbs;
            TOViewModel model = new TOViewModel();
            return View(model);

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult CreateTODocV2()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            //  breadCrumbs.Add(new BreadCrumbModel() { Name = "POR's", Path = Url.Action("Index", "POR", null, Request.Url.Scheme) });
            breadCrumbs.Add(new BreadCrumbModel() { Name = "Create TO" });
            ViewBag.Path = breadCrumbs;
            TOViewModel model = new TOViewModel();
            return View(model);

        }
        [HttpPost]
        public ActionResult CreateTODoc(TOViewModel model)
        {
            CreatePORResultViewModel result = new CreatePORResultViewModel();

            if (model.Items == null || model.Items.Any(i => (i.ItemId == null) || (i.ItemId == 0)))
            {
                result.Success = false;
                result.Message = "Все айтемы должны быть привязаны к прайсовым позициями";
                result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
            }
            else
            {

                using (Context context = new Context())
                {
                    var shTO = context.ShTOes.Find(model.TO);
                    if (shTO == null)
                    {
                        result.Success = false;
                        result.Message = "Ошибка.. не нашли ТО";
                        result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                        return Json(result);

                    }
                    else
                    {
                        if (!shTO.POIssueDate.HasValue)
                        {
                            result.Success = false;
                            result.Message = "Ошибка.. проставьте POIssueDate в СХ ";
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }
                        var shContact = context.ShContacts.FirstOrDefault(c => c.Contact == shTO.Subcontractor);
                        if (shContact == null)
                        {
                            result.Success = false;
                            result.Message = "Ошибка.. в СХ отсутсвтует контакт с именем " + shTO.Subcontractor;
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }
                        TORepository repository = new TORepository(context);
                        #region

                        SATTO satTo = new SATTO();
                        //var activity = repository.GetToActivity(shTO.TO);
                        if (!string.IsNullOrEmpty(shTO.ActivityCode))
                        {
                            satTo.Activity = shTO.ActivityCode;
                            //satTo.POType = activity.POType;
                            //satTo.WorkDescription = activity.TOWorkDescription;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Ошибка.. не нашли Активность";
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }
                        var subcontractor = context.SubContractors.FirstOrDefault(s => s.ShName == shTO.Subcontractor);
                        if (subcontractor != null)
                        {
                            satTo.SubContractor = subcontractor.Name;
                            satTo.SubContractorSapNumber = subcontractor.SAPNumber;
                            satTo.SubContractorAddress = subcontractor.Address;
                        }

                        satTo.ToType = shTO.TOType;
                        satTo.TO = shTO.TO;
                        #endregion
                        var toItems = repository.GetTOItemModels(shTO.TO,false,true).ToList();

                        var firstNotSecond = toItems.Select(i => i.TOItem).Except(model.Items.Select(i => i.TOItem));
                        var secondNotFirst = model.Items.Select(i => i.TOItem).Except(toItems.Select(i => i.TOItem));



                        if (firstNotSecond.Count() != 0 || secondNotFirst.Count() != 0)
                        {
                            result.Success = false;
                            result.Message = "Ошибка.. количество или состав элементов из таблицы не соответствует количеству в СХ";
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }

                        PriceListRepository priceListRepository = new PriceListRepository(context);
                        var activePrices = priceListRepository.GetActivePriceListsRevisionItems(subcontractor.Id, 4, toItems.Min(i => i.TOPlanDate), toItems.Max(i => i.TOPlanDate));

                        ///TODO Этот код до ввода ФОЛОВ. Корректный, Переписан ниже на репозиторий

                        /// айтемы сджойненные с инфой с формы по тоайтемИД
                        //  var test = toItems
                        //    .Join(model.Items, to => to.TOItem, it => it.TOItem, (to, it) => new { toItem = to.TOItem, priceItem = it.ItemId, site = to.Site }).ToList();
                        //var items = toItems
                        //    .Join(model.Items, to => to.TOItem, it => it.TOItem, (to, it) => new { toItem = to.TOItem, priceItem = it.ItemId, site = to.Site, quantity = to.SiteQuantity, description = to.Description, planDate = to.TOPlanDate })
                        //    .Join(context.ShSITEs.ToList(), it => it.site, si => si.Site, (it, site) => new { toItem = it.toItem, priceItem = it.priceItem, site = it.site, quantity = it.quantity, address = site.Address, description = it.description, planDate = it.planDate, siteIndex = site.Index })
                        //    .GroupJoin(context.PriceListRevisionItems, i => i.priceItem, pli => pli.Id, (i, pli) => new { toItem = i.toItem, priceItem = i.priceItem, site = i.site, quantity = i.quantity, address = i.address, description = i.description, pli = pli.FirstOrDefault(), planDate = i.planDate, siteIndex = i.siteIndex })
                        //    .ToList();

                        foreach (var item in model.Items)
                        {
                            var pli = context.PriceListRevisionItems.FirstOrDefault(i => i.Id == item.ItemId);
                            var toItem = toItems.FirstOrDefault(i => i.TOItem == item.TOItem);
                            if(pli!=null&& toItem!=null)
                            {
                                toItem.PLRI = pli;
                            }
                            else
                            {
                                throw (new Exception("Либо левый прайс, либо левый айтем"));
                            }
                        }


                        var unactivePrices = toItems.Select(i => i.PLRI).Except(activePrices);
                        if (unactivePrices.Count() > 0)
                        {
                            result.Success = false;
                            result.Message = "В списке присутствуют неактивные прайсовые позиции ";
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }




                        var sum = 0M;
                        var sumService = 0M;
                        var SumMaterials = 0M;
                        satTo.SATTOItems = new List<SATTOItem>();
                        satTo.NomerDogovora = shTO.NomerDogovora; 
                        satTo.DataDogovora = shTO.DataDogovora;

                        
                        var plistNames = toItems.Select(i => i.PLRI.PriceListRevision.PriceList.PriceListNumber).Distinct();
                        var plistDate = toItems.FirstOrDefault().PLRI.PriceListRevision.SignDate;
                        satTo.ProceListNumbers = string.Join(";", plistNames);
                        satTo.PriceListDate = plistDate;
                        satTo.WOVAT = shContact.WithOutVAT;
                        foreach (var item in toItems)
                        {
                            SATTOItem satTOItem = new SATTOItem();


                          
                            #region

                            satTOItem.PriceListRevision = item.PLRI.PriceListRevision;
                            satTOItem.PriceListRevisionItem = item.PLRI;
                            satTOItem.Quantity = item.SiteQuantity.Value;
                            satTOItem.Site = item.ShSite!=null?item.ShSite.Site:null;
                            satTOItem.FOL = item.ShFOL != null ? item.ShFOL.FOL : null;
                            satTOItem.SiteAddress = item.ShSite!=null?item.ShSite.Address:string.Format("{0}-{1}",item.ShFOL.StartPoint, item.ShFOL.DestinationPoint);
                            satTOItem.SiteIndex = item.ShSite!= null?item.ShSite.Index:0;
                            satTOItem.TOItemId = item.TOItem;
                            // satTOItem.SATTO = satTo;
                            satTOItem.PricePerItem = item.PLRI.Price;
                            satTOItem.Price = item.PLRI.Price * item.SiteQuantity.Value;
                            satTOItem.Description = item.Description;
                            satTOItem.PlanDate = item.TOPlanDate;
                            satTOItem.Unit = item.PLRI.Unit;
                            satTOItem.Type = "Service";
                            sum += satTOItem.Price;
                            sumService += satTOItem.Price;

                            satTo.SATTOItems.Add(satTOItem);
                            #endregion

                        }


                        var sampleItem = toItems.FirstOrDefault(s=>s.ShSite!=null|| s.ShFOL!=null);
                        satTo.Network = shTO.Network;
                        if (sampleItem != null)
                        {

                            if (sampleItem.ShSite != null)
                            {
                                satTo.Region = sampleItem.ShSite.MacroRegion;
                                satTo.Branch = sampleItem.ShSite.Branch;
                            }
                            else
                            {
                                if(sampleItem.FOL!=null)
                                {
                                    satTo.Region = sampleItem.ShFOL.MacroRegion;
                                    satTo.Branch = sampleItem.ShFOL.Branch;
                                }
                            }
                          
                            
                        }

                        //else
                        //{
                        //    result.Success = false;
                        //    result.Message = "Ошибка.. не нашли Сайт";
                        //    result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                        //    return Json(result);
                        //}


                        var toMatItems = repository.GetToMaterialItems(model.TO).ToList();
                        //if (toMatItems.Any(m => !m.Quantity.HasValue))
                        //{
                        //    result.Success = false;
                        //    result.Message = "Ошибка.. не для всех материалов задано количество";
                        //    result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                        //    return Json(result);
                        //}
                        if (toMatItems != null && toMatItems.Count() > 0)
                        {
                            var gjItems = toMatItems.GroupJoin(model.MatItems, tm => tm.MatTOId, mm => mm.MatItem, (tm, mm) => new { tm, mm })
                                .GroupJoin(context.ShSITEs, i => i.tm.SiteId, s => s.Site, (i, s) => new { i.tm, i.mm, s });


                            foreach (var item in gjItems)
                            {
                                var mmItem = item.mm.FirstOrDefault();
                                if (mmItem == null)
                                {
                                    result.Success = false;
                                    result.Message = "Ошибка.. Не все материалы присустствуют в списке. Попробуйте обновить страницу";
                                    result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                                    return Json(result);
                                }
                                else
                                {
                                    SATTOItem satTOItem = new SATTOItem();
                                    if ((item.tm.Price.HasValue && item.tm.Price != 0) && (!item.tm.IDItemFromPL.HasValue))
                                    {
                                        satTOItem.PricePerItem = item.tm.Price.Value;
                                        satTOItem.Price = item.tm.Price.Value * item.tm.Quantity;
                                        satTOItem.Unit = "Штука";
                                        satTOItem.Description = item.tm.Description;
                                    }
                                    else
                                    {


                                        var priceItem = activePrices.FirstOrDefault(a => a.Id == mmItem.ItemId);
                                        if (priceItem == null)
                                        {
                                            result.Success = false;
                                            result.Message = "Ошибка.. Не всем материалам просталены прайсовые позиции, либо проставлены неактивные прайсовые позиции";
                                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                                            return Json(result);
                                        }
                                        else
                                        {
                                            satTOItem.PriceListRevisionItem = priceItem;
                                            satTOItem.PriceListRevision = priceItem.PriceListRevision;
                                            satTOItem.PricePerItem = priceItem.Price;
                                            satTOItem.Unit = priceItem.Unit;
                                            satTOItem.Description = priceItem.Name;
                                            satTOItem.Price = priceItem.Price * item.tm.Quantity;


                                        }
                                    }


                                    #region
                                    var site = item.s.FirstOrDefault();

                                    if (site != null)
                                    {
                                        satTOItem.Site = site.Site;
                                        satTOItem.SiteIndex = site.Index;
                                        satTOItem.SiteAddress = site.Address;
                                    }
                                    satTOItem.MatTOItemId = item.tm.MatTOId.ToString();
                                    satTOItem.Type = "Material";
                                    satTOItem.Quantity = item.tm.Quantity;
                                    // satTOItem.SATTO = satTo;
                                    sum += satTOItem.Price;
                                    SumMaterials += satTOItem.Price;
                                    satTo.SATTOItems.Add(satTOItem);
                                    #endregion
                                }
                            }
                        }


                        satTo.Total = sum;
                        satTo.TotalMaterials = SumMaterials;
                        satTo.TotalServices = sumService;
                        satTo.CreateUserName = User.Identity.Name;
                        satTo.CreateUserDate = DateTime.Now;

                        context.SATTOs.Add(satTo);
                        try
                        {
                            context.SaveChanges();
                        }

                        catch (Exception exc)
                        {
                            result.Success = false;
                            result.Message = "Ошибка при сохранении в бд";
                            result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);
                            return Json(result);
                        }
                        result.Success = true;
                        result.Message = string.Format("Итоговая сумма {0}. Информация будет загружена в СХ в ближайшее время ", sum.ToString("G29"));


                        return Json(result);

                    }

                }
            }

            //}
            return Json(result);

        }


        public ActionResult TODocs()
        {
            return View();
        }

        [HttpPost]
        public JsonResult TODocsList()
        {
            using (Context context = new Context())
            {
                TORepository repository = new TORepository(context);
                var result = repository.GetLastSATTOList().Select(s => new
                {
                    Id = s.Id,
                    CreateUserDate = s.CreateUserDate,
                    CreateUserName = s.CreateUserName,
                    TO = s.TO,
                    SubContractor = s.SubContractor,
                    Total = s.Total,
                    UploadedToSh = s.UploadedToSh,
                    ShComment = s.ShComment


                }).ToList();







                //OrderByDescending(pors => pors.PrintDate).ToList().Select(por => new
                //{
                //    Id = por.Id,
                //    PrintDate = por.PrintDate,
                //    UserName = por.UserName,
                //    SubContractorName = por.SubContractorName,
                //    WorkStart = por.WorkStart,
                //    WorkEnd = por.WorkEnd,
                //    Project = por.Project.Name,
                //    //Status = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? "" : por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().Status.Name,
                //    // StatusDate = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? null : new DateTime?(por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().StatusDate),
                //    AVR = (por is AVRPOR) ? ((AVRPOR)por).AVRId : ""
                //});
                return Json(new { data = result, total = result.Count() });
            }
        }



        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]

        public JsonResult GetTODataById(string TOId)
        {
            using (Context context = new Context())
            {

                ShTO shTO = context.ShTOes.FirstOrDefault(p => p.TO == TOId);

                if (shTO != null)
                {
                    var result = new TOViewModel
                    {

                    };

                    result.POIssueDate = shTO.POIssueDate;
                    if (!string.IsNullOrEmpty(shTO.Subcontractor))
                    {
                        var Subc = context.SubContractors.FirstOrDefault(s => s.ShName == shTO.Subcontractor);
                        if (Subc == null)
                        {
                            return Json(new { Status = "error", Message = string.Format("Подрядчик {0} не найден. Проверьте наличие прайс-листа для подрядчика и наименование подрядчика в СХ.", shTO.Subcontractor), Brunch = "testBrunch" });
                        }
                        result.Subcontractor = Subc.SAPName;
                        var shContact = context.ShContacts.FirstOrDefault(c => c.Contact == Subc.ShName);
                        if (shContact != null)
                        {
                            result.WOVAT = shContact.WithOutVAT;
                        }

                    }
                    else
                    {
                        return Json(new { Status = "error", Message = string.Format("Для ТО не задан подрядчик") });
                    }
                    TORepository repository = new TORepository(context);
                    var activity = shTO.ActivityCode;//repository.GetToActivity(shTO.TO);
                    if (activity == null)
                    {
                        return Json(new { Status = "error", Message = string.Format("Не удалось найти активность ТО") });
                    }
                    else
                    {
                        result.Activity = shTO.ActivityCode; // .Activity;
                    }

                    int itemId = 0;

                    var ItemList = new List<TORepository.TOItemViewModel>();
                    //if (shTO.EquipmentTO)
                    //{
                    //    ItemList = repository.GetTOEquipmentItemModels(shTO.TO).ToList();
                    //}
                    //else
                    {
                        ItemList = repository.GetTOItemModels(shTO.TO).ToList();
                    }

                    var MatItems = repository.GetTOMatItemModels(shTO.TO);
                    if (ItemList == null || ItemList.Count() == 0)
                    {
                        return Json(new { Status = "error", Message = string.Format("К ТО не привязаны айтемы") });
                    }
                    var regionGroup = ItemList.GroupBy(i => i.SiteRegion);
                    var branchGroup = ItemList.GroupBy(i => i.SiteBranch);
                    //if (regionGroup.Count() > 1)
                    //{
                    //    return Json(new { Status = "error", Message = string.Format("Сайты привязаны к разным регионам:{0}", string.Join(",", regionGroup.Select(g => g.Key))) });
                    //}
                    if (regionGroup.Count() == 0)
                    {
                        return Json(new { Status = "error", Message = string.Format("Сайты не привязаны к регионам:{0}", string.Join(",", ItemList.Select(g => g.Site))) });
                    }

                    if (regionGroup.Count() == 0)
                    {
                        return Json(new { Status = "error", Message = string.Format("Сайты не привязаны к филиалам:{0}", string.Join(",", ItemList.Select(g => g.Site))) });
                    }
                    //if (branchGroup.Count() > 1)
                    //{
                    //    return Json(new { Status = "error", Message = string.Format("Сайты привязаны к разным филиалам:{0}", string.Join(",", branchGroup.Select(g => g.Key))) });
                    //}
                    if (ItemList.Any(i => !i.SiteQuantity.HasValue))
                    {
                        var withoutQuant = ItemList.Where(i => !i.SiteQuantity.HasValue).Select(i => i.TOItem);
                        return Json(new { Status = "error", Message = string.Format("Не для всех элементов проставлено количество. ID:{0}", string.Join(", ", withoutQuant)) });
                    }
                    if (ItemList.Any(i => !i.TOPlanDate.HasValue))
                    {
                        var withoutPlanDate = ItemList.Where(i => !i.TOPlanDate.HasValue).Select(i => i.TOItem);
                        return Json(new { Status = "error", Message = string.Format("Не для всех элементов проставлена плановая дата. ID:{0} ", string.Join(", ", withoutPlanDate)) });
                    }

                    result.Type = shTO.TOType;
                    result.Region = regionGroup.FirstOrDefault().Key;
                    result.Brunch = branchGroup.FirstOrDefault().Key;
                    result.Items = ItemList;
                    if (MatItems != null)
                        result.MatItems = MatItems.ToList();
                    //if (items.Where(i => i.PricePerItemSH == null || i.Quantity == null).Count() > 0)
                    //{
                    //    return Json(new { Status = "error", Message = "Для позиций не указана цена или количество. Проверьте данные в СХ." });
                    //}
                    // result.PriceTotalSH = Extentions.FinanceRound((decimal)items.Sum(i => (decimal?)i.PricePerItemSH.Value * i.Quantity), 2);
                    //   result.PriceTotal = Extentions.FinanceRound((decimal)items.Where(p => p.ECRType != null).Sum(i => (decimal?)i.PricePerItemSH.Value * i.Quantity), 2);
                    //  result.Items.AddRange(items);
                    //foreach (var item in result.Items)
                    //{
                    //    item.Id = itemId;
                    //    itemId++;
                    //}
                    if (result.Items.Count > 0)
                    {
                        return Json(result);
                    }
                    //У пора нет позиций
                    return Json(new { Status = "error", Message = "У TO нет позиций. Проверьте данные в СХ." });
                }
                //Пор не найден
                return Json(new { Status = "error", Message = "TO не найден. Проверьте данные в СХ." });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache)]
        // [HttpErrorHandler]
        public ActionResult GetTODataByIdGet(string TOId)
        {
            var to = HttpUtility.UrlDecode(TOId);
            var result = GetTODataById(to);
            //if (result.Data is Intranet.Models.TOViewModel)
            //{
            return Json(((JsonResult)result).Data, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            //    {
            //        Content = new StringContent("asdf"),//string.Format(result.Data.ToString())),
            //        ReasonPhrase = "Product ID Not Found"

            //    };
            //    Session["obj"] = result;
            //    throw new System.Web.Http.HttpResponseException(resp);
            //}
        }


        public ActionResult Print(int Id)
        {

            using (Context context = new Context())
            {
                var satTo = context.SATTOs.Find(Id);
                if (satTo != null)
                {


                    try
                    {
                        var file = ExcelParser.EpplusInteract.CreateTORequest.CreateTORequestFile(Id);
                        if (file != null)
                        {
                            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("TOR-{0}-{2}-{1}.xlsm", satTo.TO, satTo.CreateUserDate.ToString("yyyyMMddHHmmss"), Id));
                            return File(file, ".xlsx");
                        }
                    }
                    catch (Exception exc)
                    {
                        return View("~/Views/Shared/Error.cshtml");
                    }
                }


            }

            return View("~/Views/Shared/Error.cshtml");

            // return File(ExcelParser.EpplusInteract.CreatePor.CreatePorFile(Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POR-" + Id.ToString() + DateTime.Now.ToString("(yyyyMMddHHmmss)"));
        }


        public ActionResult PrintPOR(int Id)
        {

            using (Context context = new Context())
            {
                var satTo = context.SATTOs.Find(Id);
                if (satTo != null)
                {


                    try
                    {
                        var file = ExcelParser.EpplusInteract.CreateTOPOR.CreatePorFile(Id);
                        if (file != null)
                        {
                            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("POR-{0}-{2}-{1}.xlsx", satTo.TO, satTo.CreateUserDate.ToString("yyyyMMddHHmmss").CUnidecode(), Id));
                            return File(file, ".xlsx");
                        }
                    }
                    catch (Exception exc)
                    {
                        return View("~/Views/Shared/Error.cshtml");
                    }
                }


            }

            return View("~/Views/Shared/Error.cshtml");

            // return File(ExcelParser.EpplusInteract.CreatePor.CreatePorFile(Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POR-" + Id.ToString() + DateTime.Now.ToString("(yyyyMMddHHmmss)"));
        }
    }
}