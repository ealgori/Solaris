//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TaskManager.TaskParamModels;
//using OfficeOpenXml;
//using System.IO;
//using TaskManager.Service;

//namespace TaskManager.Handlers.TaskHandlers.Models.PrintOut
//{
//    public class PrintOutHandlerWorkOrder :  APrintOutHandler
//    {
//        public PrintOutHandlerWorkOrder(TaskParameters taskParameters) : base(taskParameters) { }
//    }
//    /// <summary>
//    /// Класс представляет себе главную хранимку, которая выбирает заказы для обработки
//    /// </summary>
//    public class PrintOutProc
//    {
//        public string WO { get; set; }
//        public string Element { get; set; }
//        public string MacroRegion { get; set; }
//        public string Region { get; set; }
//    }
//    /// <summary>
//    /// Класс для замены текста в заказе
//    /// </summary>
//    public class PrintOutDataWorkOrder
//    {
//        public string RbsType { get; set; }
//        public string Site { get; set; }
//        public string AddressRus { get; set; }
//        public string AddressEng { get; set; }
//        public string BranchNameRus { get; set; }
//        public string BranchNameEng { get; set; }
//        public string TechDirRus { get; set; }
//        public string TechDirEng { get; set; }
//        public string TechDirAttorney { get; set; }
//        public string TechDirAttorneyDate { get; set; }
//        public string FinDirRus { get; set; }
//        public string FinDirEng { get; set; }
//        public string FinDirAttorney { get; set; }
//        public string FinDirAttorneyDate { get; set; }
//        public string FinDirRusRP { get; set; }
//        public string TechDirRusRP { get; set; }
//        public string TechDirEngPost { get; set; }
//        public string TechDirRusPost { get; set; }
//        public string TechDirRusPostPR { get; set; }
//        public string BeforeTechDirAttorneyRus { get; set; }
//        public string AfterTechDirAttorneyRus { get; set; }
//        public string BeforeTechDirAttorneyEng { get; set; }
//        public string AfterTechDirAttorneyEng { get; set; }
//        /// <summary>
//        /// Дата от которой печатается заказ
//        /// </summary>
//        public string WoDate { get; set; }
//        /// <summary>
//        /// Главный заказ
//        /// </summary>
//        public string ParentWo { get; set; }
//        /// <summary>
//        /// Печатаемый заказ
//        /// </summary>
//        public string WO { get; set; }
//        /// <summary>
//        /// Сумма печатаемого заказа
//        /// </summary>
//        public decimal VatWo { get; set; }
//        /// <summary>
//        /// НДС печатаемого заказа
//        /// </summary>
//        public decimal Vat { get; set; }
//        /// <summary>
//        /// Перевод каретки для колонтитулов
//        /// </summary>
//        public string NewLine
//        { 
//            get { return Environment.NewLine;}
//        }
//    }
//}
