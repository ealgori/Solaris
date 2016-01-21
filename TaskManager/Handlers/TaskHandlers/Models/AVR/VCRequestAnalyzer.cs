using DbModels.DomainModels.ShClone;
using MailProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class VCRequestAnalyzer : ATaskHandler
    {
        public VCRequestAnalyzer(TaskParameters taskParameters)
            : base(taskParameters)
        {

        }
        public override bool Handle()
        {
            RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS");
            var mails = processor.GetMails(new List<string> { Common.AVRCommon.AcceptMask, Common.AVRCommon.RejectMask }, alternativeSearch: true);
            List<ShVCRequestImport> importModels = new List<ShVCRequestImport>();
            //List<AVRUnfreeezeImportModel> unfreezeModels = new List<AVRUnfreeezeImportModel>();
            foreach (var mail in mails)
            {
                var requestName = mail.Subject.Replace(Common.AVRCommon.AcceptMask + ":", "").Replace(Common.AVRCommon.RejectMask + ":", "").Trim();
                var shVCRequest = TaskParameters.Context.ShVCRequests.FirstOrDefault(r => r.Id == requestName);
                if (shVCRequest != null)
                {
                    if (mail.Subject.Contains(Common.AVRCommon.AcceptMask))
                    {



                        if (!shVCRequest.RequestAccepted.HasValue && !shVCRequest.RequestRejected.HasValue)
                        {
                            var model = new ShVCRequestImport();
                            model.Id = shVCRequest.Id;
                            model.RequestAccept = DateTime.Now;
                            importModels.Add(model);

                        }
                        else
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Информация по запросу: {0} уже была внесена в сх ранее", requestName));
                        }
                    }
                    else
                    {
                        if (!shVCRequest.RequestRejected.HasValue && !shVCRequest.RequestAccepted.HasValue)
                        {
                            var model = new ShVCRequestImport();
                            model.Id = shVCRequest.Id;
                            model.RequestReject = DateTime.Now;
                            model.RejectReason = mail.Body;
                            importModels.Add(model);
                            //unfreezeModels.Add(new AVRUnfreeezeImportModel() { AVRId = shVCRequest.ShAVRs.AVRId });

                        }
                    }


                    var path = Common.AVRCommon.GetAVRArhivePath(requestName);
                    foreach (var attach in mail.Attachments)
                    {
                        var savePath = Path.Combine(path, attach.File);
                        try
                        {
                            File.Copy(attach.FilePath, savePath);
                        }
                        catch (Exception ex)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка сохранения файла по пути:{0}", savePath));

                        }
                    }

                    try
                    {
                        processor.SaveMailToFile(mail.ConversationId, "Inbox", path);
                    }
                    catch (Exception exc)
                    {

                        TaskParameters.TaskLogger.LogError("Ошибка сохранения письма в файл");
                    }
                    processor.MoveToSuccess(mail.ConversationId);
                }
                else
                {
                    TaskParameters.TaskLogger.LogError(string.Format("Запрос не существует в сх: {0}", requestName));
                }

            }



            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });
          //  TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(unfreezeModels) });
                return true;
            
        }
    }
}
