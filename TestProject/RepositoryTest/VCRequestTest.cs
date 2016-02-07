//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using DbModels.DomainModels.ShClone;
//using DbModels.DataContext.Repositories;

//namespace TestProject.RepositoryTest
//{
//    [TestClass]
//    public class VCRequestTest
//    {
//        private ShVCRequest CreateVCRequest()
//        {
//            var request = new ShVCRequest();
//            return request;
//        }
//        private ShVCRequest CreateCompletedVCRequest_Request()
//        {
//            var request = CreateInCompletedVCRequest_Request();
//            request.RequestAccept = DateTime.Now;
//            return request;
//        }
//        private ShVCRequest CreateInCompletedVCRequest_Request()
//        {
//            var request = CreateVCRequest();
//            request.RequestSend = DateTime.Now;

//            return request;
//        }


//        private ShVCRequest CreateInCompletedVCRequest_Order()
//        {
//            var request = CreateVCRequest();
//            request.OrderSend = DateTime.Now;

//            return request;
//        }

//        private ShVCRequest CreateCompletedVCRequest_Order()
//        {
//            var request = CreateInCompletedVCRequest_Order();
//            request.OrderAccept = DateTime.Now;

//            return request;
//        }

//        private ShVCRequest CreateInCompletedVCRequest_RequestOrder()
//        {
//            var request = CreateCompletedVCRequest_Request();
//            request.OrderSend = DateTime.Now;

//            return request;
//        }

//        private ShVCRequest CreateInCompletedVCRequest_OrderRequest()
//        {
//            var request = CreateCompletedVCRequest_Order();
//            request.RequestSend = DateTime.Now;

//            return request;
//        }

//        private ShVCRequest CreateInCompletedVCRequest_OrderRequest2()
//        {
//            var request = CreateInCompletedVCRequest_Order();
//            request.RequestSend = DateTime.Now;

//            return request;
//        }

//        private ShVCRequest CreateCompletedVCRequest_OrderRequest()
//        {
//            var request = CreateCompletedVCRequest_Order();
//            request.RequestSend = DateTime.Now;
//            request.RequestAccept = DateTime.Now;
//            return request;
//        }

//        private ShVCRequest CreateCompletedVCRequest_Notify()
//        {
//            var request = CreateVCRequest();
//            request.NotificationSend = DateTime.Now;
//            return request;
//        }
        
//        private ShVCRequest CreateSuccedVC_Request_Notify()
//        {
//            return CreateCompletedVCRequest_Notify();
//        }

//        private ShVCRequest CreateSuccedVCRequest_Request()
//        {
//            return CreateCompletedVCRequest_Request();
//        }
//        private ShVCRequest CreateUnSuccedVCRequest_Request()
//        {
//            var avr = CreateCompletedVCRequest_Request();
//            avr.RequestAccept = null;
//            avr.RequestReject= DateTime.Now;
//            return avr ;
//        }

//        private ShVCRequest CreateSuccedVCRequest_Order()
//        {
//            return CreateCompletedVCRequest_Order();
//        }
//        private ShVCRequest CreateUnSuccedVCRequest_Order()
//        {
//            var avr = CreateCompletedVCRequest_Order();
//            avr.OrderAccept = null;
//            avr.OrderReject = DateTime.Now;
//            return avr;
//        }

//        private ShVCRequest CreateSuccedVCRequest_OrderRequest()
//        {
//            return CreateCompletedVCRequest_OrderRequest();
//        }
//        private ShVCRequest CreateUnSuccedVCRequest_OrderRequest()
//        {
//            var avr = CreateSuccedVCRequest_OrderRequest();
//            avr.OrderAccept = null;
//            avr.OrderReject = DateTime.Now;
//            return avr;
//        }

//        private ShVCRequest CreateUnSuccedVCRequest_RequestOrder()
//        {
//            var avr = CreateSuccedVCRequest_OrderRequest();
//            avr.RequestAccept = null;
//            avr.RequestReject = DateTime.Now;
//            return avr;
//        }
//        private ShVCRequest CreateUnSuccedVCRequest_RequestOrder2()
//        {
//            var avr = CreateSuccedVCRequest_OrderRequest();
//            avr.RequestAccept = null;
//            avr.OrderAccept = null;
//            avr.RequestReject = DateTime.Now;
//            avr.OrderReject = DateTime.Now;
//            return avr;
//        }


        
//        [TestMethod]
//        public void IncompletedRequest()
//        {
//            var request = CreateInCompletedVCRequest_Request();
//            Assert.IsFalse(VCRequestRepository.CompleteRequest(request));
//        }


