//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using TaskManager.Service;
//using DbModels.DataContext;
//using System.IO;
//using DbModels.DomainModels.ShClone;
//using DbModels.DomainModels.Base;
//using OfficeOpenXml;
//using CommonFunctions.StaticHelper;
//using CommonFunctions.Extentions;
//using System.Collections;
//using System.Drawing;
//using DbModels.Models;
//namespace TaskManager.Handlers.TaskHandlers.Models.PO
//{
//    public class PORtoODHandler : ATaskHandler
//    {
//        public PORtoODServiceData data { get; set; }
//        public PORtoODHandler(TaskParameters taskParameters)
//            : base(taskParameters)
//        {
//            data = new PORtoODServiceData
//            {
//                //Контекст БД
//                context = taskParameters.Context
//            };
//        }
//        public override bool Handle()
//        {
//            List<PORtoODProc> PORList = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromContext<PORtoODProc>(TaskParameters.Context, "SendPORtoOD", null);
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
//                    TaskParameters.TaskLogger.LogError(string.Format("Ошибка при формировании запроса ПО для обьекта ПОР {0}:{1}", data.POR.POR_Id, ex.Message));
//                }

//            }
//            if (ResultList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(ResultList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество отправленных в ОД ПОРов - {0}", ResultList.Count));
//            return true;
//        }
//        /// <summary>
//        /// Обработка конкретного ПОРа
//        /// </summary>
//        /// <param name="POR"></param>
//        /// <param name="ResultList"></param>
//        public virtual void ProcessPOR(PORtoODProc POR, ref List<PORSentToOD> ResultList)
//        {
//            TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем ПОР - {0}", POR.POR_Id));
//            ShPOR shPOR = SHPORService.GetPOR(POR.POR_Id, TaskParameters.Context);
//            if (POR == null)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("ПОР {0} не найден!", POR.POR_Id));
//                return;
//            }
//            //Получаем информацию о подрядчике из базы Максима
//            Dictionary<string, object> SParams = new Dictionary<string, object>();
//            SParams.Add("@Vendor", shPOR.Subcontractor);
//            data.VendorData = CommonFunctions.StaticHelper.StaticHelpers.GetStoredProcDataFromServer<OHDBVendorDataProc>("ERUMOMW0009_OHDB_GetVendorData", SParams).FirstOrDefault();
//            if (data.VendorData == null)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("Не удалось получить информацию о подрядчике {0} из базы Максима", shPOR.Subcontractor));
//                return;
//            }
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
//                TaskParameters.TaskLogger.LogInfo(string.Format("Сохраняем"));
//                string fileName = string.Format("POR-{0}.xlsx", data.POR.POR_Id);
//                if (!Save(fileName))
//                {
//                    return;
//                }
                
//                //Добавляем пор к итоговому списку
//                ResultList.Add(new PORSentToOD { POR = POR.POR_Id, DateSent = DateTime.Now, PathToFile = SaveToSandBox(fileName) });
//                TaskParameters.TaskLogger.LogInfo(string.Format("ПОР обработан и добавлен в итогововый набор"));
//            }
//        }

//        #region Вспомогательные методы
//        public bool ReplaceData()
//        {
//            #region Заменяем поля в файле
//            Dictionary<string, string> dict = new Dictionary<string, string>();
//            //Данные из ПОРа
//            foreach (var item in StaticHelpers.GetProperties(data.POR))
//            {
//                dict.Add(item.Name, item.GetValueExt(data.POR).ToString());
//            }
//            //Данные о подрядчике из базы Макса
//            foreach (var item in StaticHelpers.GetProperties(data.VendorData))
//            {
//                dict.Add(item.Name, item.GetValueExt(data.VendorData).ToString());
//            }

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
//            //List<EpplusService.DrawParam> DrawParams = new List<EpplusService.DrawParam>();
//            //int i = 1;
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

//            //    tableRow.Vendor = data.VendorData.VendorNumber.ToString();
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
//            //    new EpplusService.InsertTableParams() { PrintHeaders = false });
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
    
//        public class PORtoODProc
//        {
//            public string POR_Id { get; set; }
//            public string CPMFullName { get; set; }
//            public DateTime VPODate { get; set; }
//            public string Site { get; set; }
//            public decimal PlannedCosts { get; set; }
//            public string Subcontractor { get; set; }
//            public string Network { get; set; }
//            public string WBS { get; set; }
//            public string AddressRus { get; set; }
//            public DateTime WorkStartDate { get; set; }
//            public DateTime WorkEndDate { get; set; }
//        }
//        public class PORtoODServiceData
//        {
//            public Context context { get; set; }
//            public PORtoODProc POR { get; set; }
//            public EpplusService service { get; set; }
//            public OHDBVendorDataProc VendorData { get; set; }
//        }
//        /// <summary>
//        /// Процедура GetVendorData на 400м сервере
//        /// </summary>
//        public class OHDBVendorDataProc
//        {
//            public long VendorNumber { get; set; }
//            public string VendorAddress { get; set; }
//            public string VendorNameRus { get; set; }
//            public string VendorContractNo { get; set; }
//        }

//    }
//}
