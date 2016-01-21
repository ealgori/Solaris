using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using EpplusInteract;
using WIHInteract;
using System.IO;
using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System.Collections;
using TaskManager.Service;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class SendWIHPORRequests:ATaskHandler
    {
        public SendWIHPORRequests(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            var readyToSendTOs = TaskParameters.Context.ShTOes.Where(t => !string.IsNullOrEmpty(t.TOTotalAmmountApproved)&& string.IsNullOrEmpty(t.PONumber)).ToList();
            var testTO = "ТЮМЕНЬ_ТО_АУГПТ_2";
            bool test = true;
            if (test)
                readyToSendTOs = TaskParameters.Context.ShTOes.Where(t => t.TO == "ТЮМЕНЬ_ТО_АУГПТ_2").ToList();
            List<ShWIHRequest> requestList = new List<ShWIHRequest>();
            foreach (var readyToSendTO in readyToSendTOs)
            {
                if (WIHService.ReadySendTOWIHRequest(readyToSendTO.TO, WIHInteract.Constants.InternalMailTypeTOPOR, TaskParameters.Context))
                {
                    TORepository repository = new TORepository(TaskParameters.Context);
                    var satTo = repository.GetLastSATTOList().FirstOrDefault(s=>s.TO==readyToSendTO.TO&&s.UploadedToSh);
                    if (satTo != null)
                    {
                        var now = DateTime.Now;
                        string fileName = GenerateTOPorName(now);
                        if (fileName.Length > 42)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Название файла больше 42 символов '{0}'", fileName));
                            continue;
                        }
                        string fileName1 = GeneratedTORequestName(now);

                        var porBytes = ExcelParser.EpplusInteract.CreateTOPOR.CreatePorFile(satTo.Id);
                        if (porBytes == null)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка при генерации пора:'{0}' - id:'{1}'",readyToSendTO.TO, satTo.Id));
                            continue;
                        }
                        var docTOBytes = ExcelParser.EpplusInteract.CreateTORequest.CreateTORequestFile(satTo.Id,fileName);
                        if (docTOBytes == null)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка при генерации ТО запроса:'{0}' - id:'{1}'", readyToSendTO.TO, satTo.Id));
                            continue;
                        }
                   
                       

                        // сохраним файл пора в архив
                        var archive = Path.Combine(TaskParameters.DbTask.ArchiveFolder, now.ToString(@"yyyy\\MM\\dd"));
                        if (!Directory.Exists(archive))
                        {
                            try
                            {
                                Directory.CreateDirectory(archive);
                            }
                            catch(Exception exc)
                            {
                                TaskParameters.TaskLogger.LogError(string.Format("Ошибка создания папки  '{0}'; {1}", archive, exc.Message));
                                continue;
                            }
                        }
                        var filePath = Path.Combine(archive,fileName);
                        if(!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath,porBytes))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath));
                            continue;
                        }
                        var filePath1 = Path.Combine(archive, fileName1);
                        if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath1, docTOBytes))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath1));
                            continue;
                        }


                        string internalMailType = WIHInteract.Constants.InternalMailTypeTOPOR;
                        WIHMailInformation mailInf = new WIHMailInformation();
                        mailInf.InternalMailType = internalMailType;
                        mailInf.FilePath = filePath;
                        mailInf.FilePath2 = filePath1;
                        mailInf.MailBoxSigmun = "ESOLARIS";
                        mailInf.Project = "MS-SOLARIS";
                        mailInf.Subject = "PO WIH Request";
                        mailInf.Email = "technical.box.for.solaris@ericsson.com";
                        mailInf.ResponsibleTeam = "ROD Sofia";
                        mailInf.SystemComponent = "Purchase Order Request (POR)";
                        mailInf.CertificationCode = "L2302RODSofia_AO";

                        var result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS");
                        if (string.IsNullOrEmpty(result) || (string.IsNullOrWhiteSpace(result)))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Функция отправки письма не вернула ConversationIndex "));
                        }
                        else
                        {
                          
                            requestList.Add(new ShWIHRequest() { TOid = satTo.TO, WIHrequests = fileName, RequestSentToODdate = now, Type = WIHInteract.Constants.InternalMailTypeTOPOR });
                        }
                        SHInteract.Handlers.Solaris.UploadTOPOR.Handle(filePath, filePath1, satTo.TO);
                    }
                }
            }
            if (requestList.Count > 0)
            {
               
                
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
            }

            return true;




           
        }


     

        public string GenerateTOPorName(DateTime date)
        {
            return string.Format("POR-TO-{0}.xlsx",date.ToString("yyyyMMdd_HHmmss_ffff"));
        }
        public string GeneratedTORequestName(DateTime date)
        {
            return string.Format("TOR-TO-{0}.xlsm", date.ToString("yyyyMMdd_HHmmss_ffff"));
        }
    }
}
