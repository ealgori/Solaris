using DbModels.DomainModels.ShClone;
using MailProcessing;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;
using CommonFunctions.Extentions;
using System.Collections;

namespace TaskManager.Handlers.TaskHandlers.Models.Acts
{
    public class SendToSubcontractorHandler : ATaskHandler
    {
        public SendToSubcontractorHandler(TaskParameters param):base(param)
        {

        }
        public override bool Handle()
        {
            var test = true;

            DateTime startDate = new DateTime(2016,05,1);
            Expression<Func<ShAct, bool>> actExpr = a=>
            !string.IsNullOrEmpty(a.ActLink)
            &&a.ActApprovedDate.HasValue
            &&!a.SendToSubcontracor.HasValue
            &&a.CreateDate>startDate
            ;
            var importModels = new List<ActSendDateImport>();
            var infos = new List<string>();

            var now = DateTime.Now;


            var acts = TaskParameters.Context.ShActs.Where(actExpr).ToList();
            if (acts.Count > 0)
            {
                var processor = new RedemptionMailProcessor("SOLARIS");
                foreach (var act in acts)
                {
                    if (File.Exists(act.ActLink))
                    {
                        var shTO = TaskParameters.Context.ShTOes.FirstOrDefault(t => t.TO == act.TOId);
                        if (shTO != null)
                        {
                            var contact = TaskParameters.Context.ShContacts.FirstOrDefault(s => s.Contact == shTO.Subcontractor);
                            if (contact != null && !string.IsNullOrEmpty(contact.EMailAddress))
                            {
                                AutoMail mail = new AutoMail
                                {
                                    Subject = $"Act {shTO.TO}-{act.Act}",
                                    Email = contact.Contact,

                                };
                                mail.Attachments.Add(new Attachment() { FilePath=act.ActLink});
                                var result = processor.SendMail(mail,null,test?DistributionConstants.EalgoriEmail:null);
                                if(!string.IsNullOrEmpty(result))
                                {
                                    importModels.Add(new ActSendDateImport { Act=act.Act, SendDate = now });
                                }
                            }
                            else
                            {
                                infos.Add($"Отсутствуют адресаты для {shTO.Subcontractor}");
                            }
                        }
                    }
                    else
                    {
                        infos.Add($"Файл отсутсвует по пути: {act.ActLink}");
                        // надо уведомить об отсутсвтии файла акта
                    }
                }
            }
            if(infos.Count>0)
            {
                EmailParams param = new EmailParams(new List<string> { DistributionConstants.EalgoriEmail}, "Act send to Subc errors");
                if(!test)
                    param.CCRecipients = new List<string>() { DistributionConstants.EgorovEmail };
                param.AllowWithoutAttachments = true;
                param.HtmlBody += string.Format(@"");
                param.DataTables.Add("info.xls", infos.ToDataTable());
                TaskParameters.EmailHandlerParams.EmailParams.Add(param);

            }
            if(importModels.Count>0&&!test)
            {
                TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });

            }
            return true;
        }


        class ActSendDateImport
        {
            public string Act { get; set; }
            public DateTime? SendDate { get; set; }
        }
    }
}
