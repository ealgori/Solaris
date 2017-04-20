using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.Models;
using ExcelParser.EpplusInteract;
using Intranet.ActionFilters;
using System.Reflection;
using DbModels.DataContext;
using DbModels.Repository;
using DbModels.Models.Pors;
using DbModels.Models;
using DbModels.DomainModels.Solaris.Pors;
using CommonFunctions.Extentions;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext.Repositories;
using DbModels.AVRConditions;

namespace Intranet.Controllers
{
    public class PORController : Controller
    {
        //
        // GET: /POR/

        public ActionResult Index()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            breadCrumbs.Add(new BreadCrumbModel() { Name = "POR's" });

            ViewBag.Path = breadCrumbs;
            return View();
        }
        [HttpPost]
        public JsonResult Read()
        {
            using (Context context = new Context())
            {

               // context.Database.Log += (l) => { System.Diagnostics.Debug.WriteLine(l); };
                var cachedShAVR = context.ShAVRs.Where(AVRRepository.Base).ToList();
                var pors = context.PORs.OrderByDescending(p => p.PrintDate).ToList().Select(por => new
                SATPorModel
                {
                    Id = por.Id,
                    PrintDate = por.PrintDate,
                    UserName = por.UserName,
                    SubContractorName = por.SubContractorName,
                    WorkStart = por.WorkStart,
                    WorkEnd = por.WorkEnd,
                    Project = por.Project.Name,
                    //Status = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? "" : por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().Status.Name,
                    // StatusDate = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? null : new DateTime?(por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().StatusDate),
                    AVR = (por is AVRPOR) ? ((AVRPOR)por).AVRId : "",
                    Status= null 
                   
                }).ToList();
                var result = new List<SATPorModel>();
                foreach (var por in pors.OrderByDescending(p=>p.PrintDate).ToList())
                {
                    if (!string.IsNullOrEmpty(por.AVR))
                    {
                        var shAvr = cachedShAVR.FirstOrDefault(a => a.AVRId == por.AVR);
                        if (shAvr != null)
                            if (shAvr.PorAccesible)
                                result.Add(por);
                    }
                    else
                    {
                        result.Add(por);
                    }

                }
                //var avrs = context.ShAVRs.Where(AVRRepository.Base).ToList();
                //foreach (var por in result)
                //{
                //    if (!string.IsNullOrEmpty(por.AVR))
                //    {
                //        var porAVR = avrs.FirstOrDefault(p => p.AVRId == por.AVR);
                //        if (porAVR != null)
                //        {
                //            if (porAVR.Status== Statuses.NeedVCPrice)
                //            {
                //                var VCrequest = porAVR.ShVCRequests.ToList();
                //                if (VCrequest.Count > 0)
                //                {
                //                    por.Status = string.Format("Запросы:{0}", string.Join(",", VCrequest.Select(s => s.Id).ToList()));
                //                }
                //                else
                //                {
                //                    por.Status = string.Format("Запросы в вк еще не отправлены");
                //                }
                //            }

                //        }
                //    }


                

                return Json(new { data = result, total = result.Count() });
        }
        }
        public ActionResult Create()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            breadCrumbs.Add(new BreadCrumbModel() { Name = "POR's", Path = Url.Action("Index", "POR", null, Request.Url.Scheme) });
            breadCrumbs.Add(new BreadCrumbModel() { Name = "Create POR" });
            ViewBag.Path = breadCrumbs;
            PORViewModel model = new PORViewModel();
            return View(model);


        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult NewCreate()
        {
            return View();
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public ActionResult NewCreateDemos()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewCreate(NewPORViewModel model)
        {
            CreatePORResultViewModel result = new CreatePORResultViewModel();
            if (!model.WorkStart.HasValue || !model.WorkEnd.HasValue)
            {
                result.Success = false;
                result.Message = "Дата начала и дата окончания должны быть заданы";
                result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);

            }
            else
            {
                if (model != null &&
                    model.Items != null &&
                    model.Items.Count(ic => ic.Quantity > 0) > 0)
                {
                    using (Context context = new Context())
                    {
                        var project = context.Projects.FirstOrDefault(pr => pr.Id == model.ProjectId);
                        if (project != null)
                        {
                            var subContractor = context.SubContractors.FirstOrDefault(sc => sc.Id == model.SubcId);
                            if (subContractor != null)
                            {
                                // фиктивный нетворк. еще не решили как делать
                                var network =  model.Network;
                                // Нетворк пока что не проверяем по причине того, что его может не быть в наличии, пока структура не заказана черзе мус.
                                //if (network != null)
                                {
                                    try
                                    {

                                        var shAVR = context.ShAVRs.FirstOrDefault(a=>a.AVRId == model.AVRId);
                                        if (shAVR != null)
                                        {
                                            if (shAVR.TotalAmount > 50000)
                                            {
                                                if (string.IsNullOrEmpty(shAVR.RukRegionApproval))
                                                {
                                                    result.Success = false;
                                                    result.Message = "Региональный руководитель должен заапрувить данный АВР";
                                                    return Json(result);
                                                }
                                            }
                                        }
                                        AVRPOR por = new AVRPOR();
                                        por.AVRId = model.AVRId;
                                        por.Network = network;
                                        // фиктивная активность. еще не решили как делать
                                        var activity = model.Activity;
                                        if (activity == null)
                                        {
                                            result.Success = false;
                                            result.Message = "Ошибка при создании пора: Не найдена указанная активность";
                                            return Json(result);
                                        }
                                        por.Activity = activity; //model.Activity;
                                        // por.PORActivity = activity;
                                       // por.POType = activity.POType;
                                        por.WorkStart = model.WorkStart.Value;
                                        por.WorkEnd = model.WorkEnd.Value;
                                        var modelPositionsIds = model.Items.Select(it => it.PositionId);
                                        // выбрать не все айтемы, а с наивысшим ид
                                        var items = context.PriceListRevisionItems
                                            .Where(plri =>
                                               modelPositionsIds.Contains(plri.Id));

                                        var subItems = from i in items
                                                       group i by i.SAPCode into _g
                                                       select _g.Max(i => i.Id);
                                        items = items.Where(it => subItems.Contains(it.Id));
                                        var priceListRevisions = items.Select(it => it.PriceListRevision).ToList();
                                        var priceLists = priceListRevisions.Select(plr => plr.PriceList);
                                        por.PriceListRevisions = priceListRevisions;

                                        por.UserName = User.Identity.Name;

                                        por.PriceListNumbers = string.Join("; ", priceLists.Distinct().Select(pl => pl.PriceListNumber +
                                            (string.IsNullOrEmpty(pl.PriceListAdditionalNumber) ?
                                            "" :
                                            "-" + pl.PriceListAdditionalNumber)
                                            ));

                                        por.Project = project;
                                        por.PrintDate = DateTime.Now;

                                        por.SubContractor = subContractor;
                                        por.SubContractorAddress = subContractor.Address;
                                        por.SubContractorName = subContractor.Name;
                                        por.SubContractorSAPNumber = subContractor.SAPNumber;
                                        context.AVRPORs.Add(por);
                                        int count = 0;
                                        PriceListRepository priceListRepository = new PriceListRepository(context);
                                        var ecrAddCount = model.Items.Where(i => i.PositionId == -1).Count();
                                        if (items.Count() == 0 && ecrAddCount == 0)
                                        {
                                            result.Success = false;
                                            result.Message = "Укажите хотя бы один айтем";
                                            return Json(result);

                                        }
                                        foreach (var itemMod in model.Items)
                                        {
                                            PriceListRevisionItem item = null;
                                            
                                            item = context.PriceListRevisionItems.FirstOrDefault(it => it.Id == itemMod.PositionId);
                                            #region Корренктный айтем

                                            if (item != null)
                                            {
                                                if (!priceListRepository.CheckIfPriceListItemActive(subContractor.Id, project.Id, model.WorkStart, model.WorkEnd, item.Id))
                                                {
                                                    result.Success = false;
                                                    result.Message = string.Format("Айтем {0} не активен в данный момент времени:{1}-{2}", item.Name, model.WorkStart.Value.ToShortDateString(), model.WorkEnd.Value.ToShortDateString());
                                                    return Json(result);
                                                }
                                                count++;
                                                PORItem porItem = new PORItem();
                                                porItem.PriceListRevisionItem = item;
                                                porItem.No = count;
                                                porItem.Cat = "Service";
                                                porItem.Code = item.SAPCode.Code;
                                                porItem.Plant = "2349";
                                                //var modelItem = model.Items.FirstOrDefault(it => it.PositionId == item.SAPCode.Id);
                                                porItem.NetQty = itemMod.Quantity;
                                                porItem.ItemCat = "N";
                                                porItem.PRtype = "3";
                                                porItem.POrg = "1439";
                                                porItem.GLacc = "402601";
                                                porItem.Price = item.Price * itemMod.Koeff ?? 1;
                                                porItem.Curr = "RUB";
                                                porItem.PRUnit = "1";
                                                porItem.Vendor = subContractor.SAPNumber;
                                                porItem.Plandate = model.WorkEnd.Value;
                                                porItem.Description = item.Name;
                                                porItem.POR = por;
                                                porItem.Site = itemMod.Site;
                                                porItem.FIX = itemMod.SiteFix;
                                                porItem.FOL = itemMod.SiteFol;
                                                porItem.Coeff = itemMod.Koeff;
                                                porItem.ItemId = itemMod.AVRItemId.Value;

                                                var controlPrice = (porItem.Price * porItem.NetQty);
                                                if ((controlPrice!=controlPrice.FinanceRound())
                                                    || porItem.Price!= porItem.Price.FinanceRound()
                                                    || porItem.NetQty != porItem.NetQty.FinanceRound()

                                                    )
                                                {
                                                    porItem.NetQty = 1;
                                                    porItem.Price = controlPrice.FinanceRound();
                                                }
                                               



                                                context.PORItems.Add(porItem);
                                            }
                                            #endregion
                                            else
                                            {
                                                #region ECRADD
                                                if (itemMod.PositionId == -1)
                                                {
                                                    count++;
                                                    var ECRItem = context.ShAVRItems.FirstOrDefault(i => i.AVRItemId == itemMod.AVRItemId);
                                                    if (ECRItem == null)
                                                    {
                                                        result.Success = false;
                                                        result.Message = string.Format("Не удалось найти позицию ECR Add с номером {0}. Обратитесь в MTS SH Support", itemMod.AVRItemId);
                                                        return Json(result);
                                                    }
                                                    PORItem porItem = new PORItem();
                                                    //porItem.PriceListRevisionItem = item;
                                                    porItem.No = count;
                                                    porItem.Cat = "Service";
                                                    porItem.Code = "ECR-SOLA-ADD";
                                                    porItem.Plant = "2349";
                                                    //var modelItem = model.Items.FirstOrDefault(it => it.PositionId == item.SAPCode.Id);
                                                    porItem.NetQty = itemMod.Quantity;
                                                    porItem.ItemCat = "N";
                                                    porItem.PRtype = "3";
                                                    porItem.POrg = "1439";
                                                    porItem.GLacc = "402601";
                                                    porItem.Price = itemMod.PricePerItemSH * itemMod.Koeff ?? 1;
                                                    porItem.Curr = "RUB";
                                                    porItem.PRUnit = "1";
                                                    porItem.Vendor = subContractor.SAPNumber;
                                                    porItem.Plandate = model.WorkStart.Value;
                                                    porItem.Description = ECRItem.Description;
                                                    porItem.ItemId = itemMod.AVRItemId.Value;
                                                    porItem.POR = por;

                                                    context.PORItems.Add(porItem);
                                                }
                                                #endregion
                                                else
                                                {

                                                    result.Success = false;
                                                    result.Message = string.Format("Некоректные данные:Id:{0}", itemMod.PositionId.HasValue ? itemMod.PositionId.Value.ToString() : "null");
                                                    return Json(result);
                                                }
                                            }
                                        }
                                        context.SaveChanges();
                                        result.Success = true;

                                        var porAccesibleCondition = new PORAccessibleCondition(new NeedPriceCondition());
                                        if (porAccesibleCondition.IsSatisfy(shAVR, context))
                                        {
                                            shAVR.PorAccesible = true;
                                            result.Message = "POR успешно создан.";
                                            result.Url = Url.Action("PrintPor", "POR", new { Id = por.Id }, Request.Url.Scheme);
                                        }
                                        else
                                        {
                                            result.Message = "Опрайсовка проведена. Пор будет подготовлен через некоторое время. (Нужны запросы к вк и нетворк)";
                                        }
                                      
                                    }
                                    catch (Exception exc)
                                    {
                                        result.Success = false;
                                        result.Message = "Ошибка при создании пора:" + exc.Message;
                                    }
                                }
                                //else
                                //{
                                //    result.Success = false;
                                //    result.Message = "Город в базе данных не найден";
                                //}
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "Такого подрядчика не существует";
                            }

                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Такого проекта не существует";
                        }

                    }

                }
            }
            return Json(result);
        }
        /// <summary>
        /// Получение списка позиций по номеру пора
        /// </summary>
        /// <param name="AVRId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetAVRDataById(string AVRId)
        {
            using (Context context = new Context())
            {

                ShAVRs por = context.ShAVRs.FirstOrDefault(p => p.AVRId == AVRId);
                //if (por == null)
                //{
                //    por = context.ShAVRf.FirstOrDefault(p => p.AVRId == AVRId);
                //}
                if (por != null)
                {
                    var result = new NewPORViewModel
                    {
                        WorkEnd = por.WorkEnd,
                        WorkStart = por.WorkStart
                    };
                    if (!string.IsNullOrEmpty(por.Subcontractor) && !string.IsNullOrEmpty(por.Project))
                    {
                        var Subc = context.SubContractors.FirstOrDefault(s => s.ShName == por.Subcontractor);
                        //var Network = context.PORNetworks.FirstOrDefault(s => s.City == por.Subregion);
                        //if (Network == null)
                        //{
                        //    return Json(new { Status = "error", Message = string.Format("Нетворк для региона {0} не найден. Проверьте данные в СХ.", por.Subregion) });
                        //}

                        result.Network = por.Network;
                        result.Activity = por.ActivityCode;
                       
                        //result.NetworkId = 1;
                        var Project = context.Projects.FirstOrDefault(p => p.Name == por.Project);
                        if (Subc == null)
                        {
                            return Json(new { Status = "error", Message = string.Format("Подрядчик {0} не найден. Проверьте наличие прайс-листа для подрядчика и наименование подрядчика в СХ.", por.Subcontractor) });
                        }
                        result.SubcId = Subc.Id;
                        result.Subcontractor = Subc.Name;
                        result.SubRegion = por.Subregion;
                        if (Project == null)
                        {
                            return Json(new { Status = "error", Message = string.Format("Проект {0} не найден. Проверьте данные в СХ.", por.Project) });
                        }
                        result.ProjectId = Project.Id;
                    }
                    int itemId = 0;
                    var ItemList = por.Items;// context.ShAVRItems.Where(p => p.AVRFId == por.AVRId || p.AVRSId == por.AVRId).ToList();
                    if (ItemList.Where(p => !string.IsNullOrEmpty(p.ECRType) && p.ECRApprove != (string)DbModels.Constants.ECRApprove).Count() > 0)
                    {
                        return Json(new { Status = "error", Message = "Не все прайсовые позиции одобрены отделом Сорсинга." });
                    }
                    var items = ItemList.Select(p => new NewPORItemViewModel
                    {
                        Site = p.SiteId,
                        SiteFix = p.FIXId,
                        SiteFol = p.FOLId,
                        SHDescription = p.Description,
                        PricePerItemSH = p.Price,
                        PricePerItem = 0,
                        Quantity = p.Quantity.HasValue ? p.Quantity.Value : 0,
                        PriceSH = p.Price * (p.Quantity.HasValue ? p.Quantity.Value : 0),
                        Unit = p.Unit,
                        PositionId = p.ECRType == null ? null : (int?)DbModels.Constants.ECRAddId,
                        ECRType = p.ECRType,
                        AVRItemId = p.AVRItemId
                    });
                    if (items.Where(i => i.PricePerItemSH == null || i.Quantity == null).Count() > 0)
                    {
                        return Json(new { Status = "error", Message = "Для позиций не указана цена или количество. Проверьте данные в СХ." });
                    }
                    result.PriceTotalSH = Extentions.FinanceRound((decimal)items.Sum(i => (decimal?)i.PricePerItemSH.Value * i.Quantity), 2);
                    result.PriceTotal = Extentions.FinanceRound((decimal)items.Where(p => p.ECRType != null).Sum(i => (decimal?)i.PricePerItemSH.Value * i.Quantity), 2);
                    result.Items.AddRange(items);
                    foreach (var item in result.Items)
                    {
                        item.Id = itemId;
                        itemId++;
                    }
                    if (result.Items.Count > 0)
                    {
                        return View("_PorItemList", result);
                    }
                    //У пора нет позиций
                    return Json(new { Status = "error", Message = "У АВРа нет позиций. Проверьте данные в СХ." });
                }
                //Пор не найден
                return Json(new { Status = "error", Message = "АВР не найден. Проверьте данные в СХ." });
            }
        }
        public ActionResult PrintPor(int Id)
        {
            using (Context context = new Context())
            {
                var por = context.PORs.Find(Id);
                if (por != null)
                {
                    try
                    {
                        var porBytes = ExcelParser.EpplusInteract.CreatePor.CreatePorFile(Id);
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("POR-{0}-{1}-{2}.xlsx", por.Network, por.PrintDate.ToString("yyyyMMddHHmmss"), Id));
                        return File(porBytes, ".xlsx");
                    }
                    catch (Exception exc)
                    {

                        return Content(string.Format("{0}", exc.Message));
                    }
                   
                }

            }

