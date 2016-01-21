//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using TaskManager.Handlers.TaskHandlers.Models.POR;
//using TaskManager.Service;
//using DbModels.DomainModels.ShClone;
//using System.IO;
//using System.Collections;

//namespace TaskManager.Handlers.TaskHandlers.Models.PO
//{
//    public class PORRecalledHandler : ATaskHandler
//    {
//        public PORRecalledHandler(TaskParameters taskParameters)
//            : base(taskParameters) { }
//        public override bool Handle()
//        {
//            List<PORRecallCompleted> ResultList = new List<PORRecallCompleted>();
//            List<ObjectToSH> PORforRecallList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromContext<ObjectToSH>(TaskParameters.Context, "PORforRecall", null);
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество ПО для реколла - {0}", PORforRecallList.Count()));
//            foreach (var por in PORforRecallList)
//            {
//                TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем ПО - {0}", por.Object));
//                ShPOR shPor = SHPORService.GetPOR(por.Object, TaskParameters.Context);
//                if (shPor == null)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("ПОР не найден в таблице ShPORs - {0}", por.Object));
//                    continue;
//                }
//                if (string.IsNullOrEmpty(shPor.PathToFile))
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Не найден путь к файлу для ПОРа - {0}", por.Object));
//                    continue;
//                }
//                if (!File.Exists(shPor.PathToFile))
//                {
//                     TaskParameters.TaskLogger.LogError(string.Format("Файл не найден  - {0}", shPor.PathToFile));
//                    continue;
//                }
//                try
//                {
//                    var info = new FileInfo(shPor.PathToFile);
//                    using (var service = new EpplusService(info))
//                    {
//                        if (shPor.Subcontractor != "FSO")
//                        {
//                            var sht = service.GetSheet("Purchase Request (PR)");
//                            sht.Cells[40, 1].Value = string.Format("Recall request by {0}", shPor.RecallPOby);
//                            if (!string.IsNullOrEmpty(shPor.POnumber))
//                            {
//                                sht.Cells[40, 1].Value += string.Format("PO number {0}", shPor.POnumber);
//                            }
//                        }
//                        service.app.SaveAs(new FileInfo(Path.Combine(TaskParameters.DbTask.EmailSendFolder, string.Format("DEL-{0}.xlsx", shPor.POR))));
//                    }
//                }
//                catch (Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка в обработке файла- {0}", shPor.PathToFile));
//                    continue;
//                }
//                ResultList.Add(new PORRecallCompleted { POR = shPor.POR, RecallPODate = DateTime.Now });
//                TaskParameters.TaskLogger.LogInfo(string.Format("Формирование файла для рекола ПО {0} завершено.", por.Object));
//            }
//            if (ResultList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(ResultList) });
//                TaskParameters.ImportHandlerParams.ImportParams.Add(
//                    new ImportParams
//                    {
//                        ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2,
//                        Objects = new ArrayList(ResultList.Select(s => new {
//                            MUS = string.Format("DEL-{0}.xlsx", s.POR),
//                            DateSend = s.RecallPODate,
//                            POR = s.POR,
//                            MUSType = "PO Recall"
//                        }).ToList()
//                            )
//                    });
            
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество успешно отреколенных ПО - {0}", ResultList.Count()));
//            return true;
//        }
//        private class PORRecallCompleted
//        {
//            public string POR { get; set; }
//            public DateTime RecallPODate { get; set; }
//        }
        
//    }
   
//}
