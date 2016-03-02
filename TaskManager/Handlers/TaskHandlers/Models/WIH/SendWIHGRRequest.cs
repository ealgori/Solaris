using DbModels.DomainModels.ShClone;
using DbModels.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using WIHInteract;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class SendWIHGRRequest:ATaskHandler
    {
        public SendWIHGRRequest(TaskParameters taskParams):base(taskParams)
        {

        }
        public override bool Handle()
        {
            bool test = false;
            string testAvr = "205490";
            List<ShWIHRequest> requestList = new List<ShWIHRequest>();

            var confGrAVRs = TaskParameters.Context.ShAVRs.Where(a =>
                a.FactVypolneniiaRabotPodtverzhdaiuCB == true
                && !string.IsNullOrEmpty(a.PurchaseOrderNumber)
            ).ToList();

            if (test)
                confGrAVRs = TaskParameters.Context.ShAVRs.Where(a => a.AVRId == testAvr).ToList();

            var _cachedWihRequests = TaskParameters.Context.ShWIHRequests.Where(w => w.Type == WIHInteract.Constants.InternalMailTypeAVRGR).ToList();
            var _cachedSATPors = TaskParameters.Context.AVRPORs.ToList();
            var now = DateTime.Now;
            foreach (var avr in confGrAVRs)
            {
                var wihRequests = _cachedWihRequests.Where(r => r.AVRId == avr.AVRId).ToList();
                if (WIHService.RequestCanBeSended(wihRequests, WIHInteract.Constants.InternalMailTypeAVRGR))
                {
                    var satPors = _cachedSATPors.Where(p => p.AVRId == avr.AVRId).ToList();
                    if (satPors.Count > 0)
                    {
                        var satPor = satPors.OrderByDescending(s => s.PrintDate).FirstOrDefault();
                        var grBytes = ExcelParser.ExcelParser.CreateGR.CreateGRFile(satPor.Id, avr.PurchaseOrderNumber , TaskParameters.DbTask.TemplatePath);
                        if (grBytes == null)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка генерации GR для авр: {0}", avr.AVRId));
                            continue;
                        }
                        // сохраним файл GR в архив
                        var grFileName = GenerateGRName(avr.AVRId, avr.PurchaseOrderNumber);
                        var archive = Path.Combine(TaskParameters.DbTask.ArchiveFolder, now.ToString(@"yyyy\\MM\\dd"));
                        if (!Directory.Exists(archive))
                        {
                            try
                            {
                                Directory.CreateDirectory(archive);
                            }
                            catch (Exception exc)
                            {
                                TaskParameters.TaskLogger.LogError(string.Format("Ошибка создания папки  '{0}'; {1}", archive, exc.Message));
                                continue;
                            }
                        }
                        var filePath = Path.Combine(archive, grFileName);
                        if (!CommonFunctions.StaticHelpers.ByteArrayToFile(filePath, grBytes))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла:'{0}'", filePath));
                            continue;
                        }

                        string internalMailType = WIHInteract.Constants.InternalMailTypeAVRGR;
                        WIHMailInformation mailInf = new WIHMailInformation();
                        mailInf.InternalMailType = internalMailType;
                        mailInf.FilePath = filePath;
                        mailInf.MailBoxSigmun = "ESOLARIS";
                        mailInf.Project = "MS-SOLARIS";
                        mailInf.Subject = "GR WIH Request";
                        mailInf.Email = "technical.box.for.solaris@ericsson.com";
                        mailInf.ResponsibleTeam = "ROD Sofia";
                        mailInf.SystemComponent = "Other";
                        mailInf.CertificationCode = "L2302RODSofia_AO";

                        var result = WIHInteractor.SendMailToWIHRussia(mailInf, "SOLARIS", test);
                        if (string.IsNullOrEmpty(result) || (string.IsNullOrWhiteSpace(result)))
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Функция отправки письма не вернула ConversationIndex "));
                        }
                        else
                        {

                            requestList.Add(new ShWIHRequest() { AVRId = avr.AVRId, WIHrequests = grFileName, RequestSentToODdate = now, Type = WIHInteract.Constants.InternalMailTypeAVRGR });
                        }

                    }

                }
               
            }
            if (requestList.Count > 0)
            {


                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
            }
            return true;
        }

        private string GenerateGRName(string avrId, string po)
        {
            return string.Format("GR-{0}-{1}{2}", avrId, po, Path.GetExtension(TaskParameters.DbTask.TemplatePath));
        }
    }
}
