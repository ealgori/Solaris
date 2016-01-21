//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using System.IO;
//using OfficeOpenXml;
//using TaskManager.Service;
//using CommonFunctions.StaticHelper;
//using System.Collections;
//using DbModels.DomainModels.ShClone;

//namespace TaskManager.Handlers.TaskHandlers.Models.PrintOut
//{
//    public class PrintOutHandlerSupplement : APrintOutHandler
//    {
//        //private PrintOutServiceData data { get; set; }
//        public PrintOutHandlerSupplement(TaskParameters taskParameters)
//            : base(taskParameters)
//        {
//            data = new PrintOutServiceData
//            {
//                //Контекст БД
//                context = taskParameters.Context,
//                //Создаем список экселек для заполнения ошибок по разным регионам
//                RegionWorkbooks = new List<RegionWorkbook>(),
//                NowDate = DateTime.Now,
//                SPName = "PrintOutSupplement"
//            };
//        }
//        public override bool CheckErrors()
//        {
//            if (base.CheckErrors())
//            {
//                if (data.shWo.GetFreezedParent(data.context) == null)
//                {
//                    LogError(data.shElement.RegionRus, string.Format("Основной заказ для заказа {0} не указан, либо не заморожен.", data.shWo.WO));
//                    return false;
//                }
//                return true;
//            }
//            return false;
//        }
//        public override void CreateGantChart()
//        {
//            base.CreateGantChart();
//            if (data.shWo.ExcludeImPlan.Value)
//            {
//                //Удаляем лист с графиком, чтобы он не распечатался
//                data.service.app.Workbook.Worksheets.Delete(data.shtGant);
//            }
//            //Удаляем одну из двух ненужных строчек
//            DeleteImPlanText();
//        }
//        public override void ReplaceData()
//        {
//            //Копируем колонитулы с листа с текстом заказа на другие листы
//            CopyHeadersFromMainSheet();
//            #region Получение необходимых данных, заполнение листа

//            //Получаем необходимую для заказа информацию
//            Dictionary<string, string> dict = new Dictionary<string, string>();
//            var values = data.context.Regions.Where(r => r.NameRus == data.shElement.RegionRus).Select(r => new PrintOutDataSupplement
//            {
//                RbsType = data.shNode.RbsType,
//                Site = data.shSite.Site,
//                AddressEng = data.shSite.AddressEng,
//                AddressRus = data.shSite.AddressEng,
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
//            ShWO Parent = SHWOService.GetParentWo(data.shWo, data.context);
//            if (Parent == null || !Parent.WoDate.HasValue)
//            {
//                TaskParameters.TaskLogger.LogError(string.Format("Основноый заказ не найден или дата WODate не проставлена; заказ - {0}", data.shWo.WO));
//            }
//            values.ParentWoDate = Parent.WoDate.Value;
//            values.Vat = SHWOService.GetWOTotalPriceVat(data.shWo, data.context);
//            values.VatWo = SHWOService.GetWOTotalPriceWithVat(data.shWo, data.context);
//            values.WoDate = data.shWo.WoDate.Value.ToShortDateString();

//            values.VatTotalPrice = SHWOService.GetAllWOTotalPriceWithVat(data.shWo, values.VatWo, data.context);
//            //Если график не печатается
//            if (data.shWo.ExcludeImPlan.Value)
//            {
//                var timePlan = SHWOService.GetTimePlan(data.shWo, data.context);
//                values.WorkStart = timePlan.StartDate;
//                values.WorkEnd = timePlan.EndDate;
//            }
//            foreach (var item in StaticHelpers.GetProperties(values))
//            {
//                dict.Add(item.Name, item.GetValueExt(values).ToString());
//            }
//            data.service.ReplaceDataInBook(dict, true);

//            #endregion
//        }

//        /// <summary>
//        /// Класс для замены текста в заказе
//        /// </summary>
//        public class PrintOutDataSupplement
//        {
//            public string RbsType { get; set; }
//            public string Site { get; set; }
//            public string AddressRus { get; set; }
//            public string AddressEng { get; set; }
//            public string BranchNameRus { get; set; }
//            public string BranchNameEng { get; set; }
//            public string TechDirRus { get; set; }
//            public string TechDirEng { get; set; }
//            public string TechDirAttorney { get; set; }
//            public string TechDirAttorneyDate { get; set; }
//            public string FinDirRus { get; set; }
//            public string FinDirEng { get; set; }
//            public string FinDirAttorney { get; set; }
//            public string FinDirAttorneyDate { get; set; }
//            public string FinDirRusRP { get; set; }
//            public string TechDirRusRP { get; set; }
//            public string TechDirEngPost { get; set; }
//            public string TechDirRusPost { get; set; }
//            public string TechDirRusPostPR { get; set; }
//            public string BeforeTechDirAttorneyRus { get; set; }
//            public string AfterTechDirAttorneyRus { get; set; }
//            public string BeforeTechDirAttorneyEng { get; set; }
//            public string AfterTechDirAttorneyEng { get; set; }
//            /// <summary>
//            /// Дата от которой печатается заказ
//            /// </summary>
//            public string WoDate { get; set; }
//            /// <summary>
//            /// Дата от которой был напечатан основной заказ
//            /// </summary>
//            public DateTime ParentWoDate { get; set; }
//            /// <summary>
//            /// Главный заказ
//            /// </summary>
//            public string ParentWo { get; set; }
//            /// <summary>
//            /// Печатаемый заказ
//            /// </summary>
//            public string WO { get; set; }
//            /// <summary>
//            /// Сумма печатаемого заказа
//            /// </summary>
//            public decimal VatWo { get; set; }
//            /// <summary>
//            /// НДС печатаемого заказа
//            /// </summary>
//            public decimal Vat { get; set; }

//            #region Поля для Supplement и Work Order

//            /// <summary>
//            /// Общая сумма заказа и всех его дополнений
//            /// </summary>
//            public decimal TotalPrice { get; set; }
//            /// <summary>
//            /// НДС для суммы заказа и всех его дополнений
//            /// </summary>
//            public decimal VatTotalPrice { get; set; }
//            public DateTime WorkStart { get; set; }
//            public DateTime WorkEnd { get; set; }
//            #endregion

//        }
//    }
//}
