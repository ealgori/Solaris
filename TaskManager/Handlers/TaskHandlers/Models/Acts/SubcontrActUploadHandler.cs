using MailProcessing;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Acts
{
    public class SubcontrActUploadHandler : ATaskHandler
    {
        public SubcontrActUploadHandler(TaskParameters taskParams) : base(taskParams)
        {
        }

        public override bool Handle()
        {
            var processor = new RedemptionMailProcessor("SOLARIS");
            var mails = processor.GetMails(new List<string> { "#Act#" });
            foreach (var mail in mails)
            {
                if (mail.Attachments.Count > 0)
                {
                    var builder = new StringBuilder();
                    foreach (var attach in mail.Attachments)
                    {
                        var to = GetToNameFromFileName(attach.FilePath);
                        var shTO = TaskParameters.Context.ShTOes.FirstOrDefault(t => t.TO == to);
                        if (shTO != null)
                        {
                            var contact = TaskParameters.Context.ShContacts.FirstOrDefault(c => c.Contact == shTO.Subcontractor);
                            if (contact != null)
                            {
                                if (contact.ActUploadEmail.ToLower().Contains(mail.Email.ToLower()))
                                {
                                    // загружаем акт
                                    AddToLog($"{attach.File} загружен", "Успешно", builder);
                                }
                                else
                                {

                                    // добавляем в лог ошибок, что нет прав
                                    AddToLog($"{attach.File} не загружен. У Вас нет прав", "Ошибка", builder);
                                }
                            }
                        }
                        else
                        {
                            // нет то
                            AddToLog($"{attach.File} не загружен. ТО '{to}' не существует в СХ", "Ошибка", builder);
                        }
                    }
                    //шлем ответ
                    var reply = new AutoMail()
                    {
                        Email = mail.Email,
                        Subject = $"RE {mail.Subject}",
                        Body = builder.ToString()

                    };
                    processor.SendMail(reply);
                }

                // перемещаем в колмплитед
                processor.MoveToCompleted(mail.ConversationId);
            }
            return true;
        }


        public void AddToLog(string message, string status, StringBuilder builder)
        {
            if (builder != null)
            {
                builder.AppendLine($"<p>{status}:{message}</p>");
            }
        }


        string GetToNameFromFileName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
