using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailProcessing;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.WIH
{
    public class WIHTrashCleaner : ATaskHandler
    {
        public WIHTrashCleaner(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            List<string> trashMasks = new List<string> { "Fixed", "received", "failed" };
            RedemptionMailProcessor redemtion = new RedemptionMailProcessor("SOLARIS");
            {
                var trashMails = redemtion.GetMails(trashMasks);
                TaskParameters.TaskLogger.LogInfo(@"Под определение ""мусор"" попало " + trashMails.Count + " писем");
                foreach (var mail in trashMails)
                {
                    try
                    {
                        redemtion.MoveToTrash(mail.ConversationId);
                    }
                    catch (Exception exc)
                    {
                        TaskParameters.TaskLogger.LogError(string.Format("Ошибка при перемещении письма '{0}' : {1} ", mail.Subject, exc.Message));
                    }
                }
                TaskParameters.TaskLogger.LogInfo(@"Очистка папки завершена");
            }
            return true;
        }
    }
}
