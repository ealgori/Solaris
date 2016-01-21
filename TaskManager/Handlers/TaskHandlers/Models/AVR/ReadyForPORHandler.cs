using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class ReadyForPORHandler:ATaskHandler
    {
        public ReadyForPORHandler(TaskParameters taskParams)
            : base(taskParams)
        {

        }

        public override bool Handle()
        {
            // К порам готовы те, кто правильно заморожен, у кого нет лимитов и АОС
            // а также если есть лимиты или аос, имеются завершенные запросы

            var importList = new List<AVRStatusImport>();
            // в дальшнейшем можно еще о
            var avrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.BaseComp).Where(a=>string.IsNullOrEmpty(a.PurchaseOrderNumber)).ToList();
            foreach (var avr in avrs)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", avr.AVRId, avr.ReadyForPOR));
                var ready = ReadyToPORCondition.Ready(avr);
                if(ready!= avr.ReadyForPOR)
                {
                    avr.ReadyForPOR = ready;
                    importList.Add(new AVRStatusImport {  AVRId =  avr.AVRId, ReadyForPOR = ready});
                }
            }
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importList) });
            
            return true;
        }


    }
}
