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
    public class ItemPrepriceUploadHandler : ATaskHandler
    {
        public ItemPrepriceUploadHandler(TaskParameters taskParameters)
            : base(taskParameters)
        {

        }
        public override bool Handle()
        {
            var groupedByAVR = TaskParameters.Context.SATPrepricedItems.GroupBy(i => i.AVRId).Where(g => g.Any(i => !i.Uploaded)).ToList();
            List<ItemPrepriceImportModel> models = new List<ItemPrepriceImportModel>();
            List<AVRPrepriceImportModel> avrModels = new List<AVRPrepriceImportModel>();
            foreach (var avrGroup in groupedByAVR)
            {
                var lastRevision = avrGroup.GroupBy(g => g.PrepriceDate).OrderByDescending(g => g.Key).FirstOrDefault();
                if (lastRevision != null && lastRevision.Count() > 0 && lastRevision.Any(i => !i.Uploaded))
                {
                    foreach (var item in lastRevision)
                    {
                        var itemModel = new ItemPrepriceImportModel();

                        itemModel.ItemId = item.AVRItemId.Value;
                        if (item.Item != null)
                        {
                            itemModel.PriceListRevisionItemId = item.Item.Id;
                            itemModel.VCDescription = item.Item.Name;
                            itemModel.VCPrice = item.Item.Price;

                        }
                        itemModel.VCQuantity = item.vcQuantity;

                        models.Add(itemModel);

                        item.Uploaded = true;
                    }
                    avrModels.Add(new AVRPrepriceImportModel() { AVRId = avrGroup.Key, Prepriced = DateTime.Now });
                }
            }
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(models) });
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(avrModels) });
            
            return true;
        }
    }
}
