using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class ReadyForRequestHandler:ATaskHandler
    {
        public ReadyForRequestHandler(TaskParameters taskParams):base(taskParams)
        {

        }
        public override bool Handle()
        {
            // для автоматической отправки
            // запросы можно отправлять тем, кто правльно заморожен, у кого нет пора и есть ТОЛЬКО лимиты в рамках лимита
            // так же у него либо нет запросов, либо все запросы комплитед, но не саксесс
            var importList = new List<AVRStatusImport>();
            var avrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.BaseComp)
                .Where(a => string.IsNullOrEmpty(a.PurchaseOrderNumber))
                .ToList();
            foreach (var shAvr in avrs)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0}-{1}",shAvr.AVRId, shAvr.ReadyForRequest));
                var ready = ReadyToRequestCondition.Ready(shAvr);
                if(ready!=shAvr.ReadyForRequest)
                {
                    shAvr.ReadyForRequest = ready;
                    importList.Add(new AVRStatusImport { AVRId = shAvr.AVRId, ReadyForRequest = ready});
                }


            }

            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importList) });
            return true;
        }
    }
}
