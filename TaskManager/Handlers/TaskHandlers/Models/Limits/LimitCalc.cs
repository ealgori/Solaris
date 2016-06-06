using DbModels.DataContext.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class LimitCalcHandler : ATaskHandler
    {
        public LimitCalcHandler(TaskParameters param)
            : base(param)
        {

        }
        public override bool Handle()
        {
            var checkItemImport = new List<ItemCheckImport>();
            var limitExecUpdate = new List<LimitExecImport>();
            

            var limits = TaskParameters.Context.ShLimits.ToList();
            foreach (var limit  in limits)
            {
                var calcItems = limit.ShAVRitems.Where(i=>i.InLimit.HasValue).Where(a=>a.AVRS.InCalculations).ToList();
                // позиции, для которых еще не определено, в рамках лимита они или нет
                var newItems = limit.ShAVRitems.Where(i => !i.InLimit.HasValue).Where(a => a.AVRS.InCalculations).ToList();
                var lastValue = limit.Executed;

                // больше не требуется
                //limit.Executed = limit.InitValue;
                //if (!limit.Executed.HasValue)
                //    limit.Executed = 0;
                //foreach (var item in calcItems)
                //{
                //    limit.Executed += item.Quantity;
                //}
                foreach (var item in newItems)
                {
                    //limit.Executed += item.Quantity;
                    bool inLimit = false;
                    if(limit.Executed<=limit.SettedLimit)
                    {
                        inLimit = true;
                    }
                    checkItemImport.Add(new ItemCheckImport() { ItemId = item.AVRItemId, InLimit = inLimit, NeedVCReexpose = !inLimit  });
                }
                //if(lastValue!=limit.Executed)
                //    limitExecUpdate.Add(new LimitExecImport() {  LimitCode = limit.LimitCode, Executed = limit.Executed});

            }

       


          
           
         
            TaskParameters.Context.SaveChanges();


            if(checkItemImport.Count>0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(checkItemImport) });
            //if(limitExecUpdate.Count>0)
            //    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(limitExecUpdate) });
            
            
            return true;
        
        
        

        
        }
    }
}