            return null;

            // return File(ExcelParser.EpplusInteract.CreatePor.CreatePorFile(Id), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "POR-" + Id.ToString() + DateTime.Now.ToString("(yyyyMMddHHmmss)"));
        }
        /// <summary>
        /// Определение полной стоимости одной позиции
        /// Используется для вывода на форму
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult GetCodeTotalPrice(PorCodeCalcModel model)
        {
            using (Context context = new Context())
            {
                try
                {
                    PriceListRepository repo = new PriceListRepository(context);
                    //var price = repo.GetActivePrice(model.SubcId, model.ProjectId, model.WorkStart, model.WorkEnd, model.SapCodeId, model.Quantity, model.Coeff);
                    // сап код ид, это не сап код ид, а прайслистревижионайтмемИД
                    PriceListRevisionItem item = context.PriceListRevisionItems.FirstOrDefault(plri => plri.Id == model.PriceListId);
                    if (item == null)
                    {
                        return Json(0);
                    }
                    var price = Extentions.FinanceRound(Extentions.FinanceRound(item.Price * model.Coeff, 2) * model.Quantity, 2);
                    return Json(price);
                }
                catch (Exception ex)
                {
                    return Json(0);
                }
            }
        }
        [HttpPost]
        public JsonResult GetCodeTotalPriceAndPricePerItem(PorCodeCalcModel model)
        {
            using (Context context = new Context())
            {
                try
                {
                    PriceListRepository repo = new PriceListRepository(context);
                    //var price = repo.GetActivePrice(model.SubcId, model.ProjectId, model.WorkStart, model.WorkEnd, model.SapCodeId, model.Quantity, model.Coeff);
                    // сап код ид, это не сап код ид, а прайслистревижионайтмемИД
                    var item = context.PriceListRevisionItems.FirstOrDefault(plri => plri.Id == model.PriceListId);
                    if (item == null)
                    {
                        return Json(new { price = 0, pricePerItem = 0 });
                    }
                    var price = Extentions.FinanceRound(Extentions.FinanceRound(item.Price * model.Coeff, 2) * model.Quantity, 2);
                    return Json(new { price = price, pricePerItem = item.Price });
                }
                catch (Exception ex)
                {
                    return Json(new { price = 0, pricePerItem = 0 });
                }
            }
        }
        [HttpPost]
        public JsonResult CreatePor(PORViewModel model)
        {
            CreatePORResultViewModel result = new CreatePORResultViewModel();
            if (!model.WorkStart.HasValue || !model.WorkEnd.HasValue)
            {
                result.Success = false;
                result.Message = "Дата начала и дата окончания должны быть заданы";
                result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);

            }
            else
            {

                if (model.WorkStart.Value.Year != model.WorkEnd.Value.Year)
                {
                    result.Success = false;
                    result.Message = "Дата начала и дата окончания должны находится в одном и том же году";
                    result.Url = Url.Action("Index", "Home", new { Id = 1 }, Request.Url.Scheme);

                }
                else
                {

                    if (model != null &&
                        model.Items != null &&
                        model.Items.Count(ic => ic.Quantity > 0) > 0)
                    {
                        using (Context context = new Context())
                        {
                            var project = context.Projects.FirstOrDefault(pr => pr.Id == model.ProjectId);
                            if (project != null)
                            {
                                var subContractor = context.SubContractors.FirstOrDefault(sc => sc.Id == model.SubcId);
                                if (subContractor != null)
                                {
                                    var network = model.Network;
                                    if (network != null)
                                    {
                                        try
                                        {
                                            POR por = new POR();
                                            por.Network = network;
                                            //var activity = context.PORActivities.First(act => act.Id == model.Activity.Value);
                                            por.Activity = model.Activity; //model.Activity;
                                            // por.PORActivity = activity;
                                           // por.POType = activity.POType;
                                            por.WorkStart = model.WorkStart.Value;
                                            por.WorkEnd = model.WorkEnd.Value;
                                            var modelPositionsIds = model.Items.Select(it => it.PositionId);
                                            // выбрать не все айтемы, а с наивысшим ид
                                            var items = context.PriceListRevisionItems
                                                .Where(plri =>
                                                   modelPositionsIds.Contains(plri.Id));

                                            var subItems = from i in items
                                                           group i by i.SAPCode into _g
                                                           select _g.Max(i => i.Id);
                                            items = items.Where(it => subItems.Contains(it.Id));
                                            var priceListRevisions = items.Select(it => it.PriceListRevision).ToList();
                                            var priceLists = priceListRevisions.Select(plr => plr.PriceList);
                                            por.PriceListRevisions = priceListRevisions;

                                            por.UserName = User.Identity.Name;

                                            por.PriceListNumbers = string.Join("; ", priceLists.Distinct().Select(pl => pl.PriceListNumber +
                                                (string.IsNullOrEmpty(pl.PriceListAdditionalNumber) ?
                                                "" :
                                                "-" + pl.PriceListAdditionalNumber)
                                                ));

                                            por.Project = project;
                                            por.PrintDate = DateTime.Now;

                                            por.SubContractor = subContractor;
                                            por.SubContractorAddress = subContractor.Address;
                                            por.SubContractorName = subContractor.Name;
                                            por.SubContractorSAPNumber = subContractor.SAPNumber;
                                            context.PORs.Add(por);
                                            int count = 0;
                                            PriceListRepository priceListRepository = new PriceListRepository(context);
                                            if (items.Count() == 0)
                                            {
                                                result.Success = false;
                                                result.Message = "Укажите хотя бы один айтем";
                                                return Json(result);

                                            }
                                            foreach (var itemMod in model.Items)
                                            {
                                                PriceListRevisionItem item = null;

                                                item = context.PriceListRevisionItems.FirstOrDefault(it => it.Id == itemMod.PositionId);
                                                #region Корренктный айтем

                                                if (item != null)
                                                {
                                                    if (!priceListRepository.CheckIfPriceListItemActive(subContractor.Id, project.Id, model.WorkStart, model.WorkEnd, item.Id))
                                                    {
                                                        result.Success = false;
                                                        result.Message = string.Format("Айтем {0} не активен в данный момент времени:{1}-{2}", item.Name, model.WorkStart.Value.ToShortDateString(), model.WorkEnd.Value.ToShortDateString());
                                                        return Json(result);
                                                    }
                                                    count++;
                                                    PORItem porItem = new PORItem();
                                                    porItem.PriceListRevisionItem = item;
                                                    porItem.No = count;
                                                    porItem.Cat = "Service";
                                                    porItem.Code = item.SAPCode.Code;
                                                    porItem.Plant = "2349";
                                                    //var modelItem = model.Items.FirstOrDefault(it => it.PositionId == item.SAPCode.Id);
                                                    porItem.NetQty = itemMod.Quantity;
                                                    porItem.ItemCat = "N";
                                                    porItem.PRtype = "3";
                                                    porItem.POrg = "1439";
                                                    porItem.GLacc = "402601";
                                                    porItem.Price = item.Price * itemMod.Koeff ?? 1;
                                                    porItem.Curr = item.Currency;
                                                    porItem.PRUnit = "1";
                                                    porItem.Vendor = subContractor.SAPNumber;
                                                    porItem.Plandate = model.WorkEnd.Value;
                                                    porItem.Description = item.Name;
                                                    porItem.POR = por;
                                                    porItem.ItemId = itemMod.PositionId.Value;
                                                    context.PORItems.Add(porItem);
                                                }
                                                #endregion
                                                else
                                                {
                                                    //#region ECRADD
                                                    //if (itemMod.PositionId == -1)
                                                    //{
                                                    //    count++;
                                                    //    PORItem porItem = new PORItem();
                                                    //    //porItem.PriceListRevisionItem = item;
                                                    //    porItem.No = count;
                                                    //    porItem.Cat = "Service";
                                                    //    porItem.Code = item.SAPCode.Code;
                                                    //    porItem.Plant = "2349";
                                                    //    //var modelItem = model.Items.FirstOrDefault(it => it.PositionId == item.SAPCode.Id);
                                                    //    porItem.NetQty = itemMod.Quantity;
                                                    //    porItem.ItemCat = "N";
                                                    //    porItem.PRtype = "3";
                                                    //    porItem.POrg = "1439";
                                                    //    porItem.GLacc = "402601";
                                                    //    porItem.Price = itemMod. * itemMod.Koeff ?? 1;
                                                    //    porItem.Curr = item.Currency;
                                                    //    porItem.PRUnit = "1";
                                                    //    porItem.Vendor = subContractor.SAPNumber;
                                                    //    porItem.Plandate = model.WorkStart.Value;
                                                    //    porItem.Description = item.Name;
                                                    //    porItem.POR = por;

                                                    //    context.PORItems.Add(porItem);
                                                    //}
                                                    //#endregion
                                                    //else
                                                    //{

                                                    result.Success = false;
                                                    result.Message = string.Format("Некоректные данные:Id:{1}", itemMod.PositionId.HasValue ? itemMod.PositionId.Value.ToString() : "null");
                                                    return Json(result);
                                                    //}
                                                }
                                            }
                                            context.SaveChanges();
                                            result.Success = true;
                                            result.Message = "POR успешно создан";
                                            result.Url = Url.Action("PrintPor", "POR", new { Id = por.Id }, Request.Url.Scheme);
                                        }
                                        catch (Exception exc)
                                        {
                                            result.Success = false;
                                            result.Message = "Ошибка при создании пора:" + exc.Message;
                                        }
                                    }
                                    else
                                    {
                                        result.Success = false;
                                        result.Message = "Город в базе данных не найден";
                                    }
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "Такого подрядчика не существует";
                                }

                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "Такого проекта не существует";
                            }
                        }
                    }

                }
            }
            return Json(result);
        }

        [HttpPost]
        [OnlyAdmin]
        public int DeletePOR(int Id)
        {
            using (Context context = new Context())
            {
                var por = context.PORs.FirstOrDefault(p => p.Id == Id);
                if (por != null)
                {
                    var porItems = context.PORItems.Where(pi => pi.POR.Id == Id);
                    foreach (var item in porItems)
                    {
                        context.PORItems.Remove(item);
                    }
                    context.PORs.Remove(por);
                    context.SaveChanges();
                    return 1;
                }
                return 0;
            }
        }

        public ActionResult DelPorDrafts()
        {
            using(Context context = new Context())
            {
                var agreements = context.ShAddAgreements.Select(s => new AddAgreementModel() {  Name=s.AddAgreement, }).ToList();
                return View(agreements);
            }

           
        }

        public ActionResult PrintDraftPorDel(string addAgreement)
        {
            string error;
            var pordelBytes = ExcelParser.EpplusInteract.CreatePorDel.GenerateDelPOR(addAgreement,false, out error);
            if (pordelBytes != null)
            {

                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("DEL-POR-{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");

                return File(pordelBytes, ".xlsx");
            }
            else
                return Content(error);
        }
        public ActionResult PrintDraftTorDel(string addAgreement)
        {
            string error;
            var pordelBytes = ExcelParser.EpplusInteract.CreateTORequestDel.Create(addAgreement, true, out error);
            if (pordelBytes != null)
            {
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("DEL-TOR{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                return File(pordelBytes, ".xlsx");
            }
            else
                return Content(error);
        }
    }
}
