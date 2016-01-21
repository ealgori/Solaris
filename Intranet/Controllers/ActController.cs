using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.SAT;
using Intranet.Models;
using DbModels.DomainModels.ShClone;
using ExcelParser.ExcelParser;

namespace Intranet.Controllers
{
    public class ActController : Controller
    {
        //
        // GET: /Act/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAct(string button, ActModel model)
        {

            CreatePORResultViewModel result = new CreatePORResultViewModel();
            
            List<SATActService> services = new List<SATActService>();
            List<SATActMaterial> materials = new List<SATActMaterial>();

            if (ModelState.IsValid &&
                    (
                    (model.Services != null 
                    && model.Services.Any(s => s.Checked)
                    )|| button=="autoimport"
                
                //27.07.2015 Лень все переделывать. запретим акты на материалы без сервисов.
                // проблема в поиске бранча на сайтах из позицый материалов. почему то нет свойства  сайт.
                //|| 
                //    (
                //    (model.Materials != null) 
                //    && 
                //    model.Materials.Any(m => m.Quantity.HasValue)
                //    )
                //&& 
                //    model.Materials.Any(m => m.Quantity > 0)
                ))
            {

                using (Context context = new Context())
                {
                    ActRepository repository = new ActRepository(context);
                    //TORepository toRepository = new TORepository(context);
                    // ТО и контакты надо так и так проверить
                    ShTO shTO = repository.GetReadyTOForAct(filter:model.Filter).FirstOrDefault(t => t.TO == model.TO);
                    if (shTO == null)
                    {
                        result.Success = false;
                        result.Message = string.Format("TO не существует: {0}", model.TO);
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
                    try
                    {
                        var satAct = repository.GetSatAct(model.TO, User.Identity.Name, model.StartDate, model.EndDate,model.Filter);
                        var satServices = repository.GetSATServices(model.TO, model.Services,model.Filter);
                        var satMaterials = repository.GetSATMaterials(model.TO, model.Materials);

                        if (button == "create")
                        {
                            try
                            {
                                repository.CreateAct(satAct, satServices, satMaterials);
                                result.Success = true;
                                result.Message = string.Format(@"Акт  ""{0}"" создан и отправлен на загрузку в СХ.", satAct.ActName);
                                result.Url = Url.Action("ActList", "Act",null, Request.Url.Scheme);
                                result.UrlText = "Список актов";
                                return Json(result);
                            }
                            catch (Exception exc)
                            {

                                result.Success = false;
                                result.Message = string.Format("Произошла ошибка. Пожалуйста собщите о ней админитратору: {0}:{1}", exc.Message, exc.StackTrace);
                                return Json(result);
                            }
                           
                        }
                            if (button == "autoimport")
                            {
                                satServices = repository.GetSATServices(model.TO, model.Services, true);
                                var bytes =  CreateActAutoImport.Create(satAct, satServices, satMaterials);
                                if(bytes==null)
                                {
                                    result.Success = false;
                                    result.Message = string.Format("exc", "Ошибка генерации автоимпорта");
                                    return Json(result);
                                }
                                else
                                {
                                    SATActFile file = new SATActFile();
                                    file.ActAiFile = bytes;
                                    context.SATActFiles.Add(file);
                                    context.SaveChanges();
                                    result.Success = true;
                                    result.Message = string.Format(@"Файл для автоимпорта создан.", satAct.ActName);
                                    result.Url =   Url.Action("DownloadFile", "Act", new { Id = file.Id }, Request.Url.Scheme);
                                    result.UrlText = "Скачать файл";
                                    //return new FileContentResult(bytes, ".xlsx");
                                    return Json(result);
                                    //CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\temp\created.xlsx", bytes);
                                    // создание новый структы с файлом
                                    // возвращение ссылки на файл
                                }

                            }
                        return new HttpStatusCodeResult(500);
                    }

                    catch (Exception exc)
                    {
                        result.Success = false;
                        result.Message = string.Format("exc:{0}",exc.Message);
                        return Json(result);
                    }
                  
                }
               
            }
            else
            {
                result.Success = false;
                result.Message = string.Format(@"Выберите хотя бы один элемент");
                return Json(result);
            }
            
          
           
        }

        public ActionResult DownloadFile(int Id)
        {
            using(Context context = new Context())
            {
                var file = context.SATActFiles.Find(Id);
                if(file!=null)
                {
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("{0}.xlsx", "ActAutoImport"));
                    return new  FileContentResult(file.ActAiFile, ".xlsx");
                }
            }
            return null;
        }
 


