using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses
{
    public static class ReadyToPORCondition
    {
        public static  bool Ready(ShAVRs avr)
        {
            var avrItems = avr.Items;
            // нет лимитов и аос - он готов
            if (!avrItems.Any(AVRItemRepository.HasLimitComp) && (!avrItems.Any(AVRItemRepository.IsVCAddonSalesComp)))
            {
                return true;
            }
            else
            {
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
        }
    }
}
