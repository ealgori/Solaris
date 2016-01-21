//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.IO;
//using TaskManager.Service;
//using OfficeOpenXml;
////using TaskManager.Handlers.TaskHandlers.Models.POR;
//using DbModels.DataContext;
//using TaskManager;
//using TaskManager.TaskModel;
//using TaskManager.TaskParamModels;

//namespace TestProject
//{
//    [TestClass]
//    public class TestPORtoSH
//    {
//        public const string WorkOrderSheet = "Work Order";
//        public const string SoWSheet = "SoW";
//        public const string TempSoWSheet = "Temp SoW";
//        public const string GantChartSheet = "Gant Chart";
//        [TestMethod]
//        public void TestSowMapping()
//        {
//            using (Context context = new Context())
//            {
//                var row = context.SOWMapping.FirstOrDefault(i => i.Id == 32);
//            }
//        }
//        /// <summary>
//        /// Тест получения даты ПОРа
//        /// </summary>
//        [TestMethod]
//        public void TestPriceDate()
//        {
//            using (Context context = new Context())
//            {
//                //дата никуда не попадает
//                var date1 = SHPORService.GetPriceDate(new DateTime(2013, 3, 7), context);
//                Assert.AreEqual(date1, new DateTime(2013, 2, 28));
//                //дата попадает на праздники
//                var date2 = SHPORService.GetPriceDate(new DateTime(2013, 3, 3), context);
//                Assert.AreEqual(date2, new DateTime(2013, 2, 22));
//                //Дата в будущем
//                var date3 = SHPORService.GetPriceDate(DateTime.Now.AddDays(40), context);
//                Assert.AreEqual(date3, DateTime.Now);
//            }
//        }
//    }
//}
