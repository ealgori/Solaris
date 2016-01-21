using MailProcessing;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.Handlers.EmailHandlers.Abstract;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.EmailHandlers.Models
{
    public class BaseEmailHandler : AEmailHandler
    {
        public BaseEmailHandler(TaskParameters taskParameters)
            : base(taskParameters)
        {

        }
        public override bool SendMails()
        {

            RedemptionMailProcessor interactor = new RedemptionMailProcessor("SOLARIS");
            if (TaskParameters.EmailHandlerParams != null && TaskParameters.EmailHandlerParams.EmailParams.Count > 0)
            {

                foreach (var param in TaskParameters.EmailHandlerParams.EmailParams)
                {
                    try
                    {
                        AutoMail mail = new AutoMail
                        {
                            Subject = param.Subject,
                        };
                        if (param.Recipients == null || param.Recipients.Count() == 0)
                        {
                            throw new Exception("Не указаны получатели письма:" + param.Name);
                        }
                        var recipients = param.Recipients.Where(r => IsValidEmail(r));
                        if (recipients.Count() == 0)
                        {
                            throw new Exception("Ни одного корретного получателя письма:" + param.Name);
                        }
                        mail.Email = string.Join(";", recipients.Distinct());
                        if (param.CCRecipients != null && param.CCRecipients.Count > 0)
                        {
                            mail.CCEmail = string.Join(";", param.CCRecipients.Where(r => IsValidEmail(r)));
                        }
                        if (param.BCCRecipients != null && param.BCCRecipients.Count > 0)
                        {
                            mail.BCCEmail = string.Join(";", param.BCCRecipients.Where(r => IsValidEmail(r)));
                        }
                        
                         
                            var attachments = GetAttachments(param);

                            if (attachments == null || attachments.Count == 0)
                            {
                                if (!param.AllowWithoutAttachments)
                                {
                                    throw new Exception("Нет ни одного файла для отправки " + param.Name);
                                }
                            }
                            else
                            {
                                mail.Attachments.AddRange(attachments.Select(a => new Attachment() { FilePath = a }));
                            }
                        mail.Body = param.HtmlBody;
                        //Отправляем
                        interactor.SendMail(mail);


                    }
                    catch (Exception exc)
                    {
                        TaskParameters.TaskLogger.LogError(param.Name + " " + exc.Message);

                    }
                }

            }
            return true;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
