using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext;
using DbModels.DataContext.Repositories;
using DbModels.DataContext.AVRConditions;

namespace DbModels.AVRConditions
{
    public class NeedPriceCondition : IAVRCondition
    {
        /// <summary>
        ///  если не эрикссон и если еще не опрайсован().
        /// в остальных случаях требуется опрайсова
        /// + базовый
        /// </summary>
        /// <param name="shAvr"></param>
        /// <param name="context"></param>
        /// <returns></returns>

        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {
            if(!AVRRepository.HasEricssonSubcontractor(shAvr))
            {
                var isPriced = AVRRepository.GetAVRSATPor(shAvr.AVRId,context)!=null;
                if (!isPriced)
                    return true; 
            }
            return false;

        }
    }
}
