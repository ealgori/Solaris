//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using DbModels.DomainModels.ShClone;
//using DbModels.DataContext.Repositories;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;

//namespace TestProject.RepositoryTest
//{
//    [TestClass]
//    public class AVRRepositoryTest
//    {

//        private ShAVRs GetAVRLit50000()
//        {
//            var avr = new ShAVRs();
//            avr.Priority = 1;
//            avr.AVRType = "00";
//            avr.TotalAmount = 49999;
//            avr.RukFiliala = "Утвержден";
//            avr.ShVCRequests = new List<ShVCRequest>();
//            avr.PrePriced = DateTime.Now ;
//            avr.Items = new List<ShAVRItem>();

//            avr.Items.Add(new ShAVRItem() {  ECRType = "test"});



//            return avr;
//        }






//        private ShAVRs GetReadyForRequestAVR()
//        {
//            var avr = GetAVRLit50000();

//            return avr;
//        }

//        private ShAVRs GetUnReadyForRequestAVR()
//        {
//            var avr = GetAVRLit50000();
//            var request = new ShVCRequest();
//            request.NotificationSend = DateTime.Now;
//            avr.ShVCRequests.Add(request);
//            return avr;
//        }

//        private ShAVRs GetUnReadyForRequestAVR_RejectedRequest()
//        {
//            var avr = GetAVRLit50000();
//            var request = new ShVCRequest();

//            request.NotificationSend = null;
//            request.RequestSend = DateTime.Now;
//            request.RequestReject = DateTime.Now;
//            avr.ShVCRequests.Add(request);
//            return avr;
//        }

//        private ShAVRs GetUnReadyForRequestAVR_InCompleteRequest()
//        {
//            var avr = GetAVRLit50000();
//            var request = new ShVCRequest();
//            request.NotificationSend = null;
//            request.RequestSend = DateTime.Now;
//            avr.ShVCRequests.Add(request);
//            return avr;
//        }



//        private ShAVRs GetAVRBig50000()
//        {
//            var avr = GetAVRLit50000();
//            avr.TotalAmount = 50001;
//            return avr;
//        }

//        private ShAVRs GetAVRBig5000WRukReg()
//        {
//            var avr = GetAVRBig50000();
//            avr.RukRegionApproval = "Утвержден";
//            return avr;
//        }

//        private ShAVRs GetAVRBig5000WRukRegWOType()
//        {
//            var avr = GetAVRBig50000();
//            avr.RukRegionApproval = "Утвержден";
//            avr.AVRType = "";
//            return avr;
//        }

//        [TestMethod]
//        public void ShouldHavePriority_true()
//        {

//            var result = AVRRepository.ReadyForRequestComp(GetAVRLit50000());
//            Assert.IsTrue(result);

//        }
//        [TestMethod]
//        public void ShouldHaveRukRegApprove_false()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetAVRBig50000());
//            Assert.IsFalse(result);
//        }
//        [TestMethod]
//        public void ShouldHaveRukRegApprove_true()
//        {
//            var avr = GetAVRBig5000WRukReg();
           
//            var result = AVRRepository.ReadyForRequestComp(avr);
//            Assert.IsTrue(result);
//        }
//        [TestMethod]
//        public void ShouldHaveAVRType_false()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetAVRBig5000WRukRegWOType());
//            Assert.IsFalse(result);
//        }
//        [TestMethod]
//        private void SouldHavePriority()
//        {
//            var avr = GetAVRLit50000();
//            avr.Priority = null;
//            var result = AVRRepository.ReadyForRequestComp(avr);
//            Assert.IsFalse(result);


//        }

//        [TestMethod]
//        public void ShouldSendRequest()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetReadyForRequestAVR());
//            Assert.IsTrue(result);
//        }
//        [TestMethod]
//        public void ShouldSendRequest2()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetUnReadyForRequestAVR_RejectedRequest());
//            Assert.IsTrue(result);
//        }
//        [TestMethod]
//        public void ShouldNotSendRequest()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetUnReadyForRequestAVR_InCompleteRequest());
//            Assert.IsFalse(result);
//        }

//        [TestMethod]
//        public void ShouldNotSendRequest2()
//        {
//            var result = AVRRepository.ReadyForRequestComp(GetUnReadyForRequestAVR());
//            Assert.IsFalse(result);
//        }
//        // еще нужны тесты на реди фо пор

//        [TestMethod]
//        public void ShouldSendPorNoLimitsAndAOS()
//        {

//            var avr = GetAVRLit50000();
//            avr.Items.FirstOrDefault().ECRType = "";
//            avr.Items.Add(new ShAVRItem() { });
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, true);
//        }
//        [TestMethod]
//        public void ShouldNotSendPorNoItems()
//        {
//            var avr = GetAVRLit50000();
//            avr.Items = null;//new List<ShAVRItem>(); ;
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, false);

//        }

//        [TestMethod]
//        public void ShouldNotSendPorNoRequests()
//        {
//            var avr = GetAVRLit50000();
//            avr.Items.Add(new ShAVRItem() { ECRType = "asdf" });
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, false);
//        }

//        [TestMethod]
//        public void ShouldSendPorCompleteRequests()
//        {
//            var avr = GetAVRLit50000();
//            avr.Items.Add(new ShAVRItem() { ECRType = "asdf" });
//            avr.ShVCRequests =  new List<ShVCRequest>();
//            avr.ShVCRequests.Add(new ShVCRequest() { Id = "asdf", RequestSend = DateTime.Now, RequestAccept = DateTime.Now });
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, true);
//        }

