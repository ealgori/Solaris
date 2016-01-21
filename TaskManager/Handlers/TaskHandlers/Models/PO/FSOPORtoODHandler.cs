//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.IO;
//using TaskManager.Service;
//using CommonFunctions.StaticHelper;
//using OfficeOpenXml;
//using TaskManager.Handlers.TaskHandlers.Models.PO;
//using DbModels.DomainModels.ShClone;
//using DbModels.DomainModels.Base;
//using CommonFunctions.Extentions;
//using System.Collections;
//using System.Globalization;
//using DbModels.DataContext;
//using System.Drawing;
//using DbModels.Models;
//namespace TaskManager.Handlers.TaskHandlers.Models.PO
//{
//    public class FSOPORtoODHandler : ATaskHandler
//    {
//        public FSOPORtoODServiceData data { get; set; }
//        public FSOPORtoODHandler(TaskParameters taskParameters)
//            : base(taskParameters)
//        {
//            data = new FSOPORtoODServiceData
//            {
//                //Контекст БД
//                context = taskParameters.Context
//            };
//        }
//        public override bool Handle()
//        {
//            List<FSOMUStoODProc> PORList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromContext<FSOMUStoODProc>(TaskParameters.Context, "SendFSOPORtoOD", null);
//            List<PORSentToOD> ResultList = new List<PORSentToOD>();
//            foreach (var POR in PORList)
//            {
//                try
//                {
//                    data.POR = POR;
//                    ProcessPOR(data.POR, ref ResultList);
//                }
//                catch (Exception ex)
//                {
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при формировании МУС ПО на ФСО для обьекта ПОР {0}:{1}", data.POR.POR_Id, ex.Message));
//                }

//            }
//            if (ResultList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(ResultList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество отправленных в ОД мус форм на ФСО - {0}", ResultList.Count));
//            return true;
//        }
//        #region Вспомогательные методы
//        public bool ReplaceData()
//        {
//            #region Заменяем поля в файле
//            Dictionary<string, string> dict = new Dictionary<string, string>();
//            foreach (var item in StaticHelpers.GetProperties(data.POR))
//            {
//                dict.Add(item.Name, item.GetValueExt(data.POR).ToString());

//            }
//            double? Dollar = CommonFunctions.ExchangeRates.CurrencyRates.GetExchangeRateByValuteStringCode("USD");
//            if (Dollar == null)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("Не работает процедура получения курса валюты из интернета!"));
//                return false;
//            }
//            dict.Add("PlannedCostsDollar", Math.Round(data.POR.PlannedCosts / (decimal)Dollar, 2).ToString());
//            data.service.ReplaceDataInBook(dict);
//            return true;
//            #endregion
//        }
//        public bool FillAttachment()
//        {
//            //#region Заполняем таблицу на третьем листе
//            //ExcelWorkbook book = data.service.app.Workbook;
//            //ExcelWorksheet shtAttachment = data.service.GetSheet("Attachment");
//            //List<POItemStoredProcClass> tableList = new List<POItemStoredProcClass>();
//            //List<ShPORItem> PORItemList = TaskParameters.Context.ShPORItems.Where(p => p.POR_Id == data.POR.POR_Id).ToList();
//            //int i = 1;
//            //List<EpplusService.DrawParam> DrawParams = new List<EpplusService.DrawParam>();
//            //foreach (var item in PORItemList)
//            //{
//            //    var tableRow = new POItemStoredProcClass();
//            //    #region Общие поля

//            //    tableRow.No = i;
//            //    tableRow.Plant = "2349";
//            //    tableRow.NetQty = item.Quantity.Value;
//            //    tableRow.PRtype = "3";
//            //    tableRow.POrg = "1439";
//            //    tableRow.PRUnit = "1";
//            //    tableRow.Code = item.SAPItemCode;
//            //    tableRow.Description = item.Description;
//            //    tableRow.Price = item.Price.Value;
//            //    tableRow.Curr = "RUB";
//            //    #endregion

//            //    #region Разные поля для материалов и нематериалов


//            //    //Для ФСО нет номера вендора
//            //    //tableRow.Vendor = SHPORService.GetVendorNumber(item.Subcontractor),
//            //    tableRow.Plandate = item.WorkEnd.Value;
//            //    SAPCode sapCode = TaskParameters.Context.SAPCodes.FirstOrDefault(c => c.Code == item.SAPItemCode);
//            //    if (sapCode == null)
//            //    {
//            //        TaskParameters.TaskLogger.LogError(string.Format("Сап код {0} для ПОРА {1} не найден в БД", item.SAPItemCode, data.POR.POR_Id));
//            //        return false;
//            //    }
//            //    if (sapCode.Type == "MAT")
//            //    {
//            //        tableRow.Cat = "Material";
//            //        tableRow.ItemCat = "L";
//            //        tableRow.GLacc = "402201";
//            //    }
//            //    else
//            //    {
//            //        tableRow.Cat = "Service";
//            //        tableRow.ItemCat = "N";
//            //        tableRow.GLacc = "402601";
//            //    }
//            //    #endregion
//            //    #region Демонтаж

//            //    if (item.PORDismounting.Value)
//            //    {
//            //        //Раскрасить строчку
//            //        DrawParams.Add(new EpplusService.FillRow { Color = Color.Bisque, RowNum = i });
//            //    }

//            //    #endregion
                    
//            //    tableList.Add(tableRow);
//            //    i++;
//            //}

