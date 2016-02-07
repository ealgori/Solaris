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
    public class SaveMailToAdmin2:ATaskHandler
    {
        public SaveMailToAdmin2(TaskParameters taskParams):base(taskParams)
        {

        }




        public override bool Handle()
        {
            // находим реквесты которые в базовом состоянии и с лимитами в рамках лимита
            // которым не требуется предопрайсовка
            //TODO: пользуем существующий функционал
            //List<ShVCRequestImport> shVCRequestImport = new List<ShVCRequestImport>();

            //var avrs = TaskParameters.Context.ShAVRs
            //    .Where(AVRRepository.Base)
            //    .Where(a => a.NeedPreprice.HasValue && !a.NeedPreprice.Value)
            //    .Where(a => a.Items.Any(AVRItemRepository.InLimitComp)).ToList();

            //foreach (var shAVR in avrs)
            //{
            //    var request = TaskParameters.Context.ShVCRequests.Where(r => r.ShAVRs.AVRId == shAVR.AVRId).OrderByDescending(r=>r.CreateDate).FirstOrDefault();
            //    if (request == null
            //        // реквест должен быть не саксес
            //        )
            //    {
            //        var shVCRequest = SaveMailToAdmin.Handle(shAVR.AVRId, TaskParameters.Context);
            //        if (shVCRequest != null)
            //        {
            //            shVCRequest.SendRequest = true;
            //            shVCRequestImport.Add(shVCRequest);
            //        }
            //    }

            //}
            //TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(shVCRequestImport) });


            return true;
        }
    }
}
