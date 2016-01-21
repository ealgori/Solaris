using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Solaris
{
    public class SubcontractorSyncHandler:ATaskHandler
    {
        public SubcontractorSyncHandler(TaskParameters par):base(par)
        {

        }
        public override bool Handle()
        {

            #region AVR
            var importModels = new List<AVRImportModel>();
            var avrs = TaskParameters.Context.ShAVRs.Where(a =>
                (!string.IsNullOrEmpty(a.Subcontractor) && string.IsNullOrEmpty(a.SubcontractorRef))
                || string.IsNullOrEmpty(a.Subcontractor) && !string.IsNullOrEmpty(a.SubcontractorRef));
            foreach (var avr in avrs)
            {
                if (string.IsNullOrEmpty(avr.Subcontractor))
                {
                    var subc = TaskParameters.Context.SubContractors.FirstOrDefault(s => s.NameRef == avr.SubcontractorRef);
                    if (subc != null)
                    {
                        importModels.Add(new AVRImportModel() { AVR= avr.AVRId, Subcontractor = subc.ShName });
                    }
                }

                if (string.IsNullOrEmpty(avr.SubcontractorRef))
                {
                    var subc = TaskParameters.Context.SubContractors.FirstOrDefault(s => s.ShName == avr.Subcontractor);
                    if (subc != null)
                    {
                        importModels.Add(new AVRImportModel() { AVR = avr.AVRId, SubcontractorRef = subc.NameRef });
                    }
                }

            }
            #endregion
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams() { Objects = new System.Collections.ArrayList(importModels), ImportFileNearlyName= TaskParameters.DbTask.ImportFileName1  });

            return true;
        }

        class AVRImportModel
        {
            public string AVR { get; set; }
            public string Subcontractor { get; set; }
            public string SubcontractorRef { get; set; }
        }
    }
}
