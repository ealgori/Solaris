//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.Collections;
//using DbModels.DomainModels.ShClone;
//using TaskManager.Service;
//using CommonFunctions.StaticHelper;
//using System.IO;
//using CommonFunctions.Extentions;

//namespace TaskManager.Handlers.TaskHandlers.Models.MUSForms
//{
//    public class MUSSignedHandler : ATaskHandler
//    {
//        public MUSSignedHandler(TaskParameters taskParameters) : base(taskParameters) { }
//        public override bool Handle()
//        {
//            List<MUSSignedProcModel> WOList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromContext<MUSSignedProcModel>(TaskParameters.Context, "MUS_Signed", null);
//            List<MUSSignedImportModel> ResultList = new List<MUSSignedImportModel>();
//            TaskParameters.TaskLogger.LogInfo(string.Format("Выполнили хранимку MUS_Signed. Количество полученных записей - {0}", ResultList.Count));
//            foreach (var WO in WOList)
//            {
//                try
//                {
//                    ProcessWO(WO, ref ResultList);
//                }
//                catch (Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при формировании МУС на бук/адванс; Заказ - {0}; Текст ошибки {1}", WO.WO, ex.Message));
//                }

//            }
//            if (ResultList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(ResultList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество отправленных в ОД мус форм на бук/адванс - {0}", ResultList.Count));
//            return true;
//        }
//        private void ProcessWO(MUSSignedProcModel WO, ref List<MUSSignedImportModel> resultList)
//        {
//            TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем заказ - {0}", WO.WO));
//            FileInfo templateFile = new FileInfo(TaskParameters.DbTask.TemplatePath);
//            using (var service = new EpplusService(templateFile))
//            {
//                ShWO shWO = SHWOService.GetWO(WO.WO, TaskParameters.Context);
//                #region Заполнение листа с позициями
//                TaskParameters.TaskLogger.LogInfo("Заполняем лист с позициями");
//                var shtAtt = service.GetSheet("Attachment");
//                List<MUSSignedAttachmentModel> AttList = shWO.GetWoItems(TaskParameters.Context).Select(s => new MUSSignedAttachmentModel
//                {
//                    SO = "ECR-BILL",
//                    Quantity = s.Quantity.Value.ToString(),
//                    Type = "PC",
//                    Code = s.Code,
//                    Price = s.GetItemPrice(WO.Region,TaskParameters.Context).ToString(),
//                    Currency = "RUB",
//                    WO = shWO.WO,
//                    Comments = ""
//                }).ToList();
//                service.InsertTableToPatternCellInWorkBook("Table", AttList.ToDataTable(typeof(MUSSignedAttachmentModel)),
//                           new EpplusService.InsertTableParams());
//                #endregion
//                #region Замена Данных
//                TaskParameters.TaskLogger.LogInfo("Заменяем данные");
//                Dictionary<string, string> dict = new Dictionary<string, string>();
//                foreach (var item in StaticHelpers.GetProperties(WO))
//                {
//                    dict.Add(item.Name, item.GetValueExt(WO).ToString());

//                }
//                #region Считаем плановые косты для заказа
//                var paramDict = new Dictionary<string, object>();
//                paramDict.Add("@SOW", WO.SOW);
//                var PlannedCosts = StaticHelpers.GetStoredProcDataFromContext<CostsTable>(TaskParameters.Context, "MUS_WBSrequest_Costs", paramDict).FirstOrDefault();
//                if (PlannedCosts == null || PlannedCosts.TotalCost == 0)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Заказ - {0}; Не удалось посчитать плановые косты. Проверьте Табличку ItemCosts ", shWO.WO));
//                    return;
//                }
//                dict.Add("PlannedCosts", PlannedCosts.TotalCost.ToString());
//                #endregion            
//                //Вот это мне совсем не нравится. Убогость какая-то.
//                #region Пытаемся найти позицию тайп сайта
//                decimal? InvoiceAmount = GetAdvanceValue(shWO,WO.Region);
//                string HasInvoice = null;
//                DateTime? InvoiceDate = null;
//                if (InvoiceAmount != null)
//                {
//                    HasInvoice = "Yes";
//                    InvoiceDate = DateTime.Now;
//                }
//                dict.Add("InvoiceAmount", InvoiceAmount.ToString());
//                dict.Add("HasInvoice", HasInvoice);
//                dict.Add("InvoiceDate", InvoiceDate.HasValue ? InvoiceDate.Value.ToShortDateString() : "");
//                #endregion
//                service.ReplaceDataInBook(dict);
//                #endregion
//                #region Сохранение и добавление в итоговый набор
//                TaskParameters.TaskLogger.LogInfo("Сохраняем");

