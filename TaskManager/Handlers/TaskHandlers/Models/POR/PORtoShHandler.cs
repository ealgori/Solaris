//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using DbModels.DomainModels.ShClone;
//using CommonFunctions.StaticHelper;
//using System.Collections;
//using TaskManager.Service;
//using DbModels.DomainModels.Base;

//namespace TaskManager.Handlers.TaskHandlers.Models.POR
//{
//    public class PORtoShHandler : ATaskHandler
//    {
//        public PORtoShHandler(TaskParameters taskParameters)
//            : base(taskParameters) { }
//        public override bool Handle()
//        {
//            //Список SOW, по которым надо сформировать ПОР
//            //Получаем из хранимки
//            List<PORtoSHProc> SoWList = TaskParameters.Context.Database.SqlQuery<PORtoSHProc>("PORtoSH").ToList();
//            //Итоговый список обьектов POR
//            List<PORtoSH> shporlist = new List<PORtoSH>();
//            //Итоговый список обьектов POR Item
//            List<PORtoSHItem> shporitemlist = new List<PORtoSHItem>();
//            //Список обьектов SOW Item, для которых надо удалить подрядчика
//            List<ObjectToSH> deleteSubcsList = new List<ObjectToSH>();
//            //По каждому SOW пытаемся что-нибудь сформировать
//            foreach (var item in SoWList)
//            {
//                ProcessSOW(item, ref shporlist, ref shporitemlist, ref deleteSubcsList);
//            }
//            if (shporlist.Count > 0 && shporitemlist.Count > 0 && deleteSubcsList.Count > 0)
//            {
//                TaskParameters.ImportHandlerParams = new ImportHandlerParams();
//                //Загружаем ПОРы
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(shporlist) });
//                //Загружаем позиции в ПОРы
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(shporitemlist) });
//                //Удаляем подрядчика
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName4, Objects = new ArrayList(deleteSubcsList) });

//            }
//            if (SoWList.Count > 0)
//            {
//                //Убираем галочку для формирования ПОРа
//                List<ObjectToSH> RemovePORSendList = SoWList.Select(s => new ObjectToSH { Object = s.SoW }).ToList();
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(RemovePORSendList) });
//            }
//            TaskParameters.TaskLogger.LogInfo(string.Format("Количество сгенерированных ПОРов в СХ - {0}", shporlist.Count));
//            return true;

//        }
//        public void ProcessSOW(PORtoSHProc item, ref List<PORtoSH> shporlist, ref  List<PORtoSHItem> shporitemlist, ref List<ObjectToSH> deleteSubcsList)
//        {
//            //#region Получение обьектов СХ
//            //TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем SOW - {0}", item.SoW));
//            //ShSOW sow = SHPORService.GetSOW(item.SoW, TaskParameters.Context);
//            //if (sow == null)
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("Обьект SOW {0} не найден.", item.SoW));
//            //    return;
//            //}
//            //ShWBS shWBS = SHPORService.GetSOWWBS(sow, TaskParameters.Context);
//            //if (shWBS == null)
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("Обьект WBS для SOW {0} не найден.", item.SoW));
//            //    return;
//            //}
//            //ShElement shElement = SHPORService.GetSOWElement(sow, TaskParameters.Context);
//            //if (shElement == null)
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("Обьект Element для SOW {0} не найден.", item.SoW));
//            //    return;
//            //}
//            //ShSite shSite = SHPORService.GetSOWSite(sow, TaskParameters.Context);
//            //if (shSite == null)
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("Обьект Site для SOW {0} не найден.", item.SoW));
//            //    return;
//            //}

//            ////Получение списка позиций конкретного SOW
//            //List<ShSOWItem> SowItemList = SHPORService.GetSOWItemsWithSubc(sow, TaskParameters.Context);
//            ////Если у SOW нет позиций - ошибка в СХ!
//            //if (SowItemList.Count == 0)
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("SOW {0} не сформирован. Нет позиций SOW Item c заполненныйм подрядчиком", item.SoW));
//            //    return;
//            //}
//            //#endregion
//            //#region Проверка ошибок(бизнес логика)