//        [TestMethod]
//        public void ShouldNotSendPorRejectedRequests()
//        {
//            var avr = GetAVRLit50000();
//            avr.Items.Add(new ShAVRItem() { ECRType = "asdf" });
//            avr.ShVCRequests = new List<ShVCRequest>();
//            avr.ShVCRequests.Add(new ShVCRequest() { Id = "asdf", RequestSend = DateTime.Now, RequestReject = DateTime.Now });
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, false);
//        }

//        [TestMethod]
//        public void ShouldSendPorCompleteRequestAfterReject()
//        {
//            var avr = GetAVRLit50000();
//            avr.Items.Add(new ShAVRItem() { ECRType = "asdf" });
//            avr.ShVCRequests = new List<ShVCRequest>();
//            avr.ShVCRequests.Add(new ShVCRequest() { Id = "asdf", RequestSend = DateTime.Now, RequestReject = DateTime.Now });
//            avr.ShVCRequests.Add(new ShVCRequest() { Id = "asdf2", RequestSend = DateTime.Now, RequestAccept = DateTime.Now });
//            var result = AVRRepository.ReadyForPorComp(avr);
//            Assert.AreEqual(result, true);
//        }




//        //private static ShAVRItem  CreateAVRItem()
//        //{
//        //    var item = new ShAVRItem();
//        //    return item;
//        //}


//        //[TestMethod]
//        //public void IWoLimit()
//        //{
//        //    var item = CreateAVRItem();
//        //    var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //    var result2 = AVRItemRepository.IsAddonSalesComp(item);
//        //    var result3 = AVRItemRepository.HasLimitComp(item);
//        //    var result4 = AVRItemRepository.HasExceedLimitComp(item);


//        //    Assert.AreEqual(result1||result2||result3||result4, false);
//        //}

//        //[TestMethod]
//        //public void IWLimit()
//        //{
//        //    var item = CreateAVRItem();
//        //    item.InLimit = true;
//        //    var limit = new ShLimit();
//        //    item.Limit = limit;
//        //    var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //    var result2 = AVRItemRepository.IsAddonSalesComp(item);
//        //    var result3 = !AVRItemRepository.HasLimitComp(item);
//        //    var result4 = AVRItemRepository.HasExceedLimitComp(item);


//        //    Assert.AreEqual(result1 || result2 || result3 || result4, false);
//        //}


//        //[TestMethod]
//        //public void IWLimitMustBeCounted()
//        //{
//        //    var item = CreateAVRItem();
//        //    item.InLimit = true;
//        //    var limit = new ShLimit();
//        //    item.Limit = limit;
//        //    var result1 = AVRItemRepository.CalcLimitComp(item);


//        //    Assert.AreEqual(result1, true);
//        //}

//        //[TestMethod]
//        //public void IWExcLimit()
//        //{
//        //    var item = CreateAVRItem();
//        //    var limit = new ShLimit();
//        //    limit.Executed = 1000;
//        //    limit.Limit = 100;
//        //    item.Limit = limit;
//        //    var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //    var result2 = !AVRItemRepository.IsAddonSalesComp(item);
//        //    var result3 = AVRItemRepository.HasLimitComp(item);
//        //    var result4 = AVRItemRepository.HasExceedLimitComp(item);


//        //    Assert.AreEqual(result1 && result2 && result3 && result4, true);
//        //}

//        // [TestMethod]
//        //public void IWAOS()
//        //{
//        //    var item = CreateAVRItem();
//        //    item.ECRType = "test";
//        //    var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //    var result2 = AVRItemRepository.IsAddonSalesComp(item);
//        //    var result3 = !AVRItemRepository.HasLimitComp(item);
//        //    var result4 = !AVRItemRepository.HasExceedLimitComp(item);


//        //    Assert.AreEqual(result1 && result2 && result3 && result4, true);
//        //}


//        // [TestMethod]
//        // public void IWAOSLimit()
//        // {
//        //     var item = CreateAVRItem();
//        //     item.InLimit = true;
//        //     item.ECRType = "test";
//        //     var limit = new ShLimit();
//        //     item.Limit = limit;
//        //     var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //     var result2 = AVRItemRepository.IsAddonSalesComp(item);
//        //     var result3 = AVRItemRepository.HasLimitComp(item);
//        //     var result4 = !AVRItemRepository.HasExceedLimitComp(item);


//        //     Assert.AreEqual(result1 && result2 && result3 && result4, true);
//        // }


//        // [TestMethod]
//        // public void IWAOSExcLimit()
//        // {
//        //     var item = CreateAVRItem();
//        //     item.ECRType = "test";
//        //     var limit = new ShLimit();
//        //     limit.Executed = 1000;
//        //     limit.Limit = 100;
//        //     item.Limit = limit;
//        //     var result1 = AVRItemRepository.IsAddonSalesOrExceedComp(item);
//        //     var result2 = AVRItemRepository.IsAddonSalesComp(item);
//        //     var result3 = AVRItemRepository.HasLimitComp(item);
//        //     var result4 = AVRItemRepository.HasExceedLimitComp(item);


//        //     Assert.AreEqual(result1 && result2 && result3 && result4, true);
//        // }




        
//    }
//}