//        [TestMethod]
//        public void CompletedRequest()
//        {
//            var request = CreateCompletedVCRequest_Request();
//            Assert.IsTrue(VCRequestRepository.CompleteRequest(request));
//        }


//        [TestMethod]
//        public void IncompletedOrder()
//        {
//            var request = CreateInCompletedVCRequest_Order();
//            Assert.IsFalse(VCRequestRepository.CompleteRequest(request));
//        }


//        [TestMethod]
//        public void CompletedOrder()
//        {
//            var request = CreateCompletedVCRequest_Order();
//            Assert.IsTrue(VCRequestRepository.CompleteRequest(request));
//        }


//        [TestMethod]
//        public void IncompletedRequestOrder()
//        {
//            var request = CreateInCompletedVCRequest_RequestOrder();
//            Assert.IsFalse(VCRequestRepository.CompleteRequest(request));
//        }
//        [TestMethod]
//        public void IncompletedOrderRequest()
//        {
//            var request = CreateInCompletedVCRequest_OrderRequest();
//            Assert.IsFalse(VCRequestRepository.CompleteRequest(request));
//        }


//        [TestMethod]
//        public void IncompletedOrderRequest2()
//        {
//            var request = CreateInCompletedVCRequest_OrderRequest2();
//            Assert.IsFalse(VCRequestRepository.CompleteRequest(request));
//        }

//        [TestMethod]
//        public void CompletedOrderRequest()
//        {
//            var request = CreateCompletedVCRequest_OrderRequest();
//            Assert.IsTrue(VCRequestRepository.CompleteRequest(request));
//        }

//        [TestMethod]
//        public void CompletedNotify()
//        {
//            var request = CreateCompletedVCRequest_Notify();
//            Assert.IsTrue(VCRequestRepository.CompleteRequest(request));
//        }
//        // succedd
//        [TestMethod]
//        public void SuccesNotify()
//        {
//            var request = CreateSuccedVC_Request_Notify();
//            Assert.IsTrue(VCRequestRepository.SuccessRequest(request));
//        }
//        [TestMethod]
//        public void SuccedRequest()
//        {
//            var request = CreateSuccedVCRequest_Request();
//            Assert.IsTrue(VCRequestRepository.SuccessRequest(request));
//        }


//        [TestMethod]
//        public void UnSuccedRequest_Request()
//        {
//            var request = CreateUnSuccedVCRequest_Request();
//            Assert.IsFalse(VCRequestRepository.SuccessRequest(request));
//        }
//        [TestMethod]
//        public void SucccedRequest_Order()
//        {
//            var request = CreateSuccedVCRequest_Order();
//            Assert.IsTrue(VCRequestRepository.SuccessRequest(request));
//        }

//        [TestMethod]
//        public void UnSucccedRequest_Order()
//        {
//            var request = CreateUnSuccedVCRequest_Order();
//            Assert.IsFalse(VCRequestRepository.SuccessRequest(request));
//        }
//        [TestMethod]
//        public void SucccedRequest_OrderRequest()
//        {
//            var request = CreateSuccedVCRequest_OrderRequest();
//            Assert.IsTrue(VCRequestRepository.SuccessRequest(request));
//        }

//        [TestMethod]
//        public void UnSucccedRequest_OrderRequest()
//        {
//            var request = CreateUnSuccedVCRequest_OrderRequest();
//            Assert.IsFalse(VCRequestRepository.SuccessRequest(request));
//        }
//        [TestMethod]
//        public void UnSucccedRequest_RequestOrder()
//        {
//            var request = CreateUnSuccedVCRequest_RequestOrder();
//            Assert.IsFalse(VCRequestRepository.SuccessRequest(request));
//        }
//        [TestMethod]
//        public void UnSucccedRequest_RequestOrder2()
//        {
//            var request = CreateUnSuccedVCRequest_RequestOrder2();
//            Assert.IsFalse(VCRequestRepository.SuccessRequest(request));
//        }


//    }
//}
