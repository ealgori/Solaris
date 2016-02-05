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
        //public static bool Ready(ShAVRs shAvr)
        //{
        //    var items = shAvr.Items;
        //    if (!items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp) && items.Any(AVRItemRepository.InLimitComp))
        //    {

        //    }
        //    return false;
        //}
        /// <summary>
        /// Если приорите 1 и 2
        /// И нет уведломления
        /// 
        /// если Приоритет 3 и далее
        /// Есть чекнутый вс реквест и он не отправлен
        /// Нет некомплетед вс реквестов
        /// </summary>
        /// <param name="shAvr"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsSatisfy(ShAVRs shAvr, Context context)
        {

            var items = shAvr.Items;
            var requests = shAvr.ShVCRequests;
            // если это ЕС, то надо отправить уведомления в вымпел, если этого еще не сделано
            if (AVRRepository.IsES(shAvr))
            {
                // в этом случае реквесты создаются автоматом и сразу отправляются
                if (requests == null || requests.Count == 0)
                    return true;
            }
            else
            {
                // если это не ЕС, то должен быть подготовлен новый реквест, который пометили готовым к отправке, а также
                // не должно быть повисших реквестов

                if (requests.Any(VCRequestRepository.UnsendRequest))
                {
                    // из всех реквестов выбираем отправленные
                    var sendRequests = requests.Where(VCRequestRepository.SendRequest).ToList();
                    if (sendRequests.All(VCRequestRepository.CompleteRequest) && (!sendRequests.Any(VCRequestRepository.SuccessRequest)))
                    {
                        return true;
                    }
                    else
                    {
                        // если остались висящие реквесты, или по ним уже есть саксесс
                        return false;
                    }
                }
                
            }


            /// если позиции только в рамках лимита - то надо отправить уведомление - если этого еще не сделано
            //if (!items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp) && items.Any(AVRItemRepository.InLimitComp))
            //{
            //    /// реквест всего один, для уведомления

            //}
            //if (requests == null || requests.Count == 0)
            //    return true;
            //else
            //{

            //}

            return false;

        }
    }
}
