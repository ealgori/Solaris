using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcelParser.EpplusInteract;
using DbModels;

namespace Intranet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //using(Context Context = new Context())
            //{
                
            //    return File(CreatePor.CreatePorFile(1), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","POR");
            //}
               return View();
        }
    }
}
