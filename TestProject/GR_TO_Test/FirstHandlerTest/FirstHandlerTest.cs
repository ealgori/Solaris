using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO;

namespace TestProject.GR_TO_Test.FirstHandlerTest
{
    /// <summary>
    /// Summary description for FirstHandlerTest
    /// </summary>
    [TestClass]
    public class FirstHandlerTest
    {


        [TestMethod]
        public void ShouldBe3True()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith5Qty2GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.AreEqual(result.ShModels.Count, 3);
            Assert.IsTrue(result.Succeed);
            

        }


        [TestMethod]
        public void ShouldBe3Null3()
        {
            /// в сх в одной из позиций есть количество 3
            /// в сапе принято 2
            /// на данном этапе фирст вернет эту позицю с количеством 3 и произойдет постинг 3 GR.
            /// теоретически надо заменять количество. либо валиться.
            /// оставим эту проблему на 2 хэндлер.

            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved2();
            var sapItems = GetSapItemWith5Qty2GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.AreEqual(result, null);
           


        }


        [TestMethod]
        public void ShouldBe3False2()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = Get3SapItemWith5Qty3GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.AreEqual(result.ShModels.Count, 3);
            Assert.IsFalse(result.Succeed);


        }


        [TestMethod]
        public void ShouldBe3False()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith5Qty3GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.AreEqual(result.ShModels.Count, 3);
            Assert.IsFalse(result.Succeed);


        }

        [TestMethod]
        public void ShouldBeNull()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith5Qty4GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.IsNull(result);
          


        }

        [TestMethod]
        public void ShouldBeNull2()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWith3Approved();
            var sapItems = GetSapItemWith4Qty4GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.IsNull(result);



        }


        // проверить пустое количество или цену
        [TestMethod]

        public void ShouldBeNull3()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems = Get5ToItemsWithEmptyQty();
            var sapItems = GetSapItemWith5Qty3GR();
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.IsNull(result);
        }



        // проверить отсутсвтиве сап 


        [TestMethod]

        public void ShouldBeNull4()
        {
            var logManager = new LogManager();
            var firstPart = new FirstPart();
            var shItems =Get5ToItemsWith3Approved2();
            List<SAPItemModel> sapItems = null;
            var result = firstPart.Handle(shItems, sapItems, logManager);
            Assert.IsNull(result);
        }
        // проверить отсутствие количества




        public static List<ShItemModel> Get5ToItemsWith3Approved()
        {
            return new List<ShItemModel>
            {
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1},
            };
        }

        public static List<ShItemModel> Get5ToItemsWith3Approved2()
        {
            return new List<ShItemModel>
            {
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=3, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)}
              
            };
        }


        public static List<ShItemModel> Get5ToItemsWithEmptyQty()
        {
            return new List<ShItemModel>
            {
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, TOFactDate=new DateTime(2016,1,1)},
                new ShItemModel() { Id="1", GR="", MaterialCode="ECR-TEST-1", Price=10, Qty=1, TOFactDate=new DateTime(2016,1,1)}

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

        public static List<SAPItemModel> GetSapItemWith5Qty4GR()
        {
            return new List<SAPItemModel>
            {
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=5, GRQty=4  }
            };
        }

        public static List<SAPItemModel> GetSapItemWith4Qty4GR()
        {
            return new List<SAPItemModel>
            {
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=4, GRQty=4  }
            };
        }

        public static List<SAPItemModel> Get3SapItemWith5Qty3GR()
        {
            return new List<SAPItemModel>
            {
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=1, GRQty=1  },
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=1, GRQty=1  },
                 new SAPItemModel { POItemId="10", MaterialCode ="ECR-TEST-1",  Price=10, QtyOrdered=3, GRQty=1  }
            };
        }

    }
}
