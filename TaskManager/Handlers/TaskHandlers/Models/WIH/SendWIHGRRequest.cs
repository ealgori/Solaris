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
using CommonFunctions.Extentions;
using WIHInteract;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    /// <summary>
    /// 03.03.2016 написана, и изначально не используется. Причина- необохдимость наличия фактур в запросах GR
    /// Типа тогда ЛАС делает двойную работу, так как и фактуры и GR забивает он.
    /// Решено предоставлять им ежедневную рассылку с авр и его позициями на которых проставлено подтверждение выполения работ.
    /// Скорее всего производиться будет сх сендером
    /// 
    /// </summary>
    public class SendWIHGRRequest : ATaskHandler
    {
        public SendWIHGRRequest(TaskParameters taskParams) : base(taskParams)
        {

        }
        public override bool Handle()
        {
            bool test = false;
            bool jogging = true;
            string testAvr = "207241";
            List<string> poList = new List<string>();

            // текущая дата больше, чем эта и два месяца и первое число.
            List<ShWIHRequest> requestList = new List<ShWIHRequest>();
            var now = DateTime.Now;
            var confGrAVRs = TaskParameters.Context.ShAVRs.Where(a =>
             a.FactVypolneniiaRabotPodtverzhdaiuCB == true
            && a.WorkEnd.HasValue

             && !string.IsNullOrEmpty(a.PurchaseOrderNumber)
            && (a.Year == "2016" || a.Year == "2017")
            && !a.GRCreated.HasValue
            // )
            //a.PurchaseOrderNumber == "4512444164" ||
            //a.PurchaseOrderNumber == "4513003468" ||
            //a.PurchaseOrderNumber == "4513041239" ||
            //a.PurchaseOrderNumber == "4513041272" ||
            //a.PurchaseOrderNumber == "4513041371" ||
            //a.PurchaseOrderNumber == "4513041822" ||
            //a.PurchaseOrderNumber == "4513041942" ||
            //a.PurchaseOrderNumber == "4513042244" ||
            //a.PurchaseOrderNumber == "4513042411" ||
            //a.PurchaseOrderNumber == "4513042509" ||
            //a.PurchaseOrderNumber == "4513042728" ||
            //a.PurchaseOrderNumber == "4513042881" ||
            //a.PurchaseOrderNumber == "4513042884" ||
            //a.PurchaseOrderNumber == "4513042915" ||
            //a.PurchaseOrderNumber == "4513042958" ||
            //a.PurchaseOrderNumber == "4513042990" ||
            //a.PurchaseOrderNumber == "4513043027" ||
            //a.PurchaseOrderNumber == "4513043032" ||
            //a.PurchaseOrderNumber == "4513043056" ||
            //a.PurchaseOrderNumber == "4513043094" ||
            //a.PurchaseOrderNumber == "4513043128" ||
            //a.PurchaseOrderNumber == "4513043210" ||
            //a.PurchaseOrderNumber == "4513043237" ||
            //a.PurchaseOrderNumber == "4513043286" ||
            //a.PurchaseOrderNumber == "4513043332" ||
            //a.PurchaseOrderNumber == "4513043392" ||
            //a.PurchaseOrderNumber == "4513043518" ||
            //a.PurchaseOrderNumber == "4513043592" ||
            //a.PurchaseOrderNumber == "4513043682" ||
            //a.PurchaseOrderNumber == "4513044215" ||
            //a.PurchaseOrderNumber == "4513044576" ||
            //a.PurchaseOrderNumber == "4513044751" ||
            //a.PurchaseOrderNumber == "4513045187" ||
            //a.PurchaseOrderNumber == "4513045357" ||
            //a.PurchaseOrderNumber == "4513028594" ||
            //a.PurchaseOrderNumber == "4512444164"




            )
            // смотрим, что позже, дата выпуска по или время окончания работ, и от этого позднего высчитываем TwoMonthRange
            .ToList()
            //.Where(a =>
                ////Max(a.WorkEnd, a.DataVipuskaPO) // это не включать
                //a.WorkEnd.TwoMonthRange(now)) // 02.06.2016 - решено отменить эту практику
              //.ToList()
              ;

            if (test)
            {
                var _confGrAVRs = confGrAVRs.Where(a => a.AVRId == testAvr).ToList();
                confGrAVRs = TaskParameters.Context.ShAVRs.Where(a => a.AVRId == testAvr).ToList();
                var res = confGrAVRs.FirstOrDefault().WorkEnd.TwoMonthRange(DateTime.Now);

            }

            var _cachedWihRequests = TaskParameters.Context.ShWIHRequests.Where(w => w.Type == WIHInteract.Constants.InternalMailTypeAVRGR).ToList();
            var _cachedSATPors = TaskParameters.Context.AVRPORs.ToList();

            foreach (var avr in confGrAVRs)
            {
                var wihRequests = _cachedWihRequests.Where(r => r.AVRId == avr.AVRId).ToList();
                if (WIHService.RequestCanBeSended(wihRequests, WIHInteract.Constants.InternalMailTypeAVRGR))
                {
                    var satPors = _cachedSATPors.Where(p => p.AVRId == avr.AVRId).ToList();
                    if (satPors.Count > 0)
                    {
                        var satPor = satPors.OrderByDescending(s => s.PrintDate).FirstOrDefault();
                        var grBytes = ExcelParser.ExcelParser.CreateGR.CreateGRFile(satPor.Id, avr.PurchaseOrderNumber, TaskParameters.DbTask.TemplatePath);
                        if (grBytes == null)
                        {
                            TaskParameters.TaskLogger.LogError(string.Format("Ошибка генерации GR для авр: {0}", avr.AVRId));
                            continue;
                        }
                        // сохраним файл GR в архив
                        var grFileName = GenerateGRName(avr.AVRId, avr.PurchaseOrderNumber,jogging);
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
                        var mailInf = MailInfoFactory.GetGRInfo(internalMailType, filePath);
                        poList.Add(avr.PurchaseOrderNumber);
                        if (!jogging)
                        {
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

            }
            var bytes = NpoiInteract.DataTableToExcel(poList.ToDataTable());
            CommonFunctions.StaticHelpers.ByteArrayToFile(@"C:\Temp\123\avrGRLogs.xls", bytes);

            if (requestList.Count > 0)
            {


                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(requestList) });
            }
            return true;
        }

        private string GenerateGRName(string avrId, string po, bool jogging)
        {
            return string.Format("GR-{0}-{1}-{3}{4}{2}", avrId, po, Path.GetExtension(TaskParameters.DbTask.TemplatePath), DateTime.Now.ToString("ddMMyyyy"),jogging?"-N":"");
        }


    }
}
