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
    public class ReadyToRequestCondition : IAVRCondition
    {

        /// <summary>
        /// Автоматические реквесты. только при отсутствии позиций за рамками лимита и аос
        /// </summary>
        /// <param name="shAvr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {


            var items = shAvr.Items;
            var requests = shAvr.ShVCRequests;
            if (shAvr.Priority.HasValue)
            {
                // если все только в рамках лимита
                if (!items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp) && items.Any(AVRItemRepository.InLimitComp))
                {

                    //  если реквестов не было -то сразу отправляем
                    if (requests == null || requests.Count == 0)
                        return true;
                    // если высокий приоритет - то сразу отправляем уведомление. реджекта быть не может
                    if (!AVRRepository.IsES(shAvr))
                    {
                        // если низкий приоритет - то запрос с аппрувом. соотв могут быть реджекты.

                        if (requests.All(VCRequestRepository.CompleteRequest)&&!requests.Any(VCRequestRepository.SuccessRequest))
                        {
                           
                                return true;
                           
                        }
                    }

                }
            }
            return false;





            // если это ЕС, то надо отправить уведомления в вымпел, если этого еще не сделано




            return false;

        }
    }
}
