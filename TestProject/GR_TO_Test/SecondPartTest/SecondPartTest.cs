using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using System.Collections.Generic;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle;
using System.Linq;

namespace TestProject.GR_TO_Test.SecondPartTest
{
    [TestClass]
    public class SecondPartTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            //по 2 был ГР, по одному надо отправить, а один отфильтруется по дате
            var logManager = new LogManager();
            var secondPart = new SecondPart();
            var date = new DateTime(2016, 5, 1);
            var shItems = Get5ToItemsWith4Approved();
            var sapItems = GetSapItemWith5Qty2GR();
            var result = secondPart.Handle(shItems, sapItems, date, logManager);
            Assert.AreEqual(result.GRModels.Count, 1);
            Assert.AreEqual(result.GRModels.FirstOrDefault().Qty, 1);
            Assert.AreEqual(result.ShModels.Count, 1);
            Assert.AreEqual(result.ShModels.FirstOrDefault().Qty, 1);


        }


        [TestMethod]
        public void ShouldBeNull()
        {
            //по 2 был ГР, по одному надо отправить, а один отфильтруется по дате
            var logManager = new LogManager();
            var secondPart = new SecondPart();
            var date = new DateTime(2016, 5, 1);
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith5Qty2GR();
            var result = secondPart.Handle(shItems, sapItems, date, logManager);
            Assert.AreEqual(result, null);


        }

        [TestMethod]
        public void ShouldBeNull2()
        {
            //В сапе столько же, сколько и в сх. Первый хендлер обработают эту сиутацию, если она нормальная. если он не обработал, значит не нормальна и ошибка
            var logManager = new LogManager();
            var secondPart = new SecondPart();
            var date = new DateTime(2016, 5, 1);
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith5Qty3GR();
            var result = secondPart.Handle(shItems, sapItems, date, logManager);
            Assert.AreEqual(result, null);


        }


        public static List<ShItemModel> Get5ToItemsWith4Approved()
        {
            return new List<ShItemModel>
            {
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="2", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="3", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="4", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,4,1)},
                new ShItemModel() { Id="5", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,4,1)},
            };
        }


        public static List<ShItemModel> Get5ToItemsWith3Approved()
        {
            return new List<ShItemModel>
            {
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=3, TOFactDate=new DateTime(2016,1,1)}
               
            };
        }


        public static List<SAPItemModel> GetSapItemWith5Qty2GR()
        {
            return new List<SAPItemModel>
            {
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=5, GRQty=2  }
            };
        }

        public static List<SAPItemModel> GetSapItemWith5Qty3GR()
        {
            return new List<SAPItemModel>
            {
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=5, GRQty=3  }
            };
        }
    }
}
