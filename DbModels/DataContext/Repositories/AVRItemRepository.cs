using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DataContext.Repositories
{
    public static  class AVRItemRepository
    {
        private static Expression<Func<ShAVRItem, bool>> isAddonSalesExp = (i) => !string.IsNullOrEmpty(i.ECRType);

        private static Expression<Func<ShAVRItem, bool>> isVCAddonSalesExp = (i) => i.VCAddOnSales;

        public static Func<ShAVRItem, bool> IsAddonSalesComp { get { return isAddonSalesExp.Compile(); } }
        public static Func<ShAVRItem, bool> IsVCAddonSalesComp { get { return isVCAddonSalesExp.Compile(); } }

        private static Expression<Func<ShAVRItem, bool>> hasLimitExp = (i) => i.Limit!=null && i.InLimit.HasValue;
        /// <summary>
        /// есть лимит и он учтен системой
        /// </summary>
        public static Func<ShAVRItem, bool> HasLimitComp { get { return hasLimitExp.Compile(); } }

        private static Expression<Func<ShAVRItem, bool>> outOfLimitExpr = (i) => (HasLimitComp(i)  && i.InLimit.HasValue && !i.InLimit.Value  );
         //<summary>
         //позиции имеющие превышенный лимит.
         //</summary>
        public static Func<ShAVRItem, bool> OutOfLimitComp { get { return outOfLimitExpr.Compile(); } }



        private static Expression<Func<ShAVRItem, bool>> inLimitExpr = (i) => (HasLimitComp(i) && i.InLimit.HasValue && i.InLimit.Value);
        //<summary>
        //позиции имеющие превышенный лимит.
        //</summary>
        public static Func<ShAVRItem, bool> InLimitComp { get { return inLimitExpr.Compile(); } }

        private static Expression<Func<ShAVRItem, bool>> isVCAddonSalesOrExceedExp = (i) => ((HasLimitComp(i)) && (OutOfLimitComp(i))) || IsVCAddonSalesComp(i);
        public static Func<ShAVRItem, bool> IsVCAddonSalesOrExceedComp { get { return isVCAddonSalesOrExceedExp.Compile(); } }

       // private static Expression<Func<ShAVRItem, bool>> notCalcLimitExpr = (i) => (HasLimitComp(i) && !i.LimitCounted);
        /// <summary>
        /// Используется для выборки позиций с нерасчитаными лимитами для их расчета.
        /// </summary>
       // public static Func<ShAVRItem, bool> NotCalcLimitComp { get { return notCalcLimitExpr.Compile(); } }


       // private static Expression<Func<ShAVRItem, bool>> CalcLimitExpr = (i) => (HasLimitComp(i) && i.LimitCounted);
        /// <summary>
        /// если есть лимит - то он должен быть учтен. иначе досвидос.
        /// </summary>
      //  public static Func<ShAVRItem, bool> CalcLimitComp { get { return CalcLimitExpr.Compile(); } }


        public static List<ShAVRItem> GetAVRItems(string avrId, Context context)
        {
            var shAVR = context.ShAVRs.Find(avrId);
            if (shAVR != null)
                return shAVR.Items.Where(IsVCAddonSalesOrExceedComp).ToList();
            else
                return new List<ShAVRItem>();
        }

    }
}
