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

            if (!AVRRepository.NeedReexpose(shAvr))
            {
                var isPriced = AVRRepository.GetAVRSATPor(shAvr.AVRId, context) != null;
                if (isPriced)
                    return true;
            }
            else
            {
                var requests = shAvr.ShVCRequests.Where(r => r.RequestSend.HasValue).ToList();
                var avrItems = shAvr.Items;
                if (AVRRepository.IsES(shAvr))
                {
                    if (avrItems.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
                    {
                        // если за рамками лимита или аос то должен быть саксесс вс реквест
                        if (requests.Any(VCRequestRepository.SuccessRequest))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // если там только позиции в рамках лимита, то можно пор отдать
                        // для еса в этом случае было уведомление в вымпел
                        // Но так же он должен быть опрайсован
                        
                        return !needPriceCondition.IsSatisfy(shAvr, context);
                    }
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
                            return true;
                        }
                    }

                }
            }
            return false;
        }
    }
}