//            ////Проверка позиций SOW на различные ошибки
//            ////Делаем это заранее(в цикле по ПОРам), чтобы можно было записать ошибку в лог и продолжить формирование ПОРов
//            //string CheckSOWItems = SHPORService.CheckSOWItems(sow, TaskParameters.Context);
//            //if (!string.IsNullOrEmpty(CheckSOWItems))
//            //{
//            //    TaskParameters.TaskLogger.LogError(string.Format("SOW {0} не сформирован. {1}", item.SoW, CheckSOWItems));
//            //    return;

//            //}
//            //#endregion
//            //#region Для каждого SOW создаем ПОР по каждому подрядчику

//            ////Промежуточные списки созданы для того, чтобы сначала заполнить их
//            ////Если случится какая-то ошибка, обьекты из этих списков не попадут в итоговые наборы
//            ////Так кае если в одном из поров по SOW ошибка - SOW не формируется
//            //List<PORtoSH> SubPORList = new List<PORtoSH>();
//            //List<PORtoSHItem> SubPORItemList = new List<PORtoSHItem>();
//            //List<ObjectToSH> SubDeleteSubcList = new List<ObjectToSH>();
//            //foreach (var subc in SowItemList.Where(s => s.Subcontractor.Length > 2).Select(s => s.Subcontractor).Distinct().ToList())
//            //{
//            //    //Выбираем список позиций, относящихся к одному подрядчику
//            //    var subcSowItemList = SowItemList.Where(i => i.Subcontractor == subc).ToList();
//            //    #region Начальное создание POR

//            //    //Макрорегион
//            //    MacroRegion macroRegion = TaskParameters.Context.MacroRegions.FirstOrDefault(m => m.Name == shElement.MacroRegion);
//            //    if (macroRegion == null)
//            //    {
//            //        TaskParameters.TaskLogger.LogError(string.Format("Адская лажа, регион {0} не найден в бд.", shElement.MacroRegion));
//            //        return;
//            //    }
//            //    //Айди подрядчика в базе Максима
//            //    //Используетя в маске ПОРа и как параметр хранимки для получения цены позиции
//            //    int subcId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetSubcId", "@Subc", subc);
//            //    if (subcId == 0)
//            //    {
//            //        TaskParameters.TaskLogger.LogError(string.Format("Подрядчик {0} не найден в базе Максима", subc));
//            //        return;
//            //    }
//            //    //Временная маска
//            //    string date = SHPORService.GetDateMask();
//            //    string site = shSite.Site.Replace("-", "").Replace("~", "");
//            //    PORtoSH por = new PORtoSH
//            //    {
//            //        Name = string.Format("PO-{0}-{1}-{2}-{3}", macroRegion.ShortName, subcId.ToString("D3"), date, site),
//            //        CreationDate = DateTime.Today,
//            //        WBS_Id = shWBS.WBS,
//            //        SOW_Id = sow.SOW,
//            //        Site_Id = shSite.Site,
//            //        Subcontractor = subc,
//            //        WorkStart = (DateTime)subcSowItemList.Min(s => s.WorkStartDate),
//            //        WorkEnd = (DateTime)subcSowItemList.Min(s => s.WorkEndDate),
//            //        MacroRegion = shElement.MacroRegion
//            //    };
//            //    por.VPODate = SHPORService.GetPriceDate(por.WorkStart, TaskParameters.Context);
//            //    #endregion
//            //    #region Формируем список позиций для ПОРа по одному подрядчику
//            //    //Список для расчета данных по конкретному пору
//            //    List<PORtoSHItem> CurrentPORItemList = new List<PORtoSHItem>();
//            //    foreach (var sowItem in subcSowItemList)
//            //    {
//            //        //Разная логика для обычных позиций и ECR_Add
//            //        if (sowItem.IsEcrAddS())
//            //        {
//            //            #region Логика для ECR Add

//            //            PORtoSHItem porToSHItem = new PORtoSHItem
//            //            {
//            //                SOWCode = sowItem.Code,
//            //                SAPCode = sowItem.Code,
//            //                Price = sowItem.Price.Value,
//            //                Description = sowItem.Description,
//            //                Unit = sowItem.Unit,
//            //                Quantity = sowItem.Quantity.Value,
//            //                Subcontractor = subc,
//            //                WorkStart = sowItem.WorkStartDate.Value,
//            //                WorkEnd = sowItem.WorkEndDate.Value,
//            //                POR = por.Name,
//            //                Dismounting = sowItem.Dismounting.Value,
//            //                Currency = sowItem.Currency
//            //            };
//            //            //Добавляем позицию в промежуточный список
//            //            CurrentPORItemList.Add(porToSHItem);
//            //            #endregion
//            //        }
//            //        else
//            //        {
//            //            #region Логика для обычных позиций

//            //            //Получаем обьект SOWCode на основе данных из сх
//            //            SOWCode sowCode = sowItem.GetSOWCode(TaskParameters.Context);
//            //            if (sowCode == null)
//            //            {
//            //                //Если мы сюда пришли, значит что-то не так.
//            //                //Проверка на Null осуществляется ранее в методе CheckSOWItems
//            //                TaskParameters.TaskLogger.LogError(string.Format("Код {0} не найден в таблице SOWCodes", sowItem.Code));
//            //                return;
//            //            }
//            //            //SOW код дает нам набор САП Кодов
//            //            List<SOWMappingRow> SAPCodeList = sowCode.GetSAPCodes(TaskParameters.Context);
//            //            if (SAPCodeList.Count == 0)
//            //            {
//            //                TaskParameters.TaskLogger.LogError(string.Format("SOW код {0} отсутствует в таблице SOWMapping ", sowItem.Code));
//            //                return;
//            //            }
//            //            foreach (var sowMappingRow in SAPCodeList)
//            //            {
//            //                PORtoSHItem porToSHItem = new PORtoSHItem
//            //                {
//            //                    SOWCode = sowCode.Code,
//            //                    SAPCode = sowMappingRow.SAPCode.Code,
//            //                    Description = !sowItem.Dismounting.Value ? sowMappingRow.SAPCode.Description : "Dismounting - " + sowMappingRow.SAPCode.Description,
//            //                    Unit = sowMappingRow.SAPCode.Unit,
//            //                    //Вот это очень большой вопрос, какое количество брать, если SOW код распадается на несколько SAP кодов
//            //                    Quantity = sowItem.Quantity.Value * sowMappingRow.Quantity,
//            //                    Subcontractor = subc,
//            //                    WorkStart = sowItem.WorkStartDate.Value,
//            //                    WorkEnd = sowItem.WorkEndDate.Value,
//            //                    POR = por.Name,
//            //                    Dismounting  = sowItem.Dismounting.Value,
//            //                    Currency = sowItem.Currency

//            //                };
//            //                #region Получаем цену позиции из хранимки
//            //                int macroregionId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetMacroRegionId", "@macroRegion", shElement.MacroRegion);
//            //                if (macroregionId == 0)
//            //                {
//            //                    TaskParameters.TaskLogger.LogError(string.Format("МакроРегион {0} не найден в базе Максима", shElement.MacroRegion));
//            //                    return;
//            //                }
//            //                int projectId = SHPORService.GetObjectIdByName("ERUMOMW0009_OHDB_GetProjectId", "@Project", shElement.ProjectDefinitionName);
//            //                if (projectId == 0)
//            //                {
//            //                    TaskParameters.TaskLogger.LogError(string.Format("Проект {0} не найден в базе Максима", shElement.ProjectDefinitionName));
//            //                    return;
//            //                }
//            //                //Получаем цену позиции из хранимки
//            //                decimal price = SHPORService.GetPrice(porToSHItem.SAPCode, shElement.RegionRus, macroregionId, por.VPODate, projectId, subcId);
//            //                if (price == 0)
//            //                {
//            //                    //В сорсинг туле нет цены. Сворачиваемся( Надо что-то придумать.
//            //                    //Так как обработка ПОРов не должна останавливаться
//            //                    TaskParameters.TaskLogger.LogError(string.Format("Прайс не найден!!!! Сап код - {0}, регион - {1}, макрорегион - {2}, дата позиции - {3}, проект - {4}, подрядчик - {5}", porToSHItem.SAPCode, shElement.RegionRus, shElement.MacroRegion, por.VPODate, shElement.ProjectDefinitionName, subc));
//            //                    return;
//            //                }
//            //                if (porToSHItem.Dismounting)
//            //                {
//            //                    price = Math.Round(price / 2);
//            //                }
//            //                //Умножаем цену на количество
//            //                porToSHItem.Price = Math.Round(price * porToSHItem.Quantity, 2);
//            //                #endregion
//            //                //Добавляем позицию в промежуточный список
//            //                TaskParameters.TaskLogger.LogInfo(string.Format("ПОР {0} готов к загрузке в СХ",por.Name));
//            //                CurrentPORItemList.Add(porToSHItem);
//            //            }

//            //            #endregion
//            //        }
//            //        //Добавляем позицию в промежуточный список позиций для удаления подрядчика
//            //        SubDeleteSubcList.Add(new ObjectToSH { Object = sowItem.SOWItemID });
//            //    }
//            //    //Рассчитываем полную стоимость пора
//            //    por.POTotal = Math.Round(CurrentPORItemList.Sum(s => s.Price), 2);
//            //    //Добавляем ПОР и позиции в промежуточные списки
//            //    SubPORList.Add(por);
//            //    SubPORItemList.AddRange(CurrentPORItemList);
//            //    #endregion
//            //}
//            ////Добавляем в итоговый список позиций позиции всех поров, сформированных для одного SOW
//            //shporitemlist.AddRange(SubPORItemList);
//            ////Добавляем в итоговый список поров сами поры, сформированные для одного SOW
//            //shporlist.AddRange(SubPORList);
//            ////Добавляем в итоговый список номера позиций SOW Item для удаления подрядчика
//            //deleteSubcsList.AddRange(SubDeleteSubcList);
//            //#endregion
//        }
//    }
//    //Загрузка POR
//    public class PORtoSH
//    {
//        public string Name { get; set; }
//        public DateTime CreationDate { get; set; }
//        public string Subcontractor { get; set; }
//        public decimal POTotal { get; set; }
//        public string SOW_Id { get; set; }
//        public string WBS_Id { get; set; }
//        public DateTime VPODate { get; set; }
//        public DateTime WorkStart { get; set; }
//        public DateTime WorkEnd { get; set; }
//        public string Site_Id { get; set; }
//        public string MacroRegion { get; set; }

//    }
//    //Загрузка POR Item
//    public class PORtoSHItem
//    {
//        public int PORItemID { get; set; }
//        public string POR { get; set; }
//        public string SOWCode { get; set; }
//        public string SAPCode { get; set; }
//        public decimal Quantity { get; set; }
//        public decimal Price { get; set; }
//        public DateTime WorkStart { get; set; }
//        public DateTime WorkEnd { get; set; }
//        public string Description { get; set; }
//        public string Unit { get; set; }
//        public string Subcontractor { get; set; }
//        public bool Dismounting { get; set; }
//        public string Currency { get; set; }

//    }
//    //Загрузки SOW Item subc delete(SEC) и SOW Send Clear(SEC)
//    public class ObjectToSH
//    {
//        public string Object { get; set; }
//    }

//    public class PORtoSHProc
//    {
//        public string SoW { get; set; }
//    }
//}
