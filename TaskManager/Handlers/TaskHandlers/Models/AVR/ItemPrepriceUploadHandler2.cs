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
            //List<AVRPrepriceImportModel> avrModels = new List<AVRPrepriceImportModel>();
            List<ShVCRequestImport> shVCRequestImport = new List<ShVCRequestImport>();
            foreach (var avrGroup in groupedByAVR)
            {
                var lastRevision = avrGroup.GroupBy(g => g.PrepriceDate ).OrderByDescending(g => g.Key).FirstOrDefault();
                if (lastRevision != null && lastRevision.Count() > 0 && lastRevision.Any(i => !i.Uploaded))
                {
                    foreach (var item in lastRevision)
                    {
                        var itemModel = new ItemPrepriceImportModel();

                        itemModel.ItemId = item.AVRItemId.Value;
                        
                        if (!item.IsCustomItem)
                        {
                            if (item.Item != null)
                            {
                                itemModel.PriceListRevisionItemId = item.Item.Id;
                                itemModel.VCDescription = item.Item.Name;
                                if (item.VCUseCoeff)
                                {
                                    itemModel.VCPrice = item.Item.Price * item.VCCoeff;
                                }
                                else
                                {
                                    itemModel.VCPrice = item.Item.Price;
                                }
                                itemModel.VCUseCoeff = item.VCUseCoeff;

                            }
                        }
                        else
                        {
                            itemModel.VCPrice = item.vcPrice;
                            itemModel.VCDescription = item.VCDescription;
                            itemModel.VCCustomItem = true;

                        }
                        itemModel.VCQuantity = item.vcQuantity;

                        models.Add(itemModel);

                      
                    }
                    //avrModels.Add(new AVRPrepriceImportModel() { AVRId = avrGroup.Key, Prepriced = DateTime.Now });
                    var shVCRequest = SaveMailToAdmin.Handle(avrGroup.Key, TaskParameters.Context);
                    // если удалось создать заказ, то проставить тру, что позиции выгрузились. иначе попробуем на след итерции
                    if (shVCRequest != null)
                    {
                        shVCRequestImport.Add(new ShVCRequestImport() { ShAVRs = avrGroup.Key, Id = shVCRequest.Id, Attachment = shVCRequest.Attachment });
                        foreach (var item in lastRevision)
                        {
                              item.Uploaded = true;
                        }
                    }
                   
                }
            }
            if(models.Count>0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(models) });
            //TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName2, Objects = new ArrayList(avrModels) });
            if (shVCRequestImport.Count>0)
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName3, Objects = new ArrayList(shVCRequestImport) });

            return true;
        }
    }
}
