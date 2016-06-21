using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoImport.Rev3.ImportHandlers.Abstract;
using EpplusInteract;
using System.IO;
using CommonFunctions.Extentions;
using AutoImport.Rev3.DomainModels;
using DbModels.DataContext;

namespace TaskManager.Handlers.TaskHandlers.Models.AutoImport.SOLCustomAiHandlers
{
    public class TOItemApproveAiHandler : IAutoImportHandler
    {
        public HandlerResult Handle(Attachment attachment)
        {

            // string stamp = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss.FFFFFFF");
            using (Context context = new Context())
            {




                HandlerResult result = new HandlerResult();
                if (attachment.Mail.Sender == "evgeniy.devyatkov@ericsson.com" || attachment.Mail.Sender == "aleksey.chekalin@ericsson.com"
                    || attachment.Mail.Sender == "aleksey.borshchev@ericsson.com"
                    || attachment.Mail.Sender == "aleksey.gorin@ericsson.com"
                    || attachment.Mail.Sender == "aleksandr.solovyov@ericsson.com")
                {
                    string savePath = Path.Combine((global::AutoImport.Rev3.Constants.HandledFilesFolder), DateTime.Now.ToString(@"yyyy\\MM\\dd\\"));
                    try
                    {

                        var wsObjs = EpplusSimpleUniReport.ReadFile(attachment.FilePath, "DRT", 2);
                        var objs = wsObjs.Where(o => o.Column9.ToUpper() == "TRUE" || o.Column9 == "1").ToList();
                        var jobjs = objs.Join(context.ShTOItems, r => r.Column3, i => i.TOItem, (r,i)=>new  {row=r, item=i }).ToList();
                        var unApprovedObjs = wsObjs.Where(o => o.Column9.ToUpper() == "FALSE" || o.Column9 == "0").ToList();
                        List<TOApproveModel> model = new List<TOApproveModel>();
                        foreach (var obj in jobjs)
                        {

                            var to = new TOApproveModel();
                            to.ItemId = obj.row.Column3;
                            to.Approve = obj.row.Column9;
                            to.By = attachment.Mail.Sender;
                            to.Date = obj.item.WorkConfirmedByEricssonDate.HasValue?obj.item.WorkConfirmedByEricssonDate:DateTime.Now;
                            to.LinkToEridoc = obj.row.Column20;
                            DateTime workEndDate;
                            if (DateTime.TryParse(obj.row.Column8, out workEndDate))
                            {
                                to.WorkEndDate = workEndDate;
                            }
                            else
                            {
                                double d;
                                if (double.TryParse(obj.row.Column8, out d))
                                {
                                    try
                                    {
                                        workEndDate = DateTime.FromOADate(d);
                                        to.WorkEndDate = workEndDate;
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                            model.Add(to);
                        }
                        List<TOApproveModel> model2 = new List<TOApproveModel>();
                        foreach (var obj in wsObjs)
                        {
                            var row = new TOApproveModel();
                            row.ItemId = obj.Column3;
                            row.LinkToEridoc = obj.Column20;
                            if (!string.IsNullOrEmpty(row.LinkToEridoc))
                                model2.Add(row);
                        }

                        List<TOApproveModel> model3 = new List<TOApproveModel>();
                        //using (Context context = new Context())
                        //{

                        foreach (var obj in unApprovedObjs)
                        {
                            var shItem = context.ShTOItems.Find(obj.Column3);
                            if (shItem != null)
                                if (shItem.WorkConfirmedByEricsson)
                                    if (string.IsNullOrEmpty(shItem.ActId))
                                    {
                                        var row = new TOApproveModel();
                                        row.ItemId = obj.Column3;
                                        row.Date = null;
                                        row.LinkToEridoc = "";
                                        row.WorkEndDate = null;
                                        model3.Add(row);
                                    }
                                    else
                                    {
                                        result.ErrorsList.Add(string.Format("По позиции {0} уже выпущен акт, и она не может быть разморожена", obj.Column1, shItem.ActId));
                                    }
                        }
                        //}



                        // конвертируем их в дататэйбл, чтобы воспользоваться существующим функционалом
                        if (model.Count > 0)
                        {
                            var dataTable = model.ToDataTable();
                            var WB = NpoiInteract.GetNewWorkBook();
                            // встваляем в нее данные и создаем в ней новый шит
                            NpoiInteract.FillReportData(dataTable, "sheet1", WB);
                            // сохраняем это все по пути назначения
                            string path = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AutoTOItemAppove", attachment.Id, ".xls"));
                            NpoiInteract.SaveReport(path, WB);
                            result.FilesPaths.Add(path);
                        }
                        if (model2.Count > 0)
                        {
                            var dataTable2 = model2.ToDataTable();
                            var WB2 = NpoiInteract.GetNewWorkBook();
                            // встваляем в нее данные и создаем в ней новый шит
                            NpoiInteract.FillReportData(dataTable2, "sheet1", WB2);
                            // сохраняем это все по пути назначения
                            string path2 = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AutoTOItemAppove", attachment.Id + 1, ".xls"));
                            NpoiInteract.SaveReport(path2, WB2);
                            result.FilesPaths.Add(path2);
                        }

                        if (model3.Count > 0)
                        {
                            var dataTable3 = model3.ToDataTable();
                            var WB3 = NpoiInteract.GetNewWorkBook();
                            // встваляем в нее данные и создаем в ней новый шит
                            NpoiInteract.FillReportData(dataTable3, "sheet1", WB3);
                            // сохраняем это все по пути назначения
                            string path3 = Path.Combine(savePath, CommonFunctions.StaticHelpers.GetImportFileName("AutoTOItemAppove(SEC)", attachment.Id + 2, ".xls"));
                            NpoiInteract.SaveReport(path3, WB3);
                            result.FilesPaths.Add(path3);
                        }
                        result.Success = true;
                        return result;
                    }
                    catch (System.Exception ex)
                    {
                        result.ErrorsList.Add("Ошибка работы с файлом  " + ex.Message);
                        result.Success = false;
                        return result;
                    }
                }
                else
                {
                    result.ErrorsList.Add("Извините, у Вас нет прав на использование данного автоимпорта.");
                    result.Success = false;
                    return result;
                }
            }
        }
        public class TOApproveModel
        {
            public string ItemId { get; set; }
            public string Approve { get; set; }
            public string By { get; set; }
            public DateTime? Date { get; set; }
            public DateTime? WorkEndDate { get; set; }
            public string LinkToEridoc { get; set; }
        }

        //public class TOApproveModelLink
        //{
        //    public string ItemId { get; set; }
        //    public string Empty1 { get; set; }
        //    public string Empty2 { get; set; }
        //    public DateTime? Date { get; set; }
        //    public DateTime? FactDate { get; set; }
        //    public string LinkToEridoc { get; set; }
        //}


    }
}

