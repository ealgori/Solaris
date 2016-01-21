using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Intranet.Models;

using System.IO;
using DbModels.DataContext;
using DbModels.Models.ImportFilesModels;

namespace Intranet.Controllers
{
    public class ImportController : Controller
    {
        //
        // GET: /Import/
        public ActionResult Download(int Id)
        {
            using (var context = new Context())
            {
                ImportFile doc = context.ImportFiles.FirstOrDefault(d => d.Id == Id);
                if (doc != null)
                {
                    //Руками правим хедер респонса, указывая имя файла(проблема русских букв в ИЕ)
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + Server.UrlPathEncode(doc.Name) + "\"");
                    return File(doc.File, System.IO.Path.GetExtension(doc.Name));
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Index()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            breadCrumbs.Add(new BreadCrumbModel() { Name = "PriceLists", Path = Url.Action("Index", "PriceList", null, Request.Url.Scheme) });
            breadCrumbs.Add(new BreadCrumbModel() { Name = "Logs" });
            ViewBag.Path = breadCrumbs;
            return View();
        }
        [HttpPost]
        public JsonResult Read()
        {
            using (var context = new Context())
            {
                var result = (from i in context.Imports
                              from f in i.Files
                              orderby i.CreationDate descending
                              select new ImportLogViewModel
                              {
                                  ImportFileId = f.Id,
                                  ImportUploadedDate = i.CreationDate,
                                  FileName = f.Name,
                                  Processed = f.Success ? "Да" : "Нет",
                                  User = i.User
                              }).ToList();
                return Json(new { data = result, total = result.Count });
            }
        }
        [HttpPost]
        public JsonResult LogRead(int ImportFileId)
        {
            using (var context = new Context())
            {
                var result = context.ImportFileLogs.Where(l => l.ImportFile.Id == ImportFileId).Select(l => new LogViewModel
                {
                    LogType = l.Level,
                    LogId = l.Id,
                    Message = l.Message
                }).OrderByDescending(l => l.LogId).ToList();
                return Json(new { data = result, total = result.Count });
            }
        }

    }
}
