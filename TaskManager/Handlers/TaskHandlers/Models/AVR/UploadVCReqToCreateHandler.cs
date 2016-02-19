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
    /// <summary>
    /// Хендлер грузит в сх, созданные в САТ ВК запросы.
    /// </summary>
    public class UploadVCReqToCreateHandler:ATaskHandler

    {
        public UploadVCReqToCreateHandler(TaskParameters taskParams):base(taskParams)
        {

        }

        public override bool Handle()
        {
            var notUploadedVCRequests = TaskParameters.Context.VCRequestsToCreate.Where(r => !r.UploadDate.HasValue).ToList();
            List<ShVCRequestImport> imports = new List<ShVCRequestImport>();
            List<ImportPathToAVR> avrPathsList = new List<ImportPathToAVR>();
            foreach (var req in notUploadedVCRequests)
            {
                var request = CreateVCRequest.Handle(req.VCRequestNumber, TaskParameters.Context);
                if (request != null)
                {
                    req.UploadDate = DateTime.Now;
                    imports.Add(request);
                    avrPathsList.Add(new ImportPathToAVR { AvrId = req.AVRId, Path = request.Attachment} );
                }
            }

            if(imports.Count>0)
            {

                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(imports) });
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(avrPathsList) });
            }
            return true;
        }


        private class ImportPathToAVR
        {
            public string AvrId { get; set; }
            public string Path { get; set; }
        }
    }
}
