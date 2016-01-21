using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.TOH
{
    public class TORefresh:ATaskHandler
    {
          public TORefresh(TaskParameters taskParameters) : base(taskParameters) { }


          public override bool Handle()
          {
              var TOItemsToDelete = TaskParameters.Context.ShTOItems.Where(t => string.IsNullOrEmpty(t.TOId)).ToList();
              if (TOItemsToDelete.Count() > 0)
              {
                  TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(TOItemsToDelete.Select(t => new { t.TOItem }).ToList()) });
              }
              return true;
          }
    }
}
