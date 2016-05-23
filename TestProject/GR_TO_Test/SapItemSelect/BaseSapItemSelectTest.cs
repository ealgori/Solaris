using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapItemSelect;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using System.Collections.Generic;
using System.Linq;

namespace TestProject.GR_TO_Test.SapItemSelect
{
    [TestClass]
    public class BaseSapItemSelectTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var selector = new BaseSapItemsSelect();
            var sapItems = GetSapItems();
            List<GRItemModel> grModels = null;
            // все норм
            decimal summ = 12;
            if (selector.Select(sapItems, summ, out grModels))
            {
                Assert.AreEqual(grModels.Sum(g => g.Qty), summ);
            }
            else
                Assert.Fail();
            // столько нет
            if (selector.Select(sapItems, 72, out grModels))
            {
                Assert.Fail();
            }

               // отрицательное
            if (selector.Select(sapItems, -2, out grModels))
            {

                Assert.Fail();
            }

            // дробное
            summ = 12.2M;
            if (selector.Select(sapItems, 12.2M, out grModels))
            {
                Assert.AreEqual(grModels.Sum(g => g.Qty), summ);
            }
            else
                Assert.Fail();


        }
        public static List<SAPItemModel> GetSapItems()
        {
            var sapItems = new List<SAPItemModel>();
            sapItems.Add(new SAPItemModel { POItemId = "10", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "20", GRQty = 2,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "30", GRQty = 3,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "40", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "50", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "60", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "70", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            sapItems.Add(new SAPItemModel { POItemId = "80", GRQty = 0,  MaterialCode = "ECR-CODE-123", QtyOrdered = 12, Price = 123 });
            return sapItems;

        }

    }



  

}