        [HttpPost]
        public ActionResult GetTOItems(string TO, bool filter)
        {
            using (Context context = new Context())
            {
                ActRepository repository = new ActRepository(context);
                TORepository toRepository = new TORepository(context);
                var shTO = repository.GetReadyTOForAct().FirstOrDefault(t => t.TO == TO);
                if (shTO != null)
                {
                    //var items = repository.GetToItemsForAct(TO).Select(i => new { id = i.TOItem, site = i.Site, fact = i.TOFactDate.Value.ToString("dd.MM.yyyy"), price = i.PriceFromPL, description = i.DescriptionFromPL, quantity = i.Quantity }).ToList();
                    //var materials = repository.GetToMatItems(TO).Select(m => new { id = m.MatTOId, site = m.SiteId, price = m.Price, description = m.Description }).ToList();
                    var model = repository.GetToItems(shTO.TO, filter);
                    var woDates = model.Services.Where(s => !s.TOFactDate.HasValue).ToList();
                    if (woDates.Count>0)
                    {
                        return Json(new { Status = "error", Message = string.Format("Не для всех позиций ТО проставлены фактические даты:{0}",string.Join(",",woDates.Select(i=>i.TOItem))) });
                    }
                    var items = model.Services.Select(i => new { id = i.TOItem, site = i.Site, fact = i.TOFactDate.Value.ToString("dd.MM.yyyy"), price = i.PriceFromPL, description = i.DescriptionFromPL, quantity = i.Quantity, partial = !string.IsNullOrEmpty(i.ReasonForPartialClosure),act = i.ActId??"" }).ToList();
                    var materials = model.Materials.Select(m => new { id = m.MatTOId, site = m.SiteId, price = m.Price, description = m.Description, quantity = m.Quantity }).ToList();

                    if (items.Count() == 0 && materials.Count() == 0)
                    {
                        return Json(new { Status = "error", Message = "У TO нет позиций для создания акта." });
                    }
                    else
                    {
                        var po = shTO.PONumber??"Не указано";
                        var poDate = shTO.POIssueDate.HasValue ? shTO.POIssueDate.Value.ToString("dd.MM.yyyy") : "Дата не указана";
                        var subcontractor = shTO.Subcontractor;
                        var shContact = context.ShContacts.FirstOrDefault(c=>c.Contact==subcontractor);
                        var nds = shContact !=null ?( shContact.WithOutVAT?"без НДС":"с НДС") : "с НДС";
                        string region = toRepository.GetToRegion(shTO.TO);
                        string branch = toRepository.GetToBranch(shTO.TO);

                        return Json(new { Items = items, Materials = materials, info = new {po=po, poDate= poDate, subcontractor= subcontractor, nds= nds, region= region, branch=branch } });
                    }
                }
                else
                {
                    return Json(new { Status = "error", Message = "ТО не существует, либо не подохдит для создания актов" });
                }
            }

            //if (result.Items.Count > 0)
            //{
            //    return Json(result);
            //}
        }

        public ActionResult ActList()
        {
            return View();
        }


        public ActionResult PrintDraft(int Id)
        {

            using (Context context = new Context())
            {
                var act = context.SATActs.Find(Id);
                if (act != null)
                {


                    try
                    {
                        var file = ExcelParser.EpplusInteract.CreateAct.CreateActFile(Id);
                        if (file != null)
                        {
                            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("ACT-{0}.xlsm", act.ActName));
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
