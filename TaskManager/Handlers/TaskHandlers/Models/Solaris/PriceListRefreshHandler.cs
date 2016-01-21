using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using DbModels.DomainModels.ShClone;
using DbModels.DataContext;
using System.Collections;
using CommonFunctions;

namespace TaskManager.Handlers.TaskHandlers.Models.Solaris
{
    public class PriceListRefreshHandler:ATaskHandler
    {
        public PriceListRefreshHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            using (Context context = new Context())
            {
                List<ShPriceListItem> insertItems = StaticHelpers.GetStoredProcDataFromContext<ShPriceListItem>(context, "PriceListItemsInsert", null);
                if (insertItems != null && insertItems.Count > 0)
                {
                    TaskParameters.TaskLogger.LogInfo(string.Format("Количество айтемов для добаления: {0}", insertItems.Count));
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(insertItems) });
                }
                else
                {
                    TaskParameters.TaskLogger.LogError("хранимка PriceListItemsInsert не вернула результатов ");
                }
                 List<ShPriceListItem> deleteItems = StaticHelpers.GetStoredProcDataFromContext<ShPriceListItem>(context, "PriceListItemsDelete", null);
                 if (deleteItems != null && deleteItems.Count > 0)
                 {
                     TaskParameters.TaskLogger.LogInfo(string.Format("Количество айтемов для удаления: {0}", deleteItems.Count));
                     TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(deleteItems) });
                 }
                 else
                 {
                     TaskParameters.TaskLogger.LogError("хранимка PriceListItemsDelete не вернула результатов ");
                 }
                     return true;
            }       
        }
    }
}
