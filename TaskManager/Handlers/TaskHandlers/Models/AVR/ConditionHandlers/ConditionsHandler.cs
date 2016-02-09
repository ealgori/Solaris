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

            var changesStatuses = new List<StatusImportModel>();
            #region NeddPriceCondition
            var needPriceCondition = new NeedPriceCondition();
            {
                
                var needPriceStatus = Statuses.NeedPrice;
                var needPriceList = Apply(freezedAvrs, needPriceCondition, needPriceStatus);
                if (needPriceList != null && needPriceList.Count > 0)
                    changesStatuses.AddRange(needPriceList.Select(s=>new StatusImportModel() { AvrId= s.AVRId, Status = s.Status.ToString() }));
            }
            #endregion
            #region NeedVCpriceCondition
            {
                var needVCPriceCondition = new NeedVCPriceCondition();
                var needVCPriceStatus = Statuses.NeedVCPrice;

                var needVCPriceList = Apply(reexposeAVRs, needVCPriceCondition, needVCPriceStatus);
                if (needVCPriceList != null && needVCPriceList.Count > 0)
                {
                    foreach (var shAvr in needVCPriceList)
                    {
                        var reexposeTotal = shAvr.Items.Where(AVRItemRepository.IsVCAddonSalesOrExceedComp).Sum(s=>s.Price*s.Quantity);
                        shAvr.TotalVCReexpose = reexposeTotal;
                    }
                    changesStatuses.AddRange(needVCPriceList.Select(s => new StatusImportModel() { AvrId = s.AVRId, Status = s.Status.ToString() }));
                }

               
            }
            #endregion
            #region needMus
            {
                var needMUSPriceCondition = new NeedMUSCondition();
                var needMusStatus = Statuses.NeedMus;
                var needMusList = Apply(reexposeAVRs, needMUSPriceCondition, needMusStatus);
                if (needMusList != null && needMusList.Count > 0)
                    changesStatuses.AddRange(needMusList.Select(s => new StatusImportModel() { AvrId = s.AVRId, Status = s.Status.ToString() }));
            }
         
            #endregion
            #region PorAccesible
            {
                var porAccesileCondition = new PORAccessibleCondition(needPriceCondition);
                //var porAccesibleStatus = Statuses.PorReady;
                //var porAccesibleList = Apply(freezedAvrs, porAccesileCondition, porAccesibleStatus);
                //if (porAccesibleList != null && porAccesibleList.Count > 0)
                //    changesStatuses.AddRange(porAccesibleList.Select(s => new StatusImportModel() { AvrId = s.AVRId, Status = s.Status.ToString() }));
              
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                foreach (var shAvr in freezedAvrs)
                {
                    StatusImportModel importItem = null;
                    bool changed = false;
                    if (porAccesileCondition.IsSatisfy(shAvr, TaskParameters.Context))
                    {
                        if (!shAvr.PorAccesible)
                        {
                            shAvr.PorAccesible = true;
                            changed = true;

                        }
                    }
                    else
                    {
                        if (shAvr.PorAccesible)
                        {
                            shAvr.PorAccesible = false;
                            changed = true;

                        }
                    }
                    if(changed)
                        changesStatuses.Add(new StatusImportModel() { AvrId = shAvr.AVRId, PorAccesible = shAvr.PorAccesible });
                }
                watch.Stop();
                TaskParameters.TaskLogger.LogInfo(string.Format("{1}:{0}", watch.Elapsed.TotalSeconds, "porAccesible"));

            }
            #endregion
            #region ReadyForRequest
            {
                var readyRequestCond = new ReadyToRequestCondition();
                var readyRequestStatus = Statuses.ReadyForRequest;
                var readyToRequestList = Apply(reexposeAVRs, readyRequestCond, readyRequestStatus);
                if (readyToRequestList != null && readyToRequestList.Count > 0)
                    changesStatuses.AddRange(readyToRequestList.Select(s => new StatusImportModel() { AvrId = s.AVRId, Status = s.Status.ToString() }));
            }
          
            #endregion

            //TODO: А что если в сх ввести поле статус, и прогружать в него статус по этим коднишнам.
            // а так же добавить проверку их пересечения, для проверки корректности кондишнов
            var doubles = changesStatuses.GroupBy(g => g.AvrId).Where(g => g.Count() > 1).Select(s=>s.Select(sa=>sa)).ToList();
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

        public class StatusImportModel
        {
            public string AvrId { get; set; }
            public string Status { get; set; }
            public bool PorAccesible { get; set; }
        }
    }
}
