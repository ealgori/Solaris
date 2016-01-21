using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.TOH
{
   

    public class TOTotalAmmountUpdate : ATaskHandler
    {
        public TOTotalAmmountUpdate(TaskParameters taskParameters) : base(taskParameters) { }


        public override bool Handle()
        {
            var shToes = TaskParameters.Context.ShTOes.AsNoTracking();
            var shTOItems = TaskParameters.Context.ShTOItems.AsNoTracking();
            var importModels = new List<ImportModel>();
            foreach (var to in shToes)
            {
                var toItems = shTOItems.Where(t => t.TOId == to.TO && t.ExcludeWork.HasValue&&!t.ExcludeWork.Value);
                var planObektov = toItems.Count();
                var itemsTotalAmmountNew = toItems.Sum(t => t.PriceFromPL * t.Quantity).FinanceRound();
                itemsTotalAmmountNew = itemsTotalAmmountNew != 0 ? itemsTotalAmmountNew :
                        to.TOTotalAmmount != null ? to.TOTotalAmmount.Value : 0;
                if(to.PlanObektov!=planObektov|| to.TOTotalAmountNew!=itemsTotalAmmountNew)
                {
                    importModels.Add(new ImportModel() {
                    ShTO=to.TO,
                    PlanObektov=planObektov,
                    TOTotalAmountNew = itemsTotalAmmountNew
                    });
                }

            }
            if (importModels.Count() > 0)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });
            }
            return true;
        }

        class ImportModel
        {
            public string ShTO { get; set; }
            public decimal PlanObektov { get; set; }
            public decimal TOTotalAmountNew {get;set; }
        }
    }
}
