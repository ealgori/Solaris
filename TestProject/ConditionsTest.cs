using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbModels.DomainModels.ShClone;
using System.Collections.Generic;

using DbModels.DataContext;
using DbModels.DataContext.AVRConditions;
using DbModels.AVRConditions;

namespace TestProject
{
    [TestClass]
    public class ConditionsTest
    {
        #region CrateAVRs

        public ShAVRs CreateTestAvr()
        {
            return new ShAVRs();
        }

        public ShAVRs CreateFreezedAvr()
        {
            var avr = CreateTestAvr();
            avr.RukFiliala = "Утвержден";
            avr.RukOtdela = "Утвержден";
            avr.RukRegionApproval = "Утвержден";
            return avr;
        }

        public ShAVRs CreateRegularFreezedAvr()
        {
            var avr = CreateFreezedAvr();
            avr.AVRType = "00";
            return avr;
        }

        public ShAVRs CreatePerevistavlAvr()
        {
            var avr = CreateRegularFreezedAvr();
            avr.AVRType = "05";
            return avr;
        }
        #endregion
        #region CreateAVRItems
        public void AddAvrItem(ShAVRs shAvr)
        {
            if (shAvr.Items == null)
                shAvr.Items = new List<ShAVRItem>();
            shAvr.Items.Add(new ShAVRItem());
        }

        public void AddAvrAOSItem(ShAVRs shAvr)
        {
            if (shAvr.Items == null)
                shAvr.Items = new List<ShAVRItem>();
            var item = new ShAVRItem();
            item.VCAddOnSales = true;
            shAvr.Items.Add(item);
        }
        public void AddAvrInLimItem(ShAVRs shAvr)
        {
            if (shAvr.Items == null)
                shAvr.Items = new List<ShAVRItem>();
            var item = new ShAVRItem();
            item.Limit = new ShLimit();
            item.InLimit = true;
            
            shAvr.Items.Add(item);
        }

        public void AddAvrOutLimItem(ShAVRs shAvr)
        {
            if (shAvr.Items == null)
                shAvr.Items = new List<ShAVRItem>();
            var item = new ShAVRItem();
            item.Limit = new ShLimit();
            item.InLimit = false;
            shAvr.Items.Add(item);
        }
        #endregion
        #region CreateConditions
        public class ConditionsClass
        {
            public IAVRCondition NeedPriceCondition = new NeedPriceCondition();
            public IAVRCondition NeedVCPriceCondition = new NeedVCPriceCondition();
            public IAVRCondition NeedMus = new NeedMUSCondition();
            public IAVRCondition PorAcccessible = new PORAccessibleCondition(new NeedPriceCondition());
            public IAVRCondition ReadyToRequest = new ReadyToRequestCondition();

        }

        public ConditionsClass conditions = new ConditionsClass();
        public Context Context = new Context();
        #endregion




        [TestMethod]
        public void CheckRegularAVR()
        {
            var avr = CreateRegularFreezedAvr();
            Assert.IsTrue(conditions.NeedPriceCondition.IsSatisfy(avr,Context));
            Assert.IsFalse(conditions.NeedVCPriceCondition.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.NeedMus.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.PorAcccessible.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.ReadyToRequest.IsSatisfy(avr, Context));

        }
        [TestMethod]
        public void ChecReexposeAVR()
        {
            var avr = CreatePerevistavlAvr();
            Assert.IsTrue(conditions.NeedPriceCondition.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.NeedVCPriceCondition.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.NeedMus.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.PorAcccessible.IsSatisfy(avr, Context));
            Assert.IsFalse(conditions.ReadyToRequest.IsSatisfy(avr, Context));

        }
    }
}
