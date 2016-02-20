using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;
using DbModels.DataContext.Repositories;
using TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses;
using System.Collections;
using DbModels.DomainModels.ShClone;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class SaveMailToAdmin2:ATaskHandler
    {
        public SaveMailToAdmin2(TaskParameters taskParams):base(taskParams)
        {

        }




        public override bool Handle()
        {
            // // Это уведомления в вымпел об использовании лимитов. Вообще они должны отправляться автоматом
            // находим реквесты которые в базовом состоянии и с лимитами в рамках лимита
            // которым не требуется предопрайсовка
            //TODO: подготовим уведмление в вк об использовании лимитов
            List<ShVCRequestImport> shVCRequestImport = new List<ShVCRequestImport>();
            List<UploadVCReqToCreateHandler.ImportPathToAVR> avrPathsList = new List<UploadVCReqToCreateHandler.ImportPathToAVR>();

            var avrs = TaskParameters.Context.ShAVRs
                .Where(a=>a.PorAccesible)
                //.Where(a => a.Items.Any(AVRItemRepository.InLimitComp))
                //.Where(a => !a.Items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp))
                .ToList();

            foreach (var shAVR in avrs)
            {
                if (shAVR.Items.Any(AVRItemRepository.InLimitComp) && (!shAVR.Items.Any(AVRItemRepository.IsVCAddonSalesOrExceedComp)))
                    {
                    var request = TaskParameters.Context.ShVCRequests.Where(r => r.ShAVRs.AVRId == shAVR.AVRId).OrderByDescending(r => r.CreateDate).FirstOrDefault();
                    if (request == null)
                    {
                        var shVCRequest = CreateVCRequest.Handle(shAVR.AVRId, TaskParameters.Context);
                        if (shVCRequest != null)
                        {
                            shVCRequest.SendRequest = true;
                            shVCRequestImport.Add(shVCRequest);
                            avrPathsList.Add(new UploadVCReqToCreateHandler.ImportPathToAVR() { AvrId = shAVR.AVRId, Path = shVCRequest.Attachment });
                        }
                    }
                }

            }
            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(shVCRequestImport) });


            return true;
        }
    }
}
