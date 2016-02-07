using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class AVRUnfreezeHandler:ATaskHandler
    {
        public AVRUnfreezeHandler(TaskParameters taskParams):base(taskParams)
        {

        }
        public override bool Handle()
        {
            var importList = new List<AVRUnfreeezeImportModel>();
            var requests = TaskParameters.Context.ShVCRequests.Where(VCRequestRepository.UnSuccessRequest).Where(r => !string.IsNullOrEmpty(r.ShAVRs.RukFiliala)).ToList();
            foreach (var request in requests)
            {
                importList.Add(new AVRUnfreeezeImportModel {  AVRId = request.ShAVRs.AVRId});
            }
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importList) });
            return true;
          
        }
    }
}
