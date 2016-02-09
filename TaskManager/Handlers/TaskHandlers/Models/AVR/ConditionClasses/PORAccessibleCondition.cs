using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DataContext;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses
{
    public class PORAccessibleCondition : IAVRCondition
    {
        private IAVRCondition needPriceCondition;
        public PORAccessibleCondition(NeedPriceCondition needPriceCondition)
        {
            this.needPriceCondition = needPriceCondition;
        }

        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {

            var isPriced = AVRRepository.GetAVRSATPor(shAvr.AVRId, context) != null;
            if (!AVRRepository.NeedReexpose(shAvr))
            {
               
                    return isPriced;
            }
            else
            {
                var requests = shAvr.ShVCRequests.Where(r => r.RequestSend.HasValue).ToList();
                var avrItems = shAvr.Items;
                if (AVRRepository.IsES(shAvr))
                {
                      // если это ес, то пор доступен сразу, тк. там будет ес нетворк  
                        return !needPriceCondition.IsSatisfy(shAvr, context);
                    
                }
                else
                {
                    // должен был быть реквест в вк в любом случае
                    if (requests.Any(VCRequestRepository.SuccessRequest))
                    {
                        if (avrItems.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
                        {
                            // должен был быть мус
                            //TODO: т.е. нетворк от муса
                        }
                        else
                        {
                            // нетворк стандартный можно отдать пор
                            return isPriced;
                        }
                    }

                }
            }
            return false;
        }
    }
}

