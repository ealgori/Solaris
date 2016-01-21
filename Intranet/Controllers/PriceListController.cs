using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intranet.Models;
using CommonFunctions.Extentions;

using ExcelParser.Model;
using DbModels.DataContext;
using ExcelParser.EpplusInteract;
using CommonFunctions;
using ExcelParser;
using DbModels.Repository;
using DbModels.DomainModels.Solaris.Pors;

using System.IO;
using System.ComponentModel.DataAnnotations;
using OfficeOpenXml.Table;
using DbModels.Models;
using Intranet.Service;
using EpplusInteract;

namespace Intranet.Controllers
{
    public class PriceListController : Controller
    {
        //
        // GET: /PriceList/

        public ActionResult Index()
        {
            List<BreadCrumbModel> breadCrumbs = new List<BreadCrumbModel>();
            breadCrumbs.Add(new BreadCrumbModel() { Name = "PriceLists", Path = Url.Action("Index", "PriceList", null, Request.Url.Scheme) });
            breadCrumbs.Add(new BreadCrumbModel() { Name = "Import" });
            ViewBag.Path = breadCrumbs;
            return View();
        }
        public ActionResult SubcReport()
        {
            return View();
        }
        [HttpPost]
        public FileResult SubcReport(int SubcId)
        {
            using (Context context = new Context())
            {
                var dict = new Dictionary<string, object>();
                dict.Add("@Id", SubcId);
                var dataList = StaticHelpers.GetStoredProcDataFromContext<SubcReportRow>(context, "ExportPLForSubc", dict
                    ).ToList();
                if (dataList.Count > 0)
                {

                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("SubcReport-{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));


                    return File(ExcelParser.SubcRep.SubcReport(dataList), ".xls");
                }

            }

            return null;
        }


        public ActionResult ComparePriceList()
        {
            return View();
        }

        public ActionResult PriceListComparer()
        {
            PriceListRepository repository = new PriceListRepository();
            var subcs = repository.GetSubcontractorsList();
            return View(subcs);
        }
        [HttpPost]
        public ActionResult PriceListComparer(int fpl, int spl)
        {
            using (Context context = new Context())
            {
                var fplist = context.PriceLists.Find(fpl);
                var splist = context.PriceLists.Find(spl);

                if (fplist != null && splist != null)
                {
                    var fplistRevision = fplist.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault();
                    var splistRevision = splist.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault();



                    var flOiterSl = fplistRevision.PriceListRevisionItems
                        .GroupJoin(splistRevision.PriceListRevisionItems, f => f.Name, s => s.Name, (f, s) => new JoinItemModel { First = f, Second = s.FirstOrDefault() }).Where(s => s.Second == null);
                    var slOiterFl = splistRevision.PriceListRevisionItems
                     .GroupJoin(fplistRevision.PriceListRevisionItems, f => f.Name, s => s.Name, (s, f) => new JoinItemModel { Second = s, First = f.FirstOrDefault() }).Where(f => f.First == null);
                    var join = splistRevision.PriceListRevisionItems
                         .Join(fplistRevision.PriceListRevisionItems, f => f.Name, s => s.Name, (s, f) => new JoinItemModel { Second = s, First = f });
                    var fullOuter = join.Union(flOiterSl.Union(slOiterFl)).ToList();
                    //var model = fullOuter.Select(s => new { fname = s.First != null ? s.First.Name : ""
                    //,fSap = s.First != null ? s.First.SAPCode.Code  : "" 
                    //,sname = s.Second != null ? s.Second.Name : ""
                    //,sSap = s.Second != null ? s.Second.SAPCode.Code : ""

                    //}
                    List<PriceListImportViewModel> plModel = new List<PriceListImportViewModel>();
                    List<JoinItemViewModel> model = new List<JoinItemViewModel>();
                    int i = 0;
                    foreach (var item in fullOuter)
                    {
                        i++;
                        var m = new JoinItemViewModel();
                        var p = new PriceListImportViewModel();
                        p.Id = i;
                        if (item.First != null)
                        {
                            m.SubcFrom = item.First.PriceListRevision.PriceList.SubContractor.Name;

                            m.PriceNumberFrom = string.Format("{0} {1}", item.First.PriceListRevision.PriceList.PriceListNumber, item.First.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "");
                            m.SAPCodeFrom = item.First.SAPCode.Code;
                            m.DescriptionFrom = item.First.Name;
                            m.PriceFrom = item.First.Price;
                            m.UploadDateFrom = item.First.PriceListRevision.CreationDate;
                            p.Description = item.First.Name;
                            p.Price = item.First.Price;
                            p.Unit = item.First.Unit;
                        }

                        if (item.Second != null)
                        {
                            m.SubcTo = item.Second.PriceListRevision.PriceList.SubContractor.Name;
                            m.PriceNumberTo = string.Format("{0} {1}", item.Second.PriceListRevision.PriceList.PriceListNumber, item.Second.PriceListRevision.PriceList.PriceListAdditionalNumber ?? "");
                            m.SAPCodeTo = item.Second.SAPCode.Code;
                            m.DescriptionTo = item.Second.Name;
                            m.UploadDateTo = item.Second.PriceListRevision.CreationDate;
                            m.PriceTo = item.Second.Price;

                            if (item.First == null)
                            {
                                p.Description = item.Second.Name;
                                p.Unit = item.Second.Unit;

                            }
                        }

                        if (item.First != null && item.Second != null)
                        {
                            m.Delta = m.PriceFrom - m.PriceTo;
                        }
                        model.Add(m);
                        plModel.Add(p);

                    }




                    var templatePath = @"\\RU00112284\SolarisTemplates\ComparerTemplate.xlsm";
                    if (System.IO.File.Exists(templatePath))
                    {
                        EpplusService service = new EpplusService(new FileInfo(templatePath));
                        service.InsertTableToPatternCellInWorkBook("DetailsTable", model.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Light14, ShowRowStripes = true });
                        service.InsertTableToPatternCellInWorkBook("PLSTable", plModel.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });
                        var wbookbyteArray = service.GetBytes();
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("ComparePriceList({0}).xlsm", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                        return File(wbookbyteArray, ".xlsx");



                    }


                }
            }
            return null;
        }
        private class JoinItemModel
        {
            public PriceListRevisionItem First { get; set; }
            public PriceListRevisionItem Second { get; set; }
        }
        private class JoinItemViewModel
        {
            public string SubcFrom { get; set; }
            public string SubcTo { get; set; }
            public string PriceNumberFrom { get; set; }
            public string PriceNumberTo { get; set; }
            public DateTime? UploadDateFrom { get; set; }
            public DateTime? UploadDateTo { get; set; }
            public string SAPCodeFrom { get; set; }
            public string SAPCodeTo { get; set; }
            public string DescriptionFrom { get; set; }
            public string DescriptionTo { get; set; }
            public decimal? PriceFrom { get; set; }
            public decimal? PriceTo { get; set; }
            public decimal? Delta { get; set; }
        }

