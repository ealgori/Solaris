using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DataContext.Repositories
{
    public class AVRRepository
    {
        /// <summary>
        /// Основное условие вообще обращения нашего внимания на этот авр
        /// </summary>
        private static readonly Expression<Func<ShAVRs,bool>> baseRequest = (a) => 
            
            a.Priority.HasValue
            
            && (a.RukFiliala== "Утвержден")
            && ((a.TotalAmount>50000
                 && (a.RukRegionApproval== "Утвержден"))
                        ||
                (a.TotalAmount<=50000))
            //&&(!string.IsNullOrEmpty(a.AVRType))
            //&& (a.AVRType.Contains("00")||(!a.AVRType.Contains("00")
            //&&a.MSIPApprove
            //))
            &&a.Items!=null&& a.Items.Any()
            // если нет запросов    или                                     если есть комплит                                                   
           
            
            ;
      //  private static readonly Expression<Func<ShAVRs, bool>> IsPrePricedExpr = (a) => a.PrePriced.HasValue;
        //private static readonly Expression<Func<ShAVRs, bool>> WithoutRequest = (a) => (a.ShVCRequests == null || !a.ShVCRequests.Any());
        //private static readonly Expression<Func<ShAVRs,bool>> WithCompletedRequest = (a)=> (a.ShVCRequests != null && a.ShVCRequests.Any() 
        //    //&& VCRequestRepository.CompleteRequestComp(a.ShVCRequests.LastOrDefault())
        //    );
        //private static readonly Expression<Func<ShAVRs, bool>> WithLimitsAndAOS = (a) => a.Items!=null&& a.Items.Any()&&(a.Items.Any(AVRItemRepository.HasLimitComp)) || (a.Items.Any(AVRItemRepository.IsAddonSalesComp));
        //private static readonly Expression<Func<ShAVRs, bool>> WithSuccedRequest = (a)=>   (a.ShVCRequests != null && a.ShVCRequests.Any() 
            //&& VCRequestRepository.SuccessRequestComp(a.ShVCRequests.LastOrDefault())
         //   );
        private static readonly Expression<Func<ShAVRs, bool>> ReadyForPorExpr = (a) => 
            // основной
            BaseComp(a)
           
            &&
            (
             a.ReadyForPOR.HasValue&&a.ReadyForPOR.Value
            )          
             ;
         
        /// <summary>
        /// Первая часть условия попадания в выборку на предопрайсовку или отправку уведомлений
        /// </summary>
 
        private static readonly Expression<Func<ShAVRs,bool>> FirstStageBaseExpr = (a) =>

              // основной
            BaseComp(a)
            && a.InCalculations
            && string.IsNullOrEmpty(a.PurchaseOrderNumber)
              ;
        /// <summary>
        /// условие отправки уведомлений (уже предопрайсовали т.е.)
        /// </summary>
        private static readonly Expression<Func<ShAVRs,bool>> ReadyForRequestExpr = (a) =>

              // основной
              // не обязательно быть препрайст. за рамками лимитов может и не быть
              //IsPrePricedComp(a)&&
             
              FirstStageBaseComp(a)
              && a.ReadyForRequest.HasValue&&a.ReadyForRequest.Value

             ;

        /// <summary>
        /// условие попадания в список на предопрайсовку(дополнительно требуется проверить существование пора! Он должен быть)
        /// </summary>

        private static readonly Expression<Func<ShAVRs,bool>> ReadyForPrePricedExpr = (a) =>

              // основной
             (a.NeedPreprice.HasValue&& a.NeedPreprice==true)
            // &&    BaseComp(a)
            //    && a.InCalculations
           
             ;


           // BaseComp(a) && (a.ShVCRequests != null && a.ShVCRequests.Any() && VCRequestRepository.SuccessRequestComp(a.ShVCRequests.LastOrDefault()));
        public static Func<ShAVRs, bool> ReadyForRequestComp { get { return ReadyForRequestExpr.Compile(); } }
        public static Func<ShAVRs, bool> ReadyForPorComp { get { return ReadyForPorExpr.Compile(); } }
        /// <summary>
        /// он корректно заморожен и у него есть позиции
        /// </summary>
        public static Func<ShAVRs, bool> BaseComp { get { return baseRequest.Compile(); } }
        //public static Func<ShAVRs, bool> WithLimitsAndAOSComp { get { return WithLimitsAndAOS.Compile(); } }
        //public static Func<ShAVRs, bool> WithCompletedRequestComp { get { return WithCompletedRequest.Compile(); } }
        //public static Func<ShAVRs, bool> WithSucceddRequestsComp{get{return WithSuccedRequest.Compile();}}
        // public static Func<ShAVRs, bool> WithotRequestsComp{get{return WithoutRequest.Compile();}}

        /// <summary>
        /// он корректно заморожен, у него есть позиции, а так же по нему считаются лимиты
        /// </summary>
 
        public static Func<ShAVRs, bool> FirstStageBaseComp{get{return FirstStageBaseExpr.Compile();}}
         //public static Func<ShAVRs, bool> IsPrePricedComp{get{return IsPrePricedExpr.Compile();}}
        
         public static Func<ShAVRs, bool> ReadyForPrePricedComp{get{return ReadyForPrePricedExpr.Compile();}}
         public static List<ShAVRs> GetReadyToPORAVRList1(Context context)
         {

          
                 return context.ShAVRs.Include("ShVCRequests").Where(ReadyForPorComp).ToList();
            
         }
        public static List<ShAVRs> GetReadyForPricingAVRList(Context context)
        {
            return context.ShAVRs.Where(BaseComp).ToList();
        }


        public static List<ShAVRs> GetReadyToRequestAVRList(Context context)
        {
                return context.ShAVRs.Include("ShVCRequests").Where(ReadyForRequestComp).ToList();
           
        }

        public static List<ShAVRs> GetReadyToRequestAVRListHighPriority(Context context)
        {

            return context.ShAVRs.Include("ShVCRequests")
            .Where(p=>p.Priority.HasValue&&(p.Priority.Value<=2))
            .Where(ReadyForRequestComp).ToList();
         
        }

        public static List<ShAVRs> GetReadyToRequestAVRListLowPriority(Context context)
        {

            return context.ShAVRs.Include("ShVCRequests")
            .Where(p => p.Priority.HasValue && (p.Priority.Value >=3))
            .Where(ReadyForRequestComp).ToList();

        }

        /// <summary>
        /// Только для пересчета лимитов... позор на мою седую голову....
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        
        
        
        
        //public static List<ShAVRs> GetAvrForLimitsRecalculate(Context context)
        //{
        //    var avrs =  context.ShAVRs.Where(a => a.InCalculations==true).ToList();
        //    var resultAvrList = new List<ShAVRs>();
        //    foreach (var avr in avrs)
        //    {
        //        if(avr.Items.Any(AVRItemRepository.HasLimitComp))
        //        {
        //            resultAvrList.Add(avr);
        //        }
        //    }
        //    return avrs;
        //}

    }


}
