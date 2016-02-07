using DbModels.DataContext.Repositories;
using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionHandlers
{
    public class ConditionsHandler:ATaskHandler
    {
        public ConditionsHandler(TaskParameters taskParams):base(taskParams)
        {

        }

        public override bool Handle()
        {
            var freezedAvrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.Base)
                //.Where(a => a.AVRId == "205806")
                .ToList();
            var inCalcAvrs = freezedAvrs.Where(AVRRepository.InCalculations).ToList();
            var reexposeAVRs = inCalcAvrs.Where(AVRRepository.NeedReexpose).ToList();

            var changesStatuses = new List<ShAVRs>();
            #region NeddPriceCondition
            var needPriceCondition = new NeedPriceCondition();
            {
                
                var needPriceStatus = Statuses.NeedPrice;
                var needPriceList = Apply(freezedAvrs, needPriceCondition, needPriceStatus);
                if (needPriceList != null && needPriceList.Count > 0)
                    changesStatuses.AddRange(needPriceList);
            }
            //var priceWatch = new System.Diagnostics.Stopwatch();

            //priceWatch.Start();
            //foreach (var shAvr in freezedAvrs)
            //{
            //    if (needPriceCondition.IsSatisfy(shAvr, TaskParameters.Context))
            //    {
            //        if (shAvr.Status != needPrice)
            //        {
            //            shAvr.Status = Statuses.NeedPrice;
            //            changesStatuses.Add(shAvr);
            //        }
            //    }
            //}
            //priceWatch.Stop();
            //TaskParameters.TaskLogger.LogInfo(string.Format("price:{0}",priceWatch.Elapsed.TotalSeconds));
            #endregion
            #region NeedVCpriceCondition
            {
                var needVCPriceCondition = new NeedVCPriceCondition();
                var needVCPriceStatus = Statuses.NeedVCPrice;

                var needVCPriceList = Apply(reexposeAVRs, needVCPriceCondition, needVCPriceStatus);
                if (needVCPriceList != null && needVCPriceList.Count > 0)
                    changesStatuses.AddRange(needVCPriceList);
            }
            //var vcpriceWatch = new System.Diagnostics.Stopwatch();

            //vcpriceWatch.Start();
            //foreach (var shAvr in reexposeAVRs)
            //{
            //    if ((needVCPriceCondition.IsSatisfy(shAvr, TaskParameters.Context)))
            //    {
            //        if (shAvr.Status != needVCPriceStatus)
            //        {
            //            shAvr.Status = Statuses.NeedVCPrice;
            //            changesStatuses.Add(shAvr);
            //        }
            //    }
            //}
            //vcpriceWatch.Stop();
            //TaskParameters.TaskLogger.LogInfo(string.Format("vcPrice:{0}", vcpriceWatch.Elapsed.TotalSeconds));
            #endregion
            #region needMus
            {
                var needMUSPriceCondition = new NeedMUSCondition();
                var needMusStatus = Statuses.NeedMus;
                var needMusList = Apply(reexposeAVRs, needMUSPriceCondition, needMusStatus);
                if (needMusList != null && needMusList.Count > 0)
                    changesStatuses.AddRange(needMusList);
            }
            //var musWatch = new System.Diagnostics.Stopwatch();

            //musWatch.Start();
            //foreach (var shAvr in reexposeAVRs)
            //{
            //    if (needMUSPriceCondition.IsSatisfy(shAvr, TaskParameters.Context))
            //    {
            //        if(shAvr.)
            //        shAvr.Status = Statuses.NeedMus;
            //        changesStatuses.Add(shAvr);
            //    }
            //}
            //musWatch.Stop();
            //TaskParameters.TaskLogger.LogInfo(string.Format("mus:{0}", musWatch.Elapsed.TotalSeconds));
            #endregion
            #region PorAccesible
            {
                var porAccesileCondition = new PORAccessibleCondition(needPriceCondition);
                var porAccesibleStatus = Statuses.PorReady;
                var porAccesibleList = Apply(freezedAvrs, porAccesileCondition, porAccesibleStatus);
                if (porAccesibleList != null && porAccesibleList.Count > 0)
                    changesStatuses.AddRange(porAccesibleList);
            }
            //var porWatch = new System.Diagnostics.Stopwatch();
            //porWatch.Start();
            //foreach (var shAvr in freezedAvrs)
            //{
            //    if ((porAccesile.IsSatisfy(shAvr, TaskParameters.Context)))
            //    {
            //        shAvr.Status = Statuses.PorReady;
            //        changesStatuses.Add(shAvr);
            //    }
            //}
            //porWatch.Stop();
            //TaskParameters.TaskLogger.LogInfo(string.Format("por:{0}", porWatch.Elapsed.TotalSeconds));
            #endregion
            #region ReadyForRequest
            {
                var readyRequestCond = new ReadyToRequestCondition();
                var readyRequestStatus = Statuses.ReadyForRequest;
                var readyToRequestList = Apply(reexposeAVRs, readyRequestCond, readyRequestStatus);
                if (readyToRequestList != null && readyToRequestList.Count > 0)
                    changesStatuses.AddRange(readyToRequestList);
            }
            //var readyWatch = new System.Diagnostics.Stopwatch();
            //readyWatch.Start();
            //foreach (var shAvr in reexposeAVRs)
            //{
            //    if ((readyRequestCond.IsSatisfy(shAvr, TaskParameters.Context)))
            //    {
            //        shAvr.Status = Statuses.ReadyForRequest;
            //        changesStatuses.Add(shAvr);
            //    }
            //}
            //readyWatch.Stop();
            //TaskParameters.TaskLogger.LogInfo(string.Format("por:{0}", readyWatch.Elapsed.TotalSeconds));
            #endregion

            //TODO: А что если в сх ввести поле статус, и прогружать в него статус по этим коднишнам.
            // а так же добавить проверку их пересечения, для проверки корректности кондишнов
            var doubles = changesStatuses.GroupBy(g => g.AVRId).Where(g => g.Count() > 1).ToList();
            return true;

        }


        private List<ShAVRs> Apply(List<ShAVRs> shAvrs, IAVRCondition condition, Statuses status)
        {

            var changedList = new List<ShAVRs>();
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            foreach (var shAvr in shAvrs)
            {
                if (condition.IsSatisfy(shAvr, TaskParameters.Context))
                {
                    if (shAvr.Status != status)
                    {
                        shAvr.Status = status;
                        changedList.Add(shAvr);
                    }
                }
            }
            watch.Stop();
            TaskParameters.TaskLogger.LogInfo(string.Format("{1}:{0}", watch.Elapsed.TotalSeconds, status.ToString()));
            return changedList;
        }
    }
}
