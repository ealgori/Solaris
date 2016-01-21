using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using System.Collections;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ConditionClasses;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class NeedPrepriceHandler:ATaskHandler
    {
        public NeedPrepriceHandler(TaskParameters taskParams):base(taskParams)
        {

        }
        public override bool Handle()
        {
            var importList = new List<AVRStatusImport>();
            // необходимо проанализировать авр, у которых нет поров и которые заморожены по всем правилам
            // у этих авров должны быть позиции за рамками лимитов или АОС
            var avrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.FirstStageBaseComp).ToList();
            foreach (var shAvr in avrs)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}",shAvr.AVRId, shAvr.NeedPreprice));
                var need = NeedPrepriceCondition.Need(shAvr);
                if(need != shAvr.NeedPreprice)
                {
                    shAvr.NeedPreprice = need;
                    importList.Add(new AVRStatusImport { AVRId= shAvr.AVRId, NeedPreprice = need });
                }
            }

            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importList) });
            return true;
        }
    }
}
