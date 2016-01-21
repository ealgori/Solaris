using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses
{
    public static class ReadyToRequestCondition
    {
        public static bool Ready(ShAVRs shAvr)
        {
            var items = shAvr.Items;
            if (!items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp) && items.Any(AVRItemRepository.InLimitComp))
            {
                var requests = shAvr.ShVCRequests;
                if (requests == null || requests.Count == 0)
                    return true;
                else
                {
                    if (requests.All(VCRequestRepository.CompleteRequestComp) && (!requests.Any(VCRequestRepository.SuccessRequestComp)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
