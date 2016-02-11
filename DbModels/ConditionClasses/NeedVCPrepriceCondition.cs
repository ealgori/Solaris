using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DataContext;
using DbModels.DataContext.AVRConditions;

namespace DbModels.AVRConditions
{
    public  class NeedVCPriceCondition:IAVRCondition
    {

        /// <summary>
        /// 
        /// Если тип авр требует перевыставления
        /// Если нет саксеед запросов
        /// Если все предыдущие запросы завершены
        /// Если опрайсовано или Эрикссон
        /// </summary>
        /// <param name="shAvr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {
            if(AVRRepository.NeedReexpose(shAvr))
            {
                var avrItems = shAvr.Items;
                if (avrItems.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
                {
                    var requests = shAvr.ShVCRequests.Where(r => r.RequestSend.HasValue).ToList();
                    if (requests == null || requests.Count == 0)
                    {
                        if (AVRRepository.HasEricssonSubcontractor(shAvr))
                            return true;
                        else
                            return (AVRRepository.GetAVRSATPor(shAvr.AVRId,context) != null);
                    }
                    if (requests.Any(VCRequestRepository.SuccessRequest))
                    {
                        return false;
                    }
                    else
                    {
                        if (requests.All(VCRequestRepository.CompleteRequest))
                        {
                            if (AVRRepository.HasEricssonSubcontractor(shAvr))
                                return true;
                            else
                                return (AVRRepository.GetAVRSATPor(shAvr.AVRId, context) != null);
                        }
                    }
                }
                
            }
            return false;
        }
    }
}