//                //Убого определяем тип мус формы
//                //Тут тоже надо что-то придумать
//                string MUSType = WO.WO.Contains("TS") ? "Adv" : "Book";

//                String fileName = string.Format("MUS-{0}-{1}-{2}-{3}.xlsx", WO.MacroRegionShortName, MUSType, DateTime.Now.ToString("yyMMddHHmm"), WO.Site.RemoveBadChars());
//                string filePath = Path.Combine(TaskParameters.DbTask.EmailSendFolder, fileName);
//                //Пробуем сохранить файл
//                try
//                {
//                    service.app.SaveAs(new FileInfo(filePath));

//                }
//                catch (Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла {0} - {1}", fileName, ex.Message));
//                    return;
//                }
//                TaskParameters.TaskLogger.LogInfo("Добавляем заказ в итоговый набор");
//                resultList.Add(new MUSSignedImportModel
//                {
//                    MUSName = fileName,
//                    MUSSent = DateTime.Now,
//                    MUSType = "Signed",
//                    WO = shWO.WO,
//                    WBS = WO.WBS,
//                    MacroRegion = WO.MacroRegion
//                });

//                #endregion
//            }
//            TaskParameters.TaskLogger.LogInfo("Заказ обработан");

//        }
//        /// <summary>
//        /// Бредовая функция. Поиск тайп сайтовой позиции и возвращение половины ее стоимости.
//        /// Сейчас я ищу все тайп сайтовые позиции, суммирую их и делю на 2
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <returns></returns>
//        private decimal? GetAdvanceValue(ShWO wo, string Region)
//        {
//            List<ShWOItem> TSItemsList = new List<ShWOItem>();
//            foreach (var ParentWo in SHWOService.GetParentWos(wo, TaskParameters.Context, true))
//            {
//                TSItemsList.AddRange(ParentWo.GetWoItems(TaskParameters.Context).Where(i => i.Service == "Type Site").ToList());
//            }
//            if (TSItemsList.Count > 0)
//            {
//                return TSItemsList.Sum(s => Math.Round(s.GetItemTotalPrice(Region, TaskParameters.Context) / 2, 2));
//            }
//            return null;

//        }
//        private class MUSSignedProcModel
//        {
//            public string WO { get; set; }
//            public string CPMName { get; set; }
//            public string CPMSignum { get; set; }
//            public string CPMFullName { get; set; }
//            public string ProjectName { get; set; }
//            public string ProjectDefinition { get; set; }
//            public string SuperiorWBS { get; set; }
//            public string WBSName { get; set; }
//            public string ShipToParty { get; set; }
//            public string Element { get; set; }
//            public string WBS { get; set; }
//            public string SO { get; set; }
//            public string Network { get; set; }
//            public decimal? WoTotal { get; set; }
//            public string Site { get; set; }
//            public string MacroRegionShortName { get; set; }
//            public string Region { get; set; }
//            public string SOW { get; set; }
//            public string LinkToWoSigned { get; set; }
//            public string MacroRegion { get; set; }
//        }
//        private class MUSSignedImportModel
//        {
//            public string MUSName { get; set; }
//            public DateTime MUSSent { get; set; }
//            public string MUSType { get; set; }
//            public string WO { get; set; }
//            public string WBS { get; set; }
//            public string MacroRegion { get; set; }
//        }
//        private class MUSSignedAttachmentModel
//        {
//            public string SO { get; set; }
//            public string Quantity { get; set; }
//            public string Type { get; set; }
//            public string Code { get; set; }
//            public string Price { get; set; }
//            public string Currency { get; set; }
//            public string WO { get; set; }
//            public string Comments { get; set; }
//        }
//    }

//}
