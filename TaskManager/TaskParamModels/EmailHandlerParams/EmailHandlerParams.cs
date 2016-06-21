using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.TaskParamModels
{
    public class EmailHandlerParams
    {
        public List<EmailParams> EmailParams { get; set; }
        /// <summary>
        /// Возбуждать исключения, чтобы привести с срыву таска, и импорт не отработает
        /// </summary>

        public EmailHandlerParams()
        {
            EmailParams = new List<EmailParams>();

        }

        public void Add(List<string> recipients, List<string> ccrecipients,  string subject, bool allowWOAttach, string body, List<string> attachments, string testRecipients = null  )
        {
            EmailParams param = new EmailParams(recipients, subject);
           
            param.CCRecipients =ccrecipients;
            param.AllowWithoutAttachments = allowWOAttach;
            param.HtmlBody = body;
            param.FilePaths = attachments;
            param.TestRecipients = testRecipients;

            EmailParams.Add(param);


        }
    }
}