        private class PriceListImportViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string Unit { get; set; }
            public decimal? Price { get; set; }

        }


        public JsonResult GetSubcPriceLists(int subcId)
        {
            PriceListRepository repository = new PriceListRepository();
            var pls = repository.GetSubcontractorPriceLists(subcId);
            var models = new List<PriceListModel>();
            foreach (var item in pls)
            {
                var model = new PriceListModel();
                var revision = repository.GetLastPriceListRevision(item.Id);

                model.Id = item.Id;
                model.Name = string.Format("{0} {1}", item.PriceListNumber, item.PriceListAdditionalNumber ?? "");
                if (revision != null)
                {
                    model.SignDate = revision.SignDate.ToString("dd-MM-yyyy");
                    model.Comparable = item.Comparable;
                    model.Approved = revision.Approved;
                    model.RevId = revision.Id;
                    var impFile = repository.GetPLRevisionImportFile(revision.Id);
                    if (impFile != null)
                    {
                        model.FileId = impFile.Id;
                    }
                }
                models.Add(model);

            }

            //var plsModel = pls.ToList().Select(p => new
            //{
            //    Id = p.Id,
            //    Name = string.Format("{0} {1}", p.PriceListNumber, p.PriceListAdditionalNumber ?? ""),
            //    SignDate = p.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault().SignDate.ToString("dd-MM-yyyy"),
            //    Comparable = p.Comparable,
            //    Approved = p.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault().Approved,
            //    RevId = p.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault().Id
            //});
            //FileId = p.PriceListRevisions.OrderByDescending(r => r.Id).FirstOrDefault().ImportFile.Id });
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Read()
        {
            using (var context = new Context())
            {
                var result = context.PriceLists.Select(p => new PriceListViewModel
                {
                    PriceListId = p.Id,
                    CreationDate = p.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().CreationDate,
                    ExpiryDate = p.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().ExpiryDate,
                    ProjectId = p.Project.Id,
                    VendorNumber = p.SubContractor.SAPNumber,
                    VendorName = p.SubContractor.Name,
                    PriceListAdditionalNumber = p.PriceListAdditionalNumber,
                    PriceListNumber = p.PriceListNumber,
                    SignDate = p.PriceListRevisions.OrderByDescending(plr => plr.CreationDate).FirstOrDefault().SignDate,
                    SubcId = p.SubContractor.Id,
                    Project = p.Project.Name,
                    ForCompare = p.Comparable
                }).OrderByDescending(p => p.CreationDate).ToList();
                return Json(new { data = result, total = result.Count });
            }
        }
        [HttpPost]
        public ActionResult Upload(PriceListUploadModel model)
        {
            //using (var Context = new Context())
            //{
            //    var pr = new PriceList { 
            //     CreationDate = DateTime.Now,
            //      SignDate = DateTime.Now,
            //     PriceListNumber = DateTime.Now.Millisecond.ToString()
            //    };
            //    Context.PriceLists.Add(pr);
            //    Context.SaveChanges();
            //}
            using (MultiplePriceImport mPriceImport = new MultiplePriceImport(User.Identity.Name))
            {

                mPriceImport.Process(Request.Files, model.ProjectId, User.Identity.Name, model.Comparable);
            }
            return null;
        }
        [HttpPost]
        public JsonResult ApproveDisapprove(int revId, bool status)
        {
            using (Context context = new Context())
            {
                var revision = context.PriceListRevisions.Find(revId);
                if (revision != null)
                {
                    PriceListRepository reposit = new PriceListRepository(context);
                    bool result = false;
                    if (status)
                    {

                        result = reposit.ApprovePriceListRevision(revId, User.Identity.Name);
                    }
                    else
                    {
                        result = reposit.DisApprovePriceListRevision(revId, User.Identity.Name);
                    }
                    if (result)
                    {
                        reposit.SaveChanges();
                    }

                    return Json(new { success = result, status = revision.Approved });

                }
                return null;
            }
        }
        [HttpPost]
        public JsonResult DeleteCompPL(int revId)
        {
            PriceListRepository repository = new PriceListRepository();

            return Json(repository.DeleteComparablePriceList(revId, User.Identity.Name));
        }


        public JsonResult GetCrossedPlists(int plId, int subcId)
        {
            PriceListRepository repository = new PriceListRepository();
            var crossedItems = repository.GetCrossedItemsPLists(plId, subcId);
            return Json(crossedItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ComparePriceList(PorAnalyzerModel VMmodel)//  int subcontractor, DateTime? start, DateTime? end, int? sourceSubc)
        {

            using (Context context = new Context())
            {
                var result = DataService.ComparePorByBasePL(VMmodel, context);
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);

                }
              //  if (result.CompareReportModelList.Count > 0)
                {
                    var grouppedByDS = result.CompareReportModelList.GroupBy(m => m.PriceListFrom).Select((g, i) => new GrouppedModel()
                    {
                        //Date = g.FirstOrDefault().Date,
                        DS = g.Key,
                        Subcontractor = g.FirstOrDefault().SubcontractorFrom,
                        Plus = g.Where(f => f.Value > 0).Sum(f => f.Value),
                        Minus = g.Where(f => f.Value < 0).Sum(f => f.Value),
                        Saving = g.Where(f => f.Value > 0).Sum(f => f.Value) + g.Where(f => f.Value < 0).Sum(f => f.Value),
                        //Id = i + 1

                    }).ToList();

                    var grouppedByDSmonth = result.CompareReportModelList.GroupBy(m => new { m.PriceListFrom, m.PorDate.Month, m.PorDate.Year }).Select((g, i) => new GrouppedModel()
                    {
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        DS = g.Key.PriceListFrom,
                        Subcontractor = g.FirstOrDefault().SubcontractorFrom,
                        //PorDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Plus = g.Where(f => f.Value > 0).Sum(f => f.Value),
                        Minus = g.Where(f => f.Value < 0).Sum(f => f.Value),
                        Saving = g.Where(f => f.Value > 0).Sum(f => f.Value) + g.Where(f => f.Value < 0).Sum(f => f.Value),
                       // Id = i + 1

                    }).OrderBy(r => r.Year).ThenBy(r=>r.Month).ToList();

                    //var bytes = NpoiInteract.DataTableToExcel(model.ToList().ToList().ToDataTable());
                    //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("PriceListCompare({0}).xls", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                    //return File(bytes, ".xls");
                    var templatePath = @"\\RU00112284\SolarisTemplates\CompareTemplate.xlsx";
                    if (System.IO.File.Exists(templatePath))
                    {
                        var fileInfo = new FileInfo(templatePath);
                        EpplusService service = new EpplusService(fileInfo);
                        service.InsertTableToPatternCellInWorkBook("DetailTable", result.CompareReportModelList.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Light14, ShowRowStripes = true });
                        var grouppedDT = grouppedByDS.ToDataTableDN();
                        //grouppedDT.Columns.Remove("PorDate");
                        service.InsertTableToPatternCellInWorkBook("GroupTable", grouppedDT, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });
                        service.InsertTableToPatternCellInWorkBook("GroupMonthTable", grouppedByDSmonth.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });
                        List<string> errors = new List<string>();
                        errors.Add(string.Format("Не найден опорный ПЛ для :{0}",string.Join("; ", result.UncomparablePLS.Distinct())));
                        errors.Add(string.Format("Не найдены позиции в опорных ПЛ :{0}",string.Join("; ", result.UncomparableItems.Distinct())));
                        errors.Add(string.Format("Неапрувленные опорные ПЛ :{0}", string.Join("; ", result.UnApprovedRevisions.Distinct())));
                        errors.Add(string.Format("Другие ошибки :{0}", string.Join("; ", result.Errors)));
                      
                        service.InsertTableToPatternCellInWorkBook("ErrorTable", errors.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });

                        var wbookbyteArray = service.GetBytes();
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("{1}Compare({0}).xlsx", DateTime.Now.ToString("yyyyMMddHHmm"), result.Subcontractor.SAPName) + "\"");
                        return File(wbookbyteArray, ".xlsx");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, string.Format("Шаблон не найден. Обратитесь к администратору."));

                    }


                }
               // else
                {
                    ModelState.AddModelError(string.Empty, "Нет данных");
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item);
                    }
                    foreach (var item in result.UnApprovedRevisions)
                    {
                        ModelState.AddModelError(string.Empty, string.Format("Не апрувлен:{0}", item));
                    }
                }
            }

            return View(VMmodel);
        }

        
        public ActionResult GroupComparePriceList()
        {


            using (Context context = new Context())
            {


                var subcontractors = context.SubContractors.ToList();
                var parmList = new List<PorAnalyzerResultModel>();
                foreach (var subc in subcontractors)
                {
                    var VMmodel = new PorAnalyzerModel() { subcontractor = subc.Id};
                    var result = DataService.ComparePorByBasePL(VMmodel, context);
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);

                    }
                    if (result.CompareReportModelList.Count() > 0)
                    {
                        parmList.Add(result);
                    }
                    


                }


                if (parmList.Count > 0)
                {
                    var grouppedByDS = parmList.SelectMany(r=>r.CompareReportModelList).GroupBy(m => m.PriceListFrom).Select((g, i) => new GrouppedModel()
                    {
                        //Date = g.FirstOrDefault().Date,
                        DS = g.Key,
                        Subcontractor = g.FirstOrDefault().SubcontractorFrom,
                        Plus = g.Where(f => f.Value > 0).Sum(f => f.Value),
                        Minus = g.Where(f => f.Value < 0).Sum(f => f.Value),
                        Saving = g.Where(f => f.Value > 0).Sum(f => f.Value) + g.Where(f => f.Value < 0).Sum(f => f.Value),
                       // Id = i + 1

                    }).ToList();

                    var grouppedByDSmonth = parmList.SelectMany(r => r.CompareReportModelList).GroupBy(m => new { m.PriceListFrom, m.PorDate.Month, m.PorDate.Year }).Select((g, i) => new GrouppedModel()
                    {
                        Month = g.Key.Month,
                        Year = g.Key.Year,
                        DS = g.Key.PriceListFrom,
                        Subcontractor = g.FirstOrDefault().SubcontractorFrom,
                        //PorDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Plus = g.Where(f => f.Value > 0).Sum(f => f.Value),
                        Minus = g.Where(f => f.Value < 0).Sum(f => f.Value),
                        Saving = g.Where(f => f.Value > 0).Sum(f => f.Value) + g.Where(f => f.Value < 0).Sum(f => f.Value),
                       // Id = i + 1

                    }).OrderBy(r => r.Year).ThenBy(r => r.Month).ToList();

                    //var bytes = NpoiInteract.DataTableToExcel(model.ToList().ToList().ToDataTable());
                    //Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("PriceListCompare({0}).xls", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                    //return File(bytes, ".xls");
                    var templatePath = @"\\RU00112284\SolarisTemplates\CompareTemplate.xlsx";
                    if (System.IO.File.Exists(templatePath))
                    {
                        var fileInfo = new FileInfo(templatePath);
                        EpplusService service = new EpplusService(fileInfo);
                        service.InsertTableToPatternCellInWorkBook("DetailTable", parmList.SelectMany(c=>c.CompareReportModelList).ToList().ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Light14, ShowRowStripes = true });
                        var grouppedDT = grouppedByDS.ToDataTableDN();
                       // grouppedDT.Columns.Remove("PorDate");
                        service.InsertTableToPatternCellInWorkBook("GroupTable", grouppedDT, new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });
                        service.InsertTableToPatternCellInWorkBook("GroupMonthTable", grouppedByDSmonth.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });
                        List<string> errors = new List<string>();
                        errors.Add(string.Format("Прайслисты без опорных :{0}", string.Join(",", parmList.SelectMany(r=>r.UncomparablePLS.Distinct()))));
                        errors.Add(string.Format("Позиции без опорных :{0}", string.Join(",", parmList.SelectMany(r=>r.UncomparableItems.Distinct()))));
                        errors.Add(string.Format("Неапрувленные прайслисты :{0}", string.Join(",", parmList.SelectMany(r=>r.UnApprovedRevisions.Distinct()))));
                        errors.Add(string.Format("Общие ошибки :{0}", string.Join(",", parmList.SelectMany(r=>r.Errors))));

                        service.InsertTableToPatternCellInWorkBook("ErrorTable", errors.ToDataTable(), new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = true, TableStyle = TableStyles.Dark9, ShowRowStripes = true });

                        var grouppedBySubc = parmList.SelectMany(s => s.CompareReportModelList).GroupBy(g => g.SubcontractorFrom);
                        foreach (var group in grouppedBySubc)
                        {
                            service.InsertTableToWorkSheet(group.Key, group.ToList().ToDataTableDN(),new EpplusService.InsertTableParams() { PrintHeaders = true, StyledHeaders = false, BoldHeaders=true} );
                        }


                        var wbookbyteArray = service.GetBytes();
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("PorCompare({0}).xlsx", DateTime.Now.ToString("yyyyMMddHHmm")) + "\"");
                        return File(wbookbyteArray, ".xlsx");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, string.Format("Шаблон не найден. Обратитесь к администратору."));

                    }


                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Нет данных");
                }


            }

            return View("ComparePriceList");
        }



        private class PriceListModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SignDate { get; set; }
            public bool Approved { get; set; }
            public bool Comparable { get; set; }
            public int RevId { get; set; }
            public int? FileId { get; set; }
        }

    }
}
