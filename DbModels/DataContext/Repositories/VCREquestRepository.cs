using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DataContext.Repositories
{
    public class VCRequestRepository
    {
         //<summary>
         //SUCCESS подразумевает компит
         //</summary>
        //public static Expression<Func<ShVCRequest, bool>> SuccessRequestExpr = (r) =>
        //        r.RequestSend.HasValue && !r.OrderSend.HasValue && r.RequestAccept.HasValue // если был реквест и он аксепт
        //    || !r.RequestSend.HasValue && r.OrderSend.HasValue && r.OrderAccept.HasValue // если был заказ и он аксепт
        //    || r.RequestSend.HasValue && r.OrderSend.HasValue && r.RequestAccept.HasValue && r.OrderAccept.HasValue // если и заказ и ревест и аксепты
        //    || r.NotificationSend.HasValue; // если был низкий приоритет, то достаточно уведомления.
        //public static Expression<Func<ShVCRequest, bool>> CompleteRequestExpr = (r) =>
        //        r.RequestSend.HasValue && !r.OrderSend.HasValue &&(r.RequestAccept.HasValue||r.RequestReject.HasValue) // если был реквест и он аксепт или реджект
        //    || !r.RequestSend.HasValue && r.OrderSend.HasValue && (r.OrderAccept.HasValue||r.OrderReject.HasValue) // если был заказ и он аксепт
        //    || r.RequestSend.HasValue && r.OrderSend.HasValue && (r.RequestAccept.HasValue||r.RequestReject.HasValue) && (r.OrderAccept.HasValue||r.OrderReject.HasValue) // если и заказ и ревест и аксепты или реджекты
        //    || r.NotificationSend.HasValue; // если был низкий приоритет, то достаточно уведомления.

        private static Expression<Func<ShVCRequest, bool>> SuccessRequestExpr = (r) =>

             // если значение не проставлено, либо фолс на голосование и заказ

           r.RequestSend.HasValue&&(!r.HasRequest || (r.HasRequest && (r.RequestAccepted.HasValue&&!r.RequestRejected.HasValue))) && (!r.HasOrder || (r.HasOrder && (r.OrderAccepted.HasValue&&!r.OrderRejected.HasValue)));
           
           
           
        // то же что ни саксесс, только добавляется проверка на реджект 
        private static Expression<Func<ShVCRequest, bool>> CompleteRequestExpr = (r) =>
               r.RequestSend.HasValue&&(!r.HasRequest || (r.HasRequest && (r.RequestAccepted.HasValue||r.RequestRejected.HasValue))) && (!r.HasOrder || (r.HasOrder && (r.OrderAccepted.HasValue|| r.OrderRejected.HasValue)));

        private static Expression<Func<ShVCRequest, bool>> UnSuccessRequestExpr = (r) =>
            r.RequestSend.HasValue && ((r.HasOrder && r.OrderRejected.HasValue) || (r.HasRequest && r.RequestRejected.HasValue));

        private static Expression<Func<ShVCRequest, bool>> UnsendExpr = (r) =>
             r.SendRequest && !r.RequestSend.HasValue;

        private static Expression<Func<ShVCRequest, bool>> SendExpr = (r) =>
            r.SendRequest && r.RequestSend.HasValue;


        public static Func<ShVCRequest, bool> SuccessRequest { get { return SuccessRequestExpr.Compile(); } }

        /// <summary>
        /// Смотрит только на отправленные реквесты
        /// </summary>
        public static Func<ShVCRequest, bool> CompleteRequest { get { return CompleteRequestExpr.Compile(); } }

        public static Func<ShVCRequest, bool> UnSuccessRequest { get { return UnSuccessRequestExpr.Compile(); } }
        public static Func<ShVCRequest, bool> UnsendRequest { get { return UnsendExpr.Compile(); } }
        public static Func<ShVCRequest, bool> SendRequest { get { return SendExpr.Compile(); } }

        public static List<ShVCRequest> GetCheckedVCRequests(string avrId, Context context)
        {
            return context.ShVCRequests.Where(r => r.SendRequest&&r.ShAVRs.AVRId==avrId).ToList();
        }

    }
}
