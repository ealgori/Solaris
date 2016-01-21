using DbModels.DataContext;
using DbModels.DomainModels.SAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Intranet.Controllers
{
    public class AVRDistributionController : Controller
    {

        private Func<SATSubregion, bool> NotDeleted = (s) => !s.Deleted.HasValue;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(int? id = null)
        {
            using (Context context = new Context())
            {
                var recipients = context.SATSubregions.Where(NotDeleted).ToList();
                if (!id.HasValue)
                    return Json(recipients, JsonRequestBehavior.AllowGet);
                else
                {
                    var recipient = recipients.FirstOrDefault(s => s.Id == id);
                    if (recipient != null)
                        return Json(recipient, JsonRequestBehavior.AllowGet);
                    else
                        return new HttpStatusCodeResult(404);
                }
            }

        }




        [System.Web.Http.HttpPut]
        public ActionResult Put(SATSubregion model)
        {
            using (Context context = new Context())
            {


                //var entity = context.SATSubregions.FirstOrDefault(m => m.Id == model.Id);
                //if (entity != null)
                //{

                //    entity.POROREmail = model.POROREmail;
                //    entity.RukFillialaEmail = model.RukFillialaEmail;
                //    entity.RukOtdelaEmail = model.RukOtdelaEmail;
                //    entity.Name = model.Name;
                //    entity.Enabled = model.Enabled;
                //    context.SaveChanges();

                //}
                context.Entry(model).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return Json(model);





            }

        }

        [HttpPost]
        public ActionResult Post(SATSubregion model)
        {
            using (Context context = new Context())
            {


                context.Entry(model).State = System.Data.Entity.EntityState.Added;
                context.SaveChanges();
                //var entity = context.SATSubregions.FirstOrDefault(m => m.Id == model.Id);
                //if (entity == null)
                //{
                //    entity = new SATSubregion();
                //    entity.POROREmail = model.POROREmail;
                //    entity.RukFillialaEmail = model.RukFillialaEmail;
                //    entity.RukOtdelaEmail = model.RukOtdelaEmail;
                //    entity.Name = model.Name;
                //    entity.Enabled = model.Enabled;
                //    context.SATSubregions.Add(entity);
                //    context.SaveChanges();

                //}
                return Json(model);



            }

        }

        [HttpDelete]

        public ActionResult Delete(int id)
        {
            using (Context context = new Context())
            {
                var entity = context.SATSubregions.FirstOrDefault(m => m.Id == id);
                if (entity != null)
                {
                    if (!entity.Deleted.HasValue)
                    {
                        entity.Deleted = DateTime.Now;
                        context.SaveChanges();

                    }
                    return new HttpStatusCodeResult(200);

                }
                //    entity.POROREmail = model.POROREmail;
                //    entity.RukFillialaEmail = model.RukFillialaEmail;
                //    entity.RukOtdelaEmail = model.RukOtdelaEmail;
                //    context.SaveChanges();
            }
            return new HttpStatusCodeResult(404);
        }
    }
}