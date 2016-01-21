//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using TaskManager.Service;
//using OfficeOpenXml;
//using TaskManager.TaskParamModels;
//using DbModels.DataContext;
//using DbModels.DomainModels.ShClone;
//using CommonFunctions.StaticHelper;
//using CommonFunctions.Extentions;
//using TaskManager.Handlers.TaskHandlers.Models.POR;
//using System.Collections;
//namespace TaskManager.Handlers.TaskHandlers.Models.PrintOut
//{
//    public abstract class APrintOutHandler : ATaskHandler
//    {
//        //Сервисный класс, который легче сделать полем класса хендлера
//        //Чтобы не пихать каждый раз методам различные аргументы
//        protected PrintOutServiceData data { get; set; }
//        public APrintOutHandler(TaskParameters taskParameters)
//            : base(taskParameters)
//        {
//            data = new PrintOutServiceData
//            {
//                //Контекст БД
//                context = taskParameters.Context,
//                //Создаем список экселек для заполнения ошибок по разным регионам
//                RegionWorkbooks = new List<RegionWorkbook>(),
//                NowDate = DateTime.Now,
//                SPName = "PrintOutWorkOrder"


//            };
//        }
//        public const string WorkOrderSheet = "Work Order";
//        public const string SoWSheet = "SoW";
//        public const string TempSoWSheet = "Temp SoW";
//        public const string GantChartSheet = "Gant Chart";
//        public const string SHSupport = "SHSupport";

//        public override bool Handle()
//        {
//            #region Выполняем хранимку и обрабатываем каждый заказ
//            //Путь к шаблону
//            FileInfo template = new FileInfo(TaskParameters.DbTask.TemplatePath);
//            List<PrintOutProc> list = data.context.Database.SqlQuery<PrintOutProc>(data.SPName).ToList();
//            TaskParameters.TaskLogger.LogInfo(string.Format("Выполнили хранимку {0}, количество обьектов для обработки - {1}", data.SPName, list.Count));
//            //Начинаем обрабатывать конкретный WO
//            foreach (var wo in list)
//            {
//                using (data.service = new EpplusService(template))
//                {
//                    TaskParameters.TaskLogger.LogInfo(string.Format("Обрабатываем заказ - {0}", wo.WO));
//                    try
//                    {
//                        //Пытаемся получить объекты, необходымые для работы
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Пытаемся получить объекты, необходымые для работы"));
//                        if (!Initialize(wo.WO))
//                        {
//                            continue;
//                        }
//                        //Проверяем на всевозможные ошибки
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Проверяем на всевозможные ошибкиы"));
//                        if (!CheckErrors())
//                        {
//                            continue;
//                        }
//                        //Заполняем список работ
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Заполняем скоп работ"));
//                        CreateSoW();
//                        //Заполняем таймплан
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Создаем график Гантта"));
//                        CreateGantChart();
//                        //Удаляем Финансового директора, если это необходимо
//                        DeleteFinDir();
//                        //Заменяем #param# данными для конкретного заказа
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Меняем текст под конкретный заказ"));
//                        ReplaceData();
//                        //Ровняем
//                        //Autofit();
//                        //Сохраням
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Сохраняем"));
//                        Save();
//                        TaskParameters.TaskLogger.LogInfo(string.Format("Заказ обработан"));
//                        //Если дошли до сюда - все хорошо
//                    }
//                    catch (Exception ex)
//                    {
//                        LogError(SHSupport, ex.Message);
//                        continue;
//                    }
//                }
//            }
//            //Загружаем файлы в СХ, если это необходимо.
//            ImportFiles(list);
//            #endregion
//            //Сохраняем ошибки(если они есть) по папкам для отправления в регионы
//            SaveRegionErrors();
//            return true;
//        }
//        #region Вспомогательные методы(будут меняться для разных типов заказов)
//        public virtual void ImportFiles(List<PrintOutProc> WOList)
//        {
//            if (WOList.Count > 0)
//            {
//                var PrintOutCompleted = WOList.Select(l => new { Object = l.WO, Date = DateTime.Now }).ToList();
//                TaskParameters.ImportHandlerParams = new ImportHandlerParams();
//                //Загружаем заказы
//                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(PrintOutCompleted) });
//                TaskParameters.TaskLogger.LogInfo(string.Format("Количество обработанных заказов - {0}", WOList.Count));
//            }
//        }
//        public virtual bool SaveRegionErrors()
//        {
//            //Сохраняем ошибки(если они есть) по папкам для отправления в регионы
//            foreach (var Rbook in data.RegionWorkbooks)
//            {
//                //Путь к папке конкретного региона для отправки писем
//                string LogFilePath = Path.Combine(TaskParameters.DbTask.EmailSendFolder, Rbook.Region);
//                string LogFileName = string.Format("Errors({0}).xlsx", DateTime.Now.ToString("yyMMddHHmmff"));
//                Rbook.Service.CreateFolderAndSaveBook(LogFilePath, LogFileName);
//            }
//            return true;
//        }
//        /// <summary>
//        /// Получаем обьекты для дальнейшей работы
//        /// Эта процедура должна идти в самом начале обработки конкретного заказа
//        /// </summary>
//        /// <param name="wo"></param>
//        /// <returns></returns>
//        public virtual bool Initialize(string wo)
//        {
//            #region и проверяем возможные ошибки
//            ExcelWorkbook book = data.service.app.Workbook;
//            data.shtWo = data.service.GetSheet(WorkOrderSheet);
//            data.shtSoW = data.service.GetSheet(SoWSheet);
//            data.shtGant = data.service.GetSheet(GantChartSheet);
//            data.shWo = SHWOService.GetWO(wo, data.context);
//            if (data.shWo == null)
//            {
//                LogError(SHSupport, string.Format("Заказ {0} не найден в !", wo));
//                return false;
//            }
//            data.shElement = SHWOService.GetWoElement(data.shWo, data.context);
//            if (data.shElement == null)
//            {
//                LogError(SHSupport, string.Format("Элемент для сайта {0} не найден", data.shSite.Site));
//                return false;
//            }
//            data.shSite = SHWOService.GetWoSite(data.shWo, data.context);
//            if (data.shSite == null)
//            {
//                LogError(SHSupport, string.Format("Cайт для заказа {0} не найден(или элемент, или вбс+_+)", wo));
//                return false;
//            }
//            data.shNode = SHWOService.GetSiteNode(data.shSite, data.context);
//            if (data.shNode == null)
//            {
//                LogError(data.shElement.MacroRegion, string.Format(string.Format("Нода для сайта {0} не найдена", data.shSite.Site)));
//                return false;
//            }
//            return true;
//            #endregion
//        }
//        public virtual void CopyHeadersFromMainSheet()
//        {
//            data.shtGant.CopyHeaders(data.shtWo);
//            data.shtSoW.CopyHeaders(data.shtWo);
//        }
//        /// <summary>
//        /// Заполнение графика Гантта
//        /// </summary>
//        public virtual void CreateGantChart()
//        {
//            // Заполнение данных для план-графика
//            var timePlan = SHWOService.GetTimePlan(data.shWo, data.context);
//            //ячейка с началом работ
//            ExcelNamedRange GantStart = data.service.app.Workbook.Names["GantStart"];
//            //ячейка с окончанием работ
//            ExcelNamedRange GantEnd = data.service.app.Workbook.Names["GantEnd"];
//            //Строчка с образцом позиции графика. Ее мы будем копировать столько раз, сколько у нас позиций в графике
//            ExcelNamedRange RowStart = data.service.app.Workbook.Names["GantRowStart"];
//            //строчка выполнение работ(там указывается начало и окончание работ)
//            ExcelNamedRange Scope = data.service.app.Workbook.Names["GantScope"];
//            if (GantStart == null || GantEnd == null || RowStart == null || Scope == null)
//            {
//                throw new NullReferenceException("Кто-то сломал шаблон. Проверьте именованные диапазоны GantStart, GantEnd, RowStart, Scope на листе с план графиком");
//            }
//            data.shtGant.Cells[GantStart.Address].Value = timePlan.StartDate;
//            data.shtGant.Cells[GantEnd.Address].Value = timePlan.EndDate;
//            //Заполняем строчку с выполнением работ
//            data.shtGant.Cells[Scope.Address].Value = timePlan.StartDate;
//            data.shtGant.Cells[Scope.Address].Offset(0, 1).Value = timePlan.EndDate;
//            //общая продолжительность
//            data.shtGant.Cells[Scope.Address].Offset(0, 2).Value = timePlan.Rows.Sum(s => s.Diruation);
//            data.shtGant.InsertRow(RowStart.Start.Row + 1, timePlan.Rows.Count, RowStart.Start.Row);

//            int curRow = RowStart.Start.Row + 1;
//            foreach (var row in timePlan.Rows)
//            {
//                RowStart.Copy(data.shtGant.Cells[curRow, 1]);
//                data.shtGant.Cells[curRow, 1].Value = row.Position;
//                data.shtGant.Cells[curRow, 2].Value = row.Code;
//                data.shtGant.Cells[curRow, 3].Value = row.StartDate;
//                data.shtGant.Cells[curRow, 4].Value = row.EndDate;
//                data.shtGant.Cells[curRow, 5].Value = row.Diruation;
//                curRow++;

//            }
//            data.shtGant.DeleteRow(RowStart.Start.Row, 1);
//        }

//        /// <summary>
//        /// Заполнение таблицы со списком позиций
//        /// </summary>
//        public virtual void CreateSoW()
//        {
//            #region Формирование списка работ, вставка в шаблон
//            //Получаем список работ с помощью GetSOW List
//            List<SoWGroup> SoWGroupList = SHWOService.GetSoWGroupList(data.shWo, data.context);
//            ExcelNamedRange RowName = data.service.GetRange("SowName");
//            ExcelNamedRange RowTable = data.service.GetRange("SowTable");
//            ExcelNamedRange RowTotal = data.service.GetRange("SowTotal");
//            ExcelNamedRange RangeStart = data.service.GetRange("SowStart");
//            int CurrentRow = RangeStart.Start.Row + 1;
//            int CurrentColumn = RangeStart.Start.Column;
//            //Вставляем число строк, равное количеству всех позиций + 2 строчки на каждую группу
//            data.shtSoW.InsertRow(CurrentRow, SoWGroupList.Sum(s => s.SoWList.Count + 2));
//            foreach (var group in SoWGroupList)
//            {
//                //Копируем строчку с заголовком группы
//                RowName.Copy(data.shtSoW.Cells[CurrentRow, CurrentColumn]);
//                //Номер позиции в группе 
//                //Пример: 1.1
//                data.shtSoW.Cells[CurrentRow, CurrentColumn].Value = group.Position;
//                //Название группы
//                //Приме: ПИР
//                data.shtSoW.Cells[CurrentRow, CurrentColumn + 1].Value = group.Name;
//                CurrentRow++;
//                foreach (var item in group.SoWList)
//                {
//                    //Копируем строчку с позицией
//                    RowTable.Copy(data.shtSoW.Cells[CurrentRow, CurrentColumn]);
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn].Value = item.Position;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 1].Value = item.Code;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 2].Value = item.Description;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 3].Value = item.Quantity;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 4].Value = item.Unit;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 5].Value = item.Price;
//                    data.shtSoW.Cells[CurrentRow, CurrentColumn + 6].Value = item.TotalPrice;
//                    CurrentRow++;

//                }
//                //Копируем строчку с заключением по группе
//                RowTotal.Copy(data.shtSoW.Cells[CurrentRow, CurrentColumn]);
//                //Вставка соответсвующего текста
//                //Пример: Всего по разделу ПИР
//                data.shtSoW.Cells[CurrentRow, CurrentColumn + 1].Value = group.Total;
//                //Сумма всей подгруппы
//                data.shtSoW.Cells[CurrentRow, CurrentColumn + 6].Value = group.TotalPrice;

//                CurrentRow++;
//            }
//            //Проставляем полную сумму по заказу
//            var TotalWoPrice = data.shtSoW.Cells.FirstOrDefault(s => s.Text == "#TotalWOPrice#");
//            if (TotalWoPrice != null)
//            {
//                TotalWoPrice.Value = Math.Round(SoWGroupList.Sum(s => s.TotalPrice), 2);
//            }
//            //Удаляем образцовую строчку
//            data.shtSoW.DeleteRow(RowName.Start.Row, 3);
//            #endregion
//        }
//        /// <summary>
//        /// Заполняем текст данными конкретнго заказа
//        /// Используется в конце
//        /// </summary>
//        /// <param name="data"></param>
//        public virtual void ReplaceData()
//        {
//            //Копируем колонитулы с листа с текстом заказа на другие листы
//            CopyHeadersFromMainSheet();
//            #region Получение необходимых данных, заполнение листа

//            //Получаем необходимую для заказа информацию
//            Dictionary<string, string> dict = new Dictionary<string, string>();
//            var values = data.context.Regions.Where(r => r.NameRus == data.shElement.RegionRus).Select(r => new PrintOutDataWorkOrder
//            {
//                RbsType = data.shNode.RbsType,
//                Site = data.shSite.Site,
//                AddressEng = data.shSite.AddressEng,
//                AddressRus = data.shSite.AddressRus,
//                BranchNameEng = r.BranchNameEng,
//                BranchNameRus = r.BranchNameRus,
//                FinDirAttorney = r.FinDirAttorney,
//                FinDirAttorneyDate = r.FinDirAttorneyDate,
//                FinDirEng = r.FinDirEng,
//                FinDirRus = r.FinDirRus,
//                FinDirRusRP = r.FinDirRusRP,
//                TechDirAttorney = r.TechDirAttorney,
//                TechDirAttorneyDate = r.TechDirAttorneyDate,
//                TechDirEng = r.TechDirEng,
//                TechDirEngPost = r.TechDirEngPost,
//                TechDirRus = r.TechDirRus,
//                TechDirRusPost = r.TechDirRusPost,
//                TechDirRusPostPR = r.TechDirRusPostPR,
//                TechDirRusRP = r.TechDirRusRP,
//                BeforeTechDirAttorneyRus = r.BeforeTechDirAttorneyRus,
//                AfterTechDirAttorneyRus = r.AfterTechDirAttorneyRus,
//                BeforeTechDirAttorneyEng = r.BeforeTechDirAttorneyEng,
//                AfterTechDirAttorneyEng = r.AfterTechDirAttorneyEng,
//                WO = data.shWo.WO,
//                ParentWo = data.shWo.ParentWO
//            }).FirstOrDefault();
//            values.Vat = SHWOService.GetWOTotalPriceVat(data.shWo, data.context);
//            values.VatWo = SHWOService.GetWOTotalPriceWithVat(data.shWo, data.context);
//            values.WoDate = data.shWo.WoDate.Value.ToShortDateString();
//            foreach (var item in StaticHelpers.GetProperties(values))
//            {
//                dict.Add(item.Name, item.GetValueExt(values).ToString());
//            }
//            data.service.ReplaceDataInBook(dict, true);

//            #endregion
//        }
//        /// <summary>
//        /// При необходимости удаляем фин директора
//        /// Ищем в табличке Regions нужный регион и смотрим, заполнено ли у него поле FinDir
//        /// Если нет, удаляем ненужные строчки из текста заказа
//        /// </summary>
//        public virtual void DeleteFinDir()
//        {
//            var region = data.context.Regions.Where(r => r.NameRus == data.shElement.RegionRus).FirstOrDefault();
//            if (region == null)
//            {
//                string err = string.Format("Регион {0} не найден в таблице Regions", data.shElement.RegionRus);
//                LogError(SHSupport, err);
//                throw new NullReferenceException(err);
//            }
//            if (string.IsNullOrEmpty(region.FinDirRus))
//            {
//                var FinCellsList = data.service.app.Workbook.FindCellsinEntireBook("Fin");
//                foreach (var row in FinCellsList)
//                {
//                    row.Worksheet.Row(row.Start.Row).Hidden = true;
//                }
//            }
//        }
//        public virtual void DeleteImPlanText()
//        {
//            var FinCellsList = data.shtWo.FindCells("Exclude" + data.shWo.ExcludeImPlan.Value.ToString().ToUpper());
//            foreach (var row in FinCellsList)
//            {
//                row.Worksheet.Row(row.Start.Row).Hidden = true;
//            }
//        }
//        /// <summary>
//        /// Авто высота строк
//        /// Пока не работает
//        /// </summary>
//        public virtual void Autofit()
//        {
//            data.shtWo.AutoFitRows();
//            data.shtSoW.AutoFitRows();
//            data.shtGant.AutoFitRows();
//        }
//        /// <summary>
//        /// Сохранение файла конкретного заказа
//        /// </summary>
//        public virtual void Save()
//        {
//            string ExcelFilePath = Path.Combine(TaskParameters.DbTask.ArchiveFolder, data.NowDate.Year.ToString(), data.NowDate.Month.ToString(), data.NowDate.Day.ToString(), data.NowDate.ToString("HHmmss"));
//            string EmailFilePath = Path.Combine(TaskParameters.DbTask.EmailSendFolder, data.shElement.MacroRegion);
//            string FileName = data.shWo.WO + ".xlsm";
//            string PDFFileName = data.shWo.WO + ".pdf";
//            //Сохраняем файл
//            TaskParameters.TaskLogger.LogInfo(string.Format("Сохраняем в Excel"));
//            data.service.CreateFolderAndSaveBook(ExcelFilePath, FileName);
//            TaskParameters.TaskLogger.LogInfo(string.Format("Сохраняем в ПДФ"));
//            Excel2PDF.XLSConvertToPDF(Path.Combine(ExcelFilePath, FileName), Path.Combine(EmailFilePath, PDFFileName));
//        }
//        /// <summary>
//        /// Проверка ошибок конкретного заказа
//        /// </summary>
//        /// <returns></returns>
//        public virtual bool CheckErrors()
//        {
//            if (!SHWOService.CheckEcrAddPositions(data.shWo, data.context))
//            {
//                LogError(data.shElement.MacroRegion, string.Format("Данные для позиций ECR Add не заполнены", data.shWo.WO));
//                return false;
//            }
//            if (data.shWo.GetWoItems(data.context).Count() == 0)
//            {
//                LogError(data.shElement.MacroRegion, string.Format("У заказа {0} нет позиций", data.shWo.WO));
//                return false;

//            }
//            if (!data.shWo.WoDate.HasValue)
//            {
//                LogError(data.shElement.MacroRegion, string.Format("У заказа {0} отсутствует WO Date", data.shWo.WO));
//                return false;
//            }
//            return true;
//        }
//        #endregion
//        public virtual void LogError(string Region, string Error)
//        {
//            ExcelWorkbook book;
//            ExcelWorksheet sht;
//            //Пишем ошибку в обычный лог
//            TaskParameters.TaskLogger.LogError(Error);
//            //Пытаемся найти уже открытую эксельку с текущим регионом
//            RegionWorkbook service = data.RegionWorkbooks.FirstOrDefault(r => r.Region == Region);
//            //Если найти не удается, создаем и добавляем в коллекцию
//            if (service == null)
//            {
//                service = new RegionWorkbook
//                {
//                    Region = Region,
//                    Service = new EpplusService()
//                };
//                data.RegionWorkbooks.Add(service);
//                book = service.Service.app.Workbook;
//                sht = book.Worksheets.Add("Errors");
//            }
//            else
//            {
//                book = service.Service.app.Workbook;
//                sht = service.Service.GetSheet("Errors");
//            }
//            int currentRow;
//            //Если это новая книга, то свойство листа Dimension = null
//            if (sht.Dimension == null)
//            {
//                currentRow = 1;
//            }
//            else
//            {
//                currentRow = sht.Dimension.End.Row + 1;
//            }
//            //Заносим ошибку
//            sht.Cells[currentRow, 1].Value = Error;

//        }
//    }
//    public class PrintOutServiceData
//    {
//        public ShWO shWo { get; set; }
//        public ShSite shSite { get; set; }
//        public ShNode shNode { get; set; }
//        public ShElement shElement { get; set; }
//        public ExcelWorksheet shtSoW { get; set; }
//        public ExcelWorksheet shtGant { get; set; }
//        public ExcelWorksheet shtWo { get; set; }
//        public EpplusService service { get; set; }
//        public Context context { get; set; }
//        /// <summary>
//        /// Название Хранимки для выборки заказов определнного типа
//        /// </summary>
//        public string SPName { get; set; }
//        /// <summary>
//        /// Коллекция ошибок для разных регионов
//        /// </summary>
//        public List<RegionWorkbook> RegionWorkbooks { get; set; }
//        public DateTime NowDate { get; set; }

//    }
//    /// <summary>
//    /// Класс представляет собой объект ошибки для конкретного региона
//    /// </summary>
//    public class RegionWorkbook
//    {
//        public EpplusService Service { get; set; }
//        public string Region { get; set; }
//    }

//}
