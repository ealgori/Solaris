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
    public static class PORAccessibleCondition:IAVRCondition
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="avr"></param>
        /// <returns></returns>
        public static  bool Ready(ShAVRs avr)
        {
            var avrItems = avr.Items;
            
            
            
            // нет лимитов и аос - он готов
            //if (!avrItems.Any(AVRItemRepository.HasLimitComp) && (!avrItems.Any(AVRItemRepository.IsVCAddonSalesComp)))
            //{
            //    return true;
            //}
            //else
            //{
                //var requests = avr.ShVCRequests.Where(r=>r.RequestSend.HasValue).ToList();
                //if (requests == null || requests.Count == 0)
                //{
                //    return false;
                //}
                //if (requests.Any(VCRequestRepository.SuccessRequestComp))
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
                // теперь все готовы к 
                return true;

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
                var requests = shAvr.ShVCRequests.Where(r=>r.RequestSend.HasValue).ToList();
                var avrItems = shAvr.Items;
                if (AVRRepository.IsES(shAvr))
                {
                    if (avrItems.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
                    {
                        // если за рамками лимита или аос то должен быть саксесс вс реквест
                        if(requests.Any(VCRequestRepository.SuccessRequest))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // если там только позиции в рамках лимита, то можно пор отдать
                        // для еса в этом случае было уведомление в вымпел
                        return true;
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
}
