using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.TOH
{
    public class TOItemOtchetPredostHandler : ATaskHandler
    {
        public TOItemOtchetPredostHandler(TaskParameters param)
            : base(param)
        {

        }
        public override bool Handle()
        {
            var toItems = TaskParameters.Context.ShTOItems.Where(i =>
                i.OtchetPredostavlenVCfact == null
                && !string.IsNullOrEmpty(i.LinkToReportinEridoc)
                );
            var impModels = new List<ImportMModel>();
            foreach (var item in toItems)
            {
                var model = new ImportMModel();
                model.TOItemId = item.TOItem;
                model.OtchetPredostavlenVCfact = DateTime.Now;
                impModels.Add(model);

            }

            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(impModels) });

            return true;
        }

        private class ImportMModel
        {
            public string TOItemId { get; set; }
            public DateTime? OtchetPredostavlenVCfact { get; set; }
        }
    }
}
