using MailProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using System.Data;
using DbModels.DomainModels.ShClone;
using System.ComponentModel.DataAnnotations;
using EpplusInteract;
using System.IO;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport
{
    public class SOLAutoReport : ATaskHandler
    {
        public SOLAutoReport(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            RedemptionMailProcessor redemtion = new RedemptionMailProcessor("SOLARIS");
            var mails = redemtion.GetMails(new List<string>() { "AutoReport" });
            foreach (var mail in mails)
            {

                var matchs = mail.Subject.Split(new string[] { @"#" }, StringSplitOptions.RemoveEmptyEntries);
                if (matchs.Count() < 2)

                    continue;
                var reportName = matchs[1].ToUpper();
                var filter = string.Empty;
                if (matchs.Count() > 2)
                {
                    filter = matchs[2];
                }

                Dictionary<string, DataTable> workBooksDict = new Dictionary<string, DataTable>();
                string reasonMessage = string.Empty;
                var subcs = TaskParameters.Context.ShContacts.Where(c => c.EMailAddress.Contains(mail.Email)).ToList();
                if (subcs.Count() == 0)
                {
                    reasonMessage = "You not authorized for this request. Please contact aleksey.gorin@ericsson.com";
                    var contacts = mail.Email.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(contacts, reportName, TaskParameters.DbTask.ArchiveFolder) { DataTables = workBooksDict, HtmlBody = reasonMessage });
                    redemtion.MoveToUnsuccess(mail.ConversationId);
                    continue;
                }
                else
                {
                    switch (reportName)
                    {
                        case "AVRREPORT":
                            {
                                //  List<ShAVRf> avrFList = new List<ShAVRf>();
                                List<ShAVRs> avrSList = new List<ShAVRs>();
                                foreach (var subc in subcs)
                                {
                                    // List<ShAVRf> data1 = TaskParameters.Context.ShAVRf.Where(s => s.Subcontractor == subc.Contact).ToList();
                                    List<ShAVRs> data2 = TaskParameters.Context.ShAVRs.Where(s => s.Subcontractor == subc.Contact).ToList();


                                    //  avrFList.AddRange(data1);
                                    avrSList.AddRange(data2);


                                }
                                var avrModelsS = avrSList.Select(a => new AVRModel()
                                {
                                    ApproovedBySoursing = a.ApproovedBySoursing,
                                    AVRId = a.AVRId,
                                    BillNumber = a.BillNumber,
                                    BranchManagar = a.BranchManagar,
                                    CommentarijRassmotreniya = a.CommentarijRassmotreniya,
                                    DataVipuskaPO = a.DataVipuskaPO,
                                    DeliveryNumber = a.DeliveryNumber,
                                    DepartmentManager = a.DepartmentManager,
                                    FacruteNumber = a.FacruteNumber,
                                    FactVypolneniiaRabotPodtverzhdaiuCB = a.FactVypolneniiaRabotPodtverzhdaiuCB,
                                    FactVypolneniiaRabotPodtverzhdaiuRukOtd = a.FactVypolneniiaRabotPodtverzhdaiuRukOtd,
                                    KomentariiECRAdmKzayavke = a.KomentariiECRAdmKzayavke,
                                    KZDPoluchen = a.KZDPoluchen,
                                    ObjectCreationDateTime = a.ObjectCreationDateTime,
                                    OtpravlenoPodryadchikuDlyaRassmotreniya = a.OtpravlenoPodryadchikuDlyaRassmotreniya,
                                    PeredanoVOplatu = a.PeredanoVOplatu,
                                    PORotpravlenVOD = a.PORotpravlenVOD,
                                    PurchaseOrderNumber = a.PurchaseOrderNumber,
                                    RukFiliala = a.RukFiliala,
                                    RukOtdela = a.RukOtdela,
                                    SentToSourcing = a.SentToSourcing,
                                    SentToSubcontractor = a.SentToSubcontractor,
                                    Signed = a.Signed,
                                    StatusRassmotreniya = a.StatusRassmotreniya,
                                    Subcontractor = a.Subcontractor,
                                    TaskSubcontractorNumber = a.TaskSubcontractorNumber,
                                    TotalAmount = a.TotalAmount,
                                    ZayavkaECRAdmPoluchenaVobrabotku = a.ZayavkaECRAdmPoluchenaVobrabotku










                                });
                                // workBooksDict.Add(string.Format("AVRReport2013.xls"), avrFList.ToDataTableDN());
                                workBooksDict.Add(string.Format("AVRReport2014.xls"), avrModelsS.ToList().ToDataTableDN());
                                break;
                            }
                        case "TOREPORT":
                        case "ТОREPORT":
                            {
                                if (string.IsNullOrEmpty(filter))
                                {
                                    continue;
                                }
                                var templatePath = @"\\RU00112284\p\AutoImportTemplates\Solaris\TOReportTemplate.xlsx";
                                if (File.Exists(templatePath))
                                {
                                    try
                                    {
                                        EpplusService service = new EpplusService(new FileInfo(templatePath));

                                        //var test1 = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t }).ToList();
                                        //var test2 = test1.Join(TaskParameters.Context.ShTOItems, t => t.t.TO, i => i.TOId, (t, i) => new { t.c, t.t, i }).ToList();
                                        //var test3 = test2.Join(TaskParameters.Context.ShSITEs, t => t.i.Site, s => s.Site, (t, s) => new { t.c, t.t, t.i, s }).ToList();

                                        var shTO = TaskParameters.Context.ShTOes.Where(t => t.TO == filter).FirstOrDefault();
                                        if (shTO == null)
                                        {
                                            reasonMessage = "ТО не существует";
                                            TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = reasonMessage, AllowWithoutAttachments = true });
                                            break;
                                        }
                                        if (shTO.Subcontractor == null || string.IsNullOrEmpty(shTO.Subcontractor))
                                        {
                                            reasonMessage = string.Format("Не указан подрядчик для ТО'{0}'", shTO.TO);
                                            TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = reasonMessage, AllowWithoutAttachments = true });
                                            break;
                                        }

                                        //try
                                        //{
                                        //    var test1 = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t }).ToList();
                                        //    var test2 = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t })
                                        //    .Join(TaskParameters.Context.ShTOItems, t => t.t.TO, i => i.TOId, (t, i) => new { t.c, t.t, i }).ToList();
                                        //    var test3 = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t })
                                        //    .Join(TaskParameters.Context.ShTOItems, t => t.t.TO, i => i.TOId, (t, i) => new { t.c, t.t, i })
                                        //    .Join(TaskParameters.Context.ShSITEs, t => t.i.Site, s => s.Site, (t, s) => new { t.c, t.t, t.i, s }).ToList();


                                        //    var test4 = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t })
                                        //.Join(TaskParameters.Context.ShTOItems, t => t.t.TO, i => i.TOId, (t, i) => new { t.c, t.t, i })
                                        //.Join(TaskParameters.Context.ShSITEs, t => t.i.Site, s => s.Site, (t, s) => new { t.c, t.t, t.i, s })
                                        //.Select(s => new TOReportModel()
                                        //{
                                        //    Address = s.s.Address,
                                        //    Branch = s.s.Branch,
                                        //    ItemId = s.i.TOItem,
                                        //    PlanDate = s.i.TOPlanDate,
                                        //    PLDescription = s.i.DescriptionFromPL,
                                        //    Quantity = s.i.Quantity,
                                        //    Region = s.s.MacroRegion,
                                        //    Site = s.s.Site,
                                        //    SubcPlanDate = s.i.TOPlanDateSubcontractor,
                                        //    Subcontractor = s.c.Contact,
                                        //    SubcFactDate = s.i.TOFactDate,
                                        //    TO = s.t.TO,
                                        //    WorkConfirmedByEricsson = s.i.WorkConfirmedByEricsson,

                                        //    FileReportTO1 = s.i.FileReportTO1,
                                        //    FileReportTO2 = s.i.FileReportTO2,
                                        //    FileReportTO3 = s.i.FileReportTO3,
                                        //    FileReportTO4 = s.i.FileReportTO4,




                                        //}).ToList();

                                        //}
                                        //catch (Exception exc)
                                        //{

                                        //    throw;
                                        //}

                                        var result = subcs.Join(TaskParameters.Context.ShTOes.Where(t => t.TO == filter), c => c.Contact, t => t.Subcontractor, (c, t) => new { c, t })
                                        .Join(TaskParameters.Context.ShTOItems, t => t.t.TO, i => i.TOId, (t, i) => new { t.c, t.t, i })
                                        .Join(TaskParameters.Context.ShSITEs, t => t.i.Site, s => s.Site, (t, s) => new { t.c, t.t, t.i, s })
                                        .Select(s => new TOReportModel()
                                        {
                                            Address = s.s.Address,
                                            Branch = s.s.Branch,
                                            ItemId = s.i.TOItem,
                                            PlanDate = s.i.TOPlanDate,
                                            PLDescription = s.i.DescriptionFromPL,
                                            Quantity = s.i.Quantity,
                                            Region = s.s.MacroRegion,
                                            Site = s.s.Site,
                                            SubcPlanDate = s.i.TOPlanDateSubcontractor,
                                            Subcontractor = s.c.Contact,
                                            SubcFactDate = s.i.TOFactDate,
                                            TO = s.t.TO,
                                            WorkConfirmedByEricsson = s.i.WorkConfirmedByEricsson,

                                            FileReportTO1 = s.i.FileReportTO1,
                                            FileReportTO2 = s.i.FileReportTO2,
                                            FileReportTO3 = s.i.FileReportTO3,
                                            FileReportTO4 = s.i.FileReportTO4,




                                        }).ToList();
                                        foreach (var row in result)
                                        {
                                            var items = TaskParameters.Context.ShTOItems.Where(a => a.TOItem == row.ItemId);
                                            if (items.Count() > 0)
                                                row.Acts = string.Join(", ", items.Select(a => a.ActId));
                                        }

                                        if (result.Count() != 0)
                                        {
                                            service.InsertTableToPatternCellInWorkBook("table", result.ToDataTableDN(), new EpplusService.InsertTableParams() { PrintHeaders = false });
                                            Dictionary<string, string> dict = new Dictionary<string, string>();
                                            dict.Add("Sender", string.Join(", ", subcs.Select(s => s.Contact)));
                                            dict.Add("Date", DateTime.Now.ToString());
                                            dict.Add("filter", filter);
                                            service.ReplaceDataInBook(dict);
                                            string reportFileName = string.Format("{0}{1}.xlsx",
                                                            reportName,
                                                            DateTime.Now.ToString("ddMMyyyyHHmmss"));
                                            string reportPath = Path.Combine(
                                                    TaskParameters.DbTask.ArchiveFolder,
                                                        reportFileName
                                                );
                                            File.WriteAllBytes(reportPath, service.GetBytes());
                                            TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = reasonMessage, FilePaths = new List<string>() { reportPath } });
                                        }
                                        else
                                        {
                                            var toes = subcs.Join(TaskParameters.Context.ShTOes, c => c.Contact, t => t.Subcontractor, (c, t) => new { t.TO });
                                            string message = string.Empty;
                                            message += string.Format("Не найдено ТО с названием :{0}.", filter);
                                            message += string.Format("Возможны следующие варианты:");
                                            string toListText = string.Empty;
                                            foreach (var to in toes)
                                            {
                                                toListText += string.Format("<li>{0}</li>", to.TO);
                                            }
                                            message += string.Format("<ul>{0}</ul>", toListText);
                                            TaskParameters.EmailHandlerParams.EmailParams.Add(
                                                new EmailParams(
                                                    new List<string> { mail.Email }
                                                    , reportName
                                                    , TaskParameters.DbTask.ArchiveFolder)
                                                    {
                                                        HtmlBody = message
                                                    ,
                                                        AllowWithoutAttachments = true
                                                    });
                                        }

                                    }
                                    catch (Exception exc)
                                    {
                                        TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = "Sorry, there is an error. Please contact aleksey.gorin@ericsson.com", AllowWithoutAttachments = true });
                                        TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { "aleksey.gorin@ericsson.com" }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = "Sorry, there is an error. Please contact aleksey.gorin@ericsson.com", AllowWithoutAttachments = true });
                                    }

                                }



                                break;
                            }

                        default:
                            {
                                reasonMessage = "Report is not allowed. Please contact aleksey.gorin@ericsson.com";
                                TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { HtmlBody = reasonMessage, AllowWithoutAttachments = true });
                                break;
                            }
                    }
                }
                if (workBooksDict.Keys.Count > 0)
                    //TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { "aleksey.gorin@ericsson.com" }, reportName, TaskParameters.DbTask.ArchiveFolder) { DataTables = workBooksDict, HtmlBody = reasonMessage });

                    TaskParameters.EmailHandlerParams.EmailParams.Add(new EmailParams(new List<string> { mail.Email }, reportName, TaskParameters.DbTask.ArchiveFolder) { DataTables = workBooksDict, HtmlBody = reasonMessage });

                redemtion.MoveToSuccess(mail.ConversationId);
            }
            return true;
        }


        public class AVRModel
        {

            [Display(Name = "Заявка на оплату АВР")]
            public string AVRId { get; set; }
            [Display(Name = "Object creation date/time")]
            public DateTime? ObjectCreationDateTime { get; set; }
            [Display(Name = "Подрядчик")]
            public string Subcontractor { get; set; }
            [Display(Name = "Номер заявки подрядчика")]
            public string TaskSubcontractorNumber { get; set; }
            [Display(Name = "Стоимость работ")]
            public decimal? TotalAmount { get; set; }
            [Display(Name = "Состав работ и смета утверждены рук. отдела")]
            public string RukOtdela { get; set; }
            [Display(Name = "Состав работ и смета утверждены рук. отдела by")]
            public string DepartmentManager { get; set; }
            [Display(Name = "Состав работ и смета утверждены рук. филиала")]
            public string RukFiliala { get; set; }
            [Display(Name = "Состав работ и смета утверждены рук. филиала by")]
            public string BranchManagar { get; set; }
            [Display(Name = "Факт выполнения работ подтверждаю CB")]
            public bool FactVypolneniiaRabotPodtverzhdaiuCB { get; set; }
            [Display(Name = "Факт выполнения работ утвержден рук. отдела by")]
            public string FactVypolneniiaRabotPodtverzhdaiuRukOtd { get; set; }
            [Display(Name = "Заявка ECR адм. получена в обработку")]
            public DateTime? ZayavkaECRAdmPoluchenaVobrabotku { get; set; }
            [Display(Name = "Номер Purchase Order")]
            public string PurchaseOrderNumber { get; set; }
            [Display(Name = "Дата выпуска PO")]
            public DateTime? DataVipuskaPO { get; set; }
            [Display(Name = "POR отправлен в OD")]
            public DateTime? PORotpravlenVOD { get; set; }
            [Display(Name = "Коментарии ECR Adm к заявке")]
            public string KomentariiECRAdmKzayavke { get; set; }
            [Display(Name = "Отправлено в соурсинг")]
            public DateTime? SentToSourcing { get; set; }
            [Display(Name = "Утверждение соурсингом")]
            public DateTime? ApproovedBySoursing { get; set; }
            [Display(Name = "Подписание")]
            public DateTime? Signed { get; set; }
            [Display(Name = "Отправлено подрядчику")]
            public DateTime? SentToSubcontractor { get; set; }
            [Display(Name = "Счет №")]
            public string BillNumber { get; set; }
            [Display(Name = "Счет-фактура №")]
            public string FacruteNumber { get; set; }
            [Display(Name = "КЗД получен")]
            public DateTime? KZDPoluchen { get; set; }
            [Display(Name = "Статус рассмотрения")]
            public string StatusRassmotreniya { get; set; }
            [Display(Name = "Комментарий рассмотрения")]
            public string CommentarijRassmotreniya { get; set; }
            [Display(Name = "Передано в оплату")]
            public DateTime? PeredanoVOplatu { get; set; }
            [Display(Name = "Отправлено подрядчику для корректировки")]
            public DateTime? OtpravlenoPodryadchikuDlyaRassmotreniya { get; set; }
            [Display(Name = "Номер почтового отправления")]
            public string DeliveryNumber { get; set; }






        }

        public class TOReportModel
        {
            [Display(Name = "Номер ТО")]
            public string TO { get; set; }
            [Display(Name = "Регион")]
            public string Region { get; set; }
            [Display(Name = "Филиал")]
            public string Branch { get; set; }
            [Display(Name = "Номер площадки")]
            public string Site { get; set; }
            [Display(Name = "Адрес")]
            public string Address { get; set; }
            [Display(Name = "Подрядчик")]
            public string Subcontractor { get; set; }
            [Display(Name = "Номер позиции")]
            public string ItemId { get; set; }
            [Display(Name = "Описание работ")]
            public string PLDescription { get; set; }
            [Display(Name = "Количество из заказа")]
            public decimal? Quantity { get; set; }
            [Display(Name = "Планируемая дата выполнения работ на объекте")]
            public DateTime? PlanDate { get; set; }
            [Display(Name = "Планируемая подрядчиком дата выполения работ на объекте")]
            public DateTime? SubcPlanDate { get; set; }
            [Display(Name = "Фактическая дата выполения работ на объекте подрячиком")]
            public DateTime? SubcFactDate { get; set; }
            [Display(Name = "Выполение работ подтверждено Ericsson")]
            public bool WorkConfirmedByEricsson { get; set; }
            public string WorkConfirmedByEricssonBy { get; set; }
            public DateTime? WorkConfirmedByEricssonDate { get; set; }
            [Display(Name = "Упоминание о № акта или оплате")]
            public string Acts { get; set; }
            public string FileReportTO1 { get; set; }
            public string FileReportTO2 { get; set; }
            public string FileReportTO3 { get; set; }
            public string FileReportTO4 { get; set; }




        }
    }
}
