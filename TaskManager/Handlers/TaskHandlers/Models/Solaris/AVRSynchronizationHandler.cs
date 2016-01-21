using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;
using DbModels.DataContext;
using DbModels.DomainModels.Solaris.Pors;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.Solaris
{
    public class AVRSynchronizationHandler:ATaskHandler
    {

        public AVRSynchronizationHandler(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {

            using (Context context = new Context())
            {
                var avrPors =context.AVRPORs.Where(avp=>avp.UploadedToSH==null);
                if (avrPors.Count()>0)
                {
                    TaskParameters.TaskLogger.LogInfo(string.Format("Количество аврпоров для прогрузки: {0}", avrPors.Count()));
                    var date = DateTime.Now;
                    var formatedDate = date.ToString("dd-MM-yyyy HH:mm:ss");
                    foreach (var avr in avrPors)
                    {
                        avr.UploadedToSH = date;
                    }
                    TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(avrPors.Select(av => new { AVRId = av.AVRId, uploaded = formatedDate }).ToList()) });
                }
                else
                {
                    TaskParameters.TaskLogger.LogError("Новых авропоров нет.");
                }
                context.SaveChanges();
                return true;
            }       
        }
    }
}
