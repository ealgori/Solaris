using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Intranet.Models;
using DbModels.DataContext;


namespace Intranet.Controllers
{   
    public class SubContractorsController : Controller
    {

        public ActionResult Index()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            breadCrumbs.Add(new BreadCrumbModel(){ Name="SubContractor", Path= Url.Action("Index", "SubContractor",null, Request.Url.Scheme)});
            breadCrumbs.Add(new BreadCrumbModel(){ Name="List"});
            ViewBag.Path = breadCrumbs;
            return View();
        }


         [HttpPost]
        public JsonResult Read()
        {
             using (var context = new Context())
            {
                var result = context.SubContractors.ToList().Select(sbc => new {Id=sbc.Id, Name = sbc.Name, Address=sbc.Address, SAPNumber=sbc.SAPNumber, SAPName=sbc.SAPName, Project=(sbc.Project==null?"Не указан":sbc.Project.Name)}).ToList();
                return Json(new { data = result, total = result.Count });
            }
          ;
        }

      
    }
}