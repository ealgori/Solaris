using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Intranet.Models;


using DbModels.DataContext;
using DbModels.Repository;
using Intranet.ActionFilters;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext.Repositories;
using CommonFunctions.Extentions;

namespace Intranet.Controllers
{
    public class JsonController : Controller
    {
        //
        // GET: /Json/

        public ActionResult Index()
        {
            return View();
        }
          [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult ProjectList()
        {
            using (Context context = new Context())
            {
                var list = context.Projects.Select(s => new
                {
                    text = s.Name,
                    value = s.Id
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
          [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult SubcList()
        {
            using (Context context = new Context())
            {
                var list = context.SubContractors.Select(s => new
                {
                    text = s.Name,
                    value = s.Id
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
          [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult NetworkList()
        {
            using (Context context = new Context())
            {
                var list = context.PORNetworks.Select(s => new
                {
                    text = s.City,
                    value = s.Id
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
          [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult ActivityList()
        {
            using (Context context = new Context())
            {
                var list = context.PORActivities.Select(s => new
                {
                    text = s.POType,
                    value = s.Id
                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

          [CacheControl(HttpCacheability.NoCache), HttpGet]
          public JsonResult ToList()
          {
              using (Context context = new Context())
              {
                  TORepository repository = new TORepository(context);
                  var list = repository.GetAcceptedToList().Select(s => new
                  {
                      text = string.Format("{0} ({1})",s.TO, s.Year),
                      value = s.TO,
                      avr = HttpUtility.UrlEncode(s.TO)
                      
                  }).ToList();
                  return Json(list, JsonRequestBehavior.AllowGet);
              }
          }

          [CacheControl(HttpCacheability.NoCache), HttpGet]
          public JsonResult ToListForActs(string year, bool filter)
          {
              using (Context context = new Context())
              {
                  ActRepository repository = new ActRepository(context);
                  var toes = repository.GetReadyTOForAct(year,filter);
                  if (toes != null)
                  {
                      var list = toes.Select(s => new
                      {
                          text = string.Format("{0} ({1})",s.TO, s.Year),
                          value = s.TO
                      }).ToList();
                      return Json(list, JsonRequestBehavior.AllowGet);
                  }
                  return null;
              }
          }

        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult GetAVRList()
        {
            using (Context context = new Context())
            {
               // var avrs= AVRRepository.GetReadyToPORAVRList(context).Select(a => new { avr = a.AVRId, conf = a.RukFiliala, workStart = a.WorkStart, workEnd = a.WorkEnd }).ToList();
                var avrs = AVRRepository.GetReadyForPricingAVRList(context)
                    .Where(av=>string.IsNullOrEmpty(av.PurchaseOrderNumber))
                    .Select(a => new {
                        avr = a.AVRId,
                        conf = a.RukFiliala,
                        workStart = a.WorkStart,
                        workEnd = a.WorkEnd,
                        needPreprice = a.NeedPreprice,
                        por = a.PurchaseOrderNumber }
                    ).ToList();
             
                return Json(avrs, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SubcId"></param>
        /// <param name="ProjectId"></param>
        /// <param name="WorkStart"></param>
        /// <param name="WorkEnd"></param>
        /// <param name="nsc">Убрать сапкода из запроса</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]
        
        public JsonResult PositionList(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd, bool nsc=false)
        {
            using (Context context = new Context())
            {
              
                //PriceListRepository priceListRepository = new PriceListRepository(context);
                //var items = priceListRepository.GetActivePriceListsItemsSAPCodes(SubcId, ProjectId, WorkStart, WorkEnd);
                //var list = items.Select(s => new
                //{
                //    text = s.Code + " - " + s.Description,
                //    value = s.Id
                //}).ToList();
                //return Json(list, JsonRequestBehavior.AllowGet);

                PriceListRepository priceListRepository = new PriceListRepository(context);
                var items = priceListRepository.GetActivePriceListsRevisionItems(SubcId, ProjectId, WorkStart, WorkEnd);
                var list = items.Select(s => new
                {
                    text = nsc?s.Name:s.SAPCode.Code + " - " + s.Name,
                    value = s.Id,
                    price = s.Price
                }).ToList();
               
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// ВОзвращает активные позиции прайс листов для то
        /// </summary>
        /// <param name="ToId"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]
        public JsonResult TOPositionList(string ToId)
        {
            using (Context context = new Context())
            {
                var to = context.ShTOes.Find(ToId);
                if (to != null)
                {
                  
                    TORepository toRepository = new TORepository(context);
                    var toSubcontr = toRepository.GetTOSubContractor(to.TO);
                    if (toSubcontr == null)
                        return null;
                    var project = context.Projects.FirstOrDefault(p=>p.Name=="Solaris");
                    var toitems = toRepository.GetTOItems(to);
                    PriceListRepository priceListRepository = new PriceListRepository(context);
                    var items = priceListRepository.GetActivePriceListsRevisionItems(toSubcontr.Id, project.Id, toitems.Min(i => i.TOPlanDate), toitems.Max(i => i.TOPlanDate));
                    var list = items.Select(s => new
                    {
                        text = s.SAPCode.Code + " - " + s.Name,
                        value = s.Id
                    }).ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                return null;

            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache)]
        public JsonResult TOPositionListGet(string ToId)
        {
            var result = TOPositionList(HttpUtility.UrlDecode(ToId));
            if (result == null)
                return null;
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// ВОзвращает активные позиции прайс листов для то
        /// </summary>
        /// <param name="ToId"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpGet]
        public JsonResult TOItemsPositionList(string ToId)
        {
            using (Context context = new Context())
            {
                var to = context.ShTOes.Find(ToId);
                if (to != null)
                {

                    TORepository toRepository = new TORepository(context);
                    var toSubcontr = toRepository.GetTOSubContractor(to.TO);
                    if (toSubcontr == null)
                        return null;
                  
                    var toitems = toRepository.GetTOItemModels(to.TO);


                    return Json(toitems, JsonRequestBehavior.AllowGet);
                }
                return null;

            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]
        public JsonResult PositionListWithECR(int SubcId, int ProjectId, DateTime? WorkStart, DateTime? WorkEnd)
        {
            using (Context context = new Context())
            {

                //PriceListRepository priceListRepository = new PriceListRepository(context);
                //var items = priceListRepository.GetActivePriceListsItemsSAPCodes(SubcId, ProjectId, WorkStart, WorkEnd);
                //var list = items.Select(s => new
                //{
                //    text = s.Code + " - " + s.Description,
                //    value = s.Id
                //}).ToList();
                //return Json(list, JsonRequestBehavior.AllowGet);

                PriceListRepository priceListRepository = new PriceListRepository(context);
                var items = priceListRepository.GetActivePriceListsRevisionItems(SubcId, ProjectId, WorkStart, WorkEnd);
                var list = items.Select(s => new
                {
                    text = s.SAPCode.Code + " - " + s.Name,
                    value = s.Id
                }).ToList();
                list.Add(new { text = "ECRADD", value = -1 });
                return Json(list, JsonRequestBehavior.AllowGet);

            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]
        public JsonResult PositionListForAWR(string AWRId, DateTime? WorkStart, DateTime? WorkEnd)
        {
            using (Context context = new Context())
            {

                var AWR = context.ShAVRs.FirstOrDefault(a=>a.AVRId == AWRId);
                if (AWR !=null && !string.IsNullOrEmpty(AWR.Subcontractor) && !string.IsNullOrEmpty(AWR.Project))
                {
                    var Subc = context.SubContractors.FirstOrDefault(s => s.Name == AWR.Subcontractor);
                    var Project = context.Projects.FirstOrDefault(p => p.Name == AWR.Project);
                    if (Subc != null && Project != null)
                    {
                        return PositionList(Subc.Id, Project.Id, WorkStart, WorkEnd);
                    }
                }
                //Ошибка
                return Json(null);
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache), HttpPost]
        public JsonResult GetPriceListItemPrice(int id)
          {
              
            if (Request.IsAjaxRequest())
              {
                  using (Context context = new Context())
                  {
                      var item = context.PriceListRevisionItems.Find(id);
                      if (item == null)
                          return null;
                      else
                          return Json(item.Price);
                  }
              }
              return null;
          }
        
        public JsonResult GetActList()
        {
            using (Context context = new Context())
            {

                var result = context.SATActs.OrderByDescending(act => act.CreateDate).ToList().Select(act => new
                {
                    Id = act.Id,
                    TO = act.TO,
                    PrintDate = act.CreateDate,
                    UserName = act.CreateName,
                    ActName = act.ActName,
                  //  SubContractorName = por.SubContractorName,
                    WorkStart = act.StartDate,
                    WorkEnd = act.EndDate,
                    UploadedToSH = act.UploadedToSH,
                    UploadedToSHComment = act.UploadToSHComment,
                    Price = act.SATActItems.Sum(i=>i.Quantity*i.Price).FinanceRound()
                   // Project = act.,
                    //Status = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? "" : por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().Status.Name,
                    // StatusDate = por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault() == null ? null : new DateTime?(por.PORStatuses.OrderByDescending(p => p.Id).FirstOrDefault().StatusDate),
                   // AVR = (por is AVRPOR) ? ((AVRPOR)por).AVRId : ""
                });
                return Json(new { data = result, total = result.Count() });
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [CacheControl(HttpCacheability.NoCache)]
        public JsonResult SubcontractorsList()
        {
             
            using (var context = new Context())
            {
                var result = context.SubContractors.ToList().Select(sbc => new {Id=sbc.Id, Name = sbc.Name}).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
         
        }
        }
    }
}
