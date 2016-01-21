using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbModels.Repository;
using DbModels.DataContext;

namespace TestProject.RepositoryTest
{
    [TestClass]
    public class PriceListRepositoryTest
    {
        [TestMethod]
        public void GetActiveRevisoins()
        {
            
        PriceListRepository rep = new PriceListRepository();
        Context cont = new Context();
        var pls = cont.PriceLists.OrderByDescending(pl=>pl.PriceListRevisions.Count);
        var act = rep.GetActivePriceListsRevisionItems(72,1,DateTime.Now,DateTime.Now);
        
        }
    }
}
