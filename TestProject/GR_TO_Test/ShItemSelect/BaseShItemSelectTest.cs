using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO;

namespace TestProject.GR_TO_Test.ShItemSelect
{
    [TestClass]
    public class BaseShItemSelectTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var baseShItemSelect = new BaseShItemSelect();
            var models = GetShModels();
            decimal summ = 15;
            List<ShItemModel> selected = null;
            // две позиции
            if (baseShItemSelect.Select(models, summ, out selected))
            {
                if (selected.Sum(s => s.Qty) != summ)
                    Assert.Fail();
            }
            else
                Assert.Fail();

            // конкретная сумма
            summ = 5;
            if (baseShItemSelect.Select(models, summ, out selected))
            {
                if (selected.Sum(s => s.Qty) != summ)
                    Assert.Fail();
            }
            else
                Assert.Fail();

            // нет такого
            summ = -3;
            if (baseShItemSelect.Select(models, summ, out selected))
            {
               
                    Assert.Fail();
            }


            // дробная
            summ = 6.3M;
            if (baseShItemSelect.Select(models, summ, out selected))
            {
                if (selected.Sum(s => s.Qty) != summ)
                    Assert.Fail();
            }
            else
                Assert.Fail();

            // отсутствие количества и цены
            // присутствие GR
            
             





        }


        public static List<ShItemModel> GetShModels()
        {
            var models = new List<ShItemModel>();
            models.Add(new ShItemModel { Id="1",  TOFactDate = new DateTime(2016,1,2), Price=12, Qty=1 });
            models.Add(new ShItemModel { Id = "2", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 2 });
            models.Add(new ShItemModel { Id = "3", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 3 });
            models.Add(new ShItemModel { Id = "4", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 4 });
            models.Add(new ShItemModel { Id = "5", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 5 });
            models.Add(new ShItemModel { Id = "6", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 6 });
            models.Add(new ShItemModel { Id = "7", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 7 });
            models.Add(new ShItemModel { Id = "8", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 8 });
            models.Add(new ShItemModel { Id = "9", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 9 });
            models.Add(new ShItemModel { Id = "9", TOFactDate = new DateTime(2016, 1, 2), Price = 12, Qty = 2.3M });

            return models;
        }
    }
}
