using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using DbModels.Service;
using WIHInteract;
using DbModels.DomainModels.ShClone;
using System.IO;
using System.Collections;
using CommonFunctions.Extentions;
using MailProcessing;
using TaskManager.Service;

namespace TaskManager.Handlers.TaskHandlers.Models.PO
{
    /// <summary>
    /// Для рекола, как минимум после реджекта, требуется коррекшн комплитед на вих запросе
    /// </summary>
    public class PORecallHandler:ATaskHandler
    {
        public PORecallHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            bool test = true;
            var requestList = new List<ShWIHRequest>();
            var recalled = TaskParameters.Context.ShTOes.Where(t => t.RecallPO && !string.IsNullOrEmpty(t.PONumber)).ToList();
            foreach (var to in recalled)
            {
                // сначала надо проверить, что мы еще не отправили запроса
                DateTime now = DateTime.Now;
                if (WIHService.ReadySendTOWIHRequest(to.TO, WIHInteract.Constants.InternalMailTypeTORecall, TaskParameters.Context))
                {
                    var shWih = TaskParameters.Context.ShWIHRequests.OrderByDescending(w=>w.RequestSentToODdate).FirstOrDefault(t=>t.TOid==to.TO);
                    if(shWih==null)
                    {
                        TaskParameters.TaskLogger.LogError(string.Format("Отсутствует запрос на создание PO для TO:{0}",to.TO));
                        continue;
                    }
                    string recallFilePath = string.Empty;
                    string recallFileName = string.Empty;
                    try
                    {
                       //var porFiles = Directory.GetFiles(TaskParameters.DbTask.ArchiveFolder, shWih.WIHrequests, SearchOption.AllDirectories);
                       // if (porFiles.Count() == 0)
                        {
                            if(File.Exists(TaskParameters.DbTask.TemplatePath))
                            {
                                EpplusService service = new EpplusService(new FileInfo(TaskParameters.DbTask.TemplatePath));
                                var dict = new Dictionary<string, string>();
                                dict.Add("PONumber", to.PONumber);
                                service.ReplaceDataInBook(dict);
                                recallFilePath = TaskParameters.DbTask.ArchiveFolder;
                                recallFileName = string.Format("{0}-{1}-R.xlsx", to.PONumber, DateTime.Now.ToString("yyyyMMddHHmmss"));
                                service.CreateFolderAndSaveBook(recallFilePath, recallFileName);

                            }
                            else
                            {
                                TaskParameters.TaskLogger.LogError("Шаблон рекола не существует по пути " + TaskParameters.DbTask.TemplatePath);
                                continue;
                            }
                            /// новый вариант рекола. стандартизованый шаблон. В шаблоне только номер по
                            /// 
                        }
                        //else
                        //{
                        //    /// старый вариант рекола. требовался файл самого заказа. 
                        //    string porFilePath = porFiles[0];
                        //    string fileName = Path.GetFileName(porFilePath);
                        //    string filePath = Path.GetDirectoryName(porFilePath);
                        //    recallFileName = fileName.Replace("POR-TO", "RECALL-TO");
                        //    recallFilePath = Path.Combine(filePath, recallFileName);
                        //    if (!File.Exists(recallFilePath))
                        //    {
                        //        EpplusService service = new EpplusService(new FileInfo(porFilePath));
                        //        service.app.Workbook.Worksheets.FirstOrDefault().Cells[41, 1].Value = string.Format("Please recall PO: {0}", to.PONumber);
                        //        service.app.Workbook.Worksheets.FirstOrDefault().Cells[42, 1].Value = string.Format("{0}", "PО cancellation agreement is not needed");
                        //        if (!string.IsNullOrEmpty(to.RecallPOComment))
                        //        {
                        //            service.app.Workbook.Worksheets.FirstOrDefault().Cells[43, 1].Value = string.Format("{0}", to.RecallPOComment.CUnidecode());
                        //        }
                        //        service.CreateFolderAndSaveBook(filePath, recallFileName);

                        //    }
                        //}
                       
                       

                    }
                    catch (Exception exc)
                    {
                        TaskParameters.TaskLogger.LogError("Ошибка работы с файлами "+ exc.Message);
                        continue;
                    }

                    string internalMailType = WIHInteract.Constants.InternalMailTypeTORecall;
                   // if (!test)
                    {
                        // повадились слаться двойные запросы, что вызывает два ответа: компли и реджект. реджект затирает комплит и новый пор не уходит.
                        if (requestList.Any(r => r.TOid == to.TO))
                        {
                            RedemptionMailProcessor proc = new RedemptionMailProcessor("SOLARIS");
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("ДВОЙНОЙ РЕКОЛЛ ЗАПРОС!!!");
                            builder.AppendLine("Информация:");
                            builder.AppendLine(string.Format("Выборка: ", string.Join(",", recalled.Select(r => r.TO))));
                            builder.AppendLine(string.Format("RequestList: ", string.Join(",", requestList.Select(r => r.TOid))));
                         

                            proc.SendMail(new global::Models.AutoMail() { Email = "aleksey.gorin@ericsson.com", Subject = "Double RECALL", Body = builder.ToString() });
                            continue;


                        }
                        
                        
                        WIHMailInformation mailInf = new WIHMailInformation();
                        mailInf.InternalMailType = internalMailType;
                        mailInf.MailBoxSigmun = "ESOLARIS";
                        mailInf.FilePath = Path.Combine(recallFilePath,recallFileName);

                        mailInf.Project = "MS-SOLARIS";
                        mailInf.Subject = "PO WIH Request";
                        mailInf.Email = "technical.box.for.solaris@ericsson.com";
                        mailInf.ResponsibleTeam = "ROD Sofia";
                        mailInf.SystemComponent = "Purchase Order Request (POR)";
                        //mailInf.SystemComponent = WIHInteract.Constants.WIHMailTypePORRecallASTELIT;
                        mailInf.CertificationCode = "L2302RODSofia_AO";
                        mailInf.Description = recallFileName;
                        var result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS",test);
                        if (string.IsNullOrEmpty(result) || (string.IsNullOrWhiteSpace(result)))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Функция отправки письма не вернула ConversationIndex "));
                        }
                        else
                        {
                            
                            requestList.Add(new ShWIHRequest() { TOid = to.TO, WIHrequests = recallFileName, RequestSentToODdate = now, Type = WIHInteract.Constants.InternalMailTypeTORecall });
                        }
                    }
                    //else
                    //{
                    //    System.Diagnostics.Debug.WriteLine(recallFilePath);
                    //}
                }
            }

            if (requestList.Count > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
            }
            return true;
           
        }
    }
}