//            ////// вставка таблицы
//            //data.service.InsertTableToPatternCellInWorkBook("Table", tableList.ToDataTable(typeof(POItemStoredProcClass)),
//            //    new EpplusService.InsertTableParams() { PrintHeaders = false, DrawParams = DrawParams
//            //   });
//            //return true;
//            //#endregion
//            return false;
//        }
//        public bool Save(string fileName)
//        {
//            #region Сохранение
//            // сохранение
//            string filePath = Path.Combine(TaskParameters.DbTask.EmailSendFolder, fileName);


//            //Пробуем сохранить файл
//            try
//            {
//                data.service.app.SaveAs(new FileInfo(filePath));

//            }
//            catch (Exception ex)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла {0} - {1}", fileName, ex.Message));
//                return false;
//            }

//            return true;
//            #endregion
//        }
//        public string SaveToSandBox(string fileName)
//        {
//            string SandBoxfilePath = Path.Combine(TaskParameters.DbTask.ArchiveFolder, DateTime.Now.ToString("yyyy\\MM\\dd\\HHmmss"), fileName);
//            try
//            {
//                data.service.app.SaveAs(new FileInfo(SandBoxfilePath));
//                return SandBoxfilePath;
//            }
//            catch (Exception ex)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("Ошибка при сохранении файла в Sandbox{0} - {1}", fileName, ex.Message));
//                return string.Empty;
//            }
//        }
//        #endregion
//        /// <summary>
//        /// Обработка конкретного ПОРа
//        /// </summary>
//        /// <param name="POR"></param>
//        /// <param name="ResultList"></param>
//        public void ProcessPOR(FSOMUStoODProc POR, ref List<PORSentToOD> ResultList)
//        {
//            TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем ПОР - {0}", POR.POR_Id));
//            ShPOR shPOR = SHPORService.GetPOR(POR.POR_Id, TaskParameters.Context);
//            if (POR == null)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("ПОР {0} не найден!", POR.POR_Id));
//                return;
//            }
//            //Проверка на позиции ECR Add
//            if (!shPOR.MultiplePositionsCorrect(TaskParameters.Context))
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("В ПОРе {0} найдены пока еще неодобренные Ваней позиции ECR Add", POR.POR_Id));
//                return;
//            }
//            FileInfo templateFile = new FileInfo(TaskParameters.DbTask.TemplatePath);
//            using (data.service = new EpplusService(templateFile))
//            {
//                TaskParameters.TaskLogger.LogInfo(string.Format("Заменяем данные в книге"));
//                if (!ReplaceData())
//                {
//                    return;
//                }
//                TaskParameters.TaskLogger.LogInfo(string.Format("Заполняем страницу с позициями"));
//                if (!FillAttachment())
//                {
//                    return;
//                }
//                string fileName = string.Format("MUS-{0}.xlsx", data.POR.POR_Id);
//                TaskParameters.TaskLogger.LogInfo(string.Format("Сохраняем"));
//                if (!Save(fileName))
//                {
//                    return;
//                }
//                //Добавляем пор к итоговому списку
//                ResultList.Add(new PORSentToOD { POR = POR.POR_Id, DateSent = DateTime.Now, PathToFile = SaveToSandBox(fileName) });
//                TaskParameters.TaskLogger.LogInfo(string.Format("ПОР обработан и добавлен в итогововый набор"));
//            }
//        }
//    }
//    public class PORSentToOD
//    {
//        public string POR { get; set; }
//        public DateTime DateSent { get; set; }
//        public string PathToFile { get; set; }
//    }
//    public class FSOMUStoODProc
//    {
//        public string POR_Id { get; set; }
//        public string CPMName { get; set; }
//        public string CPMSignum { get; set; }
//        public string CPMFullName { get; set; }
//        public string ProjectName { get; set; }
//        public string ProjectDefinition { get; set; }
//        public string SuperiorWBS { get; set; }
//        public string WBSName { get; set; }
//        public string ShipToParty { get; set; }
//        public string Element { get; set; }
//        public string FSOActivity { get; set; }
//        public decimal PlannedCosts { get; set; }
//        public string Site { get; set; }
//        public string WBS { get; set; }
//        public string SO { get; set; }
//        public string Network { get; set; }
//        public string Subcontractor { get; set; }
//    }
//    public class FSOPORtoODServiceData
//    {
//        public Context context { get; set; }
//        public FSOMUStoODProc POR { get; set; }
//        public EpplusService service { get; set; }
//    }
//    public class POItemStoredProcClass
//    {
//        public int No { get; set; }
//        public string Cat { get; set; }
//        public string Code { get; set; }
//        public string Plant { get; set; }
//        public decimal NetQty { get; set; }
//        public string B1 { get; set; }
//        public string B2 { get; set; }
//        public string ItemCat { get; set; }
//        public string PRtype { get; set; }
//        public string B3 { get; set; }
//        public string B4 { get; set; }
//        public string POrg { get; set; }
//        public string B6 { get; set; }
//        public string B7 { get; set; }
//        public string GLacc { get; set; }
//        public decimal Price { get; set; }
//        public string Curr { get; set; }
//        public string PRUnit { get; set; }
//        public string B8 { get; set; }
//        public string Vendor { get; set; }
//        public string B9 { get; set; }
//        public string B10 { get; set; }
//        public string B11 { get; set; }
//        public string B12 { get; set; }
//        public string B13 { get; set; }
//        public string B14 { get; set; }
//        public string B15 { get; set; }
//        public string B16 { get; set; }
//        public DateTime Plandate { get; set; }
//        public string Description { get; set; }

//    }
//}
