using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses
{
    public static class NeedPrepriceCondition
    {
        public static bool Need(ShAVRs shAvr)
        {
            var avrItems = shAvr.Items;
            if (avrItems.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
            {
                var requests = shAvr.ShVCRequests.Where(r=>r.RequestSend.HasValue).ToList();
                if (requests == null || requests.Count == 0)
                    return true;
                if(requests.Any(VCRequestRepository.SuccessRequestComp))
                {
                    return false;
                }
                else
                {
                    if(requests.All(VCRequestRepository.CompleteRequestComp))
                    {
                        return true;
                    }
                    return false;
                }


            }
            else
            {
                return false;

            }
        }
    }
}
