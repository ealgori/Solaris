using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbModels.DataContext;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext.Repositories;
using DbModels.DataContext.AVRConditions;
using DbModels.Service;

namespace DbModels.AVRConditions
{
    public class NeedMUSCondition : IAVRCondition
    {
        /// <summary>
        /// если не ЕС
        /// Если нет вих запросов или последний реджектнут
        /// Наличие Саксеед ВКРеквеста
        /// </summary>
        /// <param name="shAvr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {
            if(!AVRRepository.IsES(shAvr))
            {
                //TODO: если забит нетворк полученый с муса, то ничего этого не надо.
                var wihRequests = context.ShWIHRequests.Where(r => r.AVRId == shAvr.AVRId).ToList();
                var canBeSend = WIHService.RequestCanBeSended(wihRequests, WIHInteract.Constants.InternalMailTypeAVRMUS);
                if(canBeSend)
                {
                    if (shAvr.ShVCRequests == null) return false;
                    var requests = shAvr.ShVCRequests.Where(r=>r.SendRequest);
                    if(requests.Any(VCRequestRepository.SuccessRequest))
                    {
                        return true;
                    }
                    
                }
            }
            else
            {
                // если это ес, то мус уйдет после подписанного заказа в вымпеле
                if (shAvr.ShVCRequests.Any(VCRequestRepository.SuccessRequest))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
