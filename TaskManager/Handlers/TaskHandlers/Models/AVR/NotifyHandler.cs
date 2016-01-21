using DbModels.DataContext.Repositories;
using MailProcessing;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.Email;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR
{
    public class NotifyHandler : ATaskHandler
    {
        public NotifyHandler(TaskParameters taskParams) : base(taskParams)
        {

        }

        public override bool Handle()
        {
            var avrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.BaseComp)
                .Where(a=>a.Items.Any(AVRItemRepository.HasLimitComp)
                        ||a.Items.Any(AVRItemRepository.IsVCAddonSalesComp)).ToList();
            var cachedSATPors = TaskParameters.Context.AVRPORs.ToList();

            string admMailFormat = "Привет. Просьба опрайсовать следующие АВР: {0}";
            string vcAdmMailFormat = "Привет. Следующие АВР готовы к опрайсовке: {0}";
            var now = DateTime.Now;
            // у которых нет сат поров
            var avrsPN = avrs.Where(a => !a.PriceNotifySend.HasValue
                                         && !cachedSATPors.Any(p => p.AVRId == a.AVRId)                            
            ).ToList();


            // у которых есть сат поры
            var avrsVPN = avrs.Where(a => !a.VCPriceNotifySend.HasValue
                                            && cachedSATPors.Any(p => p.AVRId == a.AVRId)
            ).ToList();
            RedemptionMailProcessor processor = null;
            var importModels = new List<NoteDatesModel>();
            if (avrsPN.Any() || avrsVPN.Any())
            {
                processor = new RedemptionMailProcessor("SOLARIS");
            }
            else
                return true;
            

            if (avrsPN.Count > 0)
            {
                var autoMail = new AutoMail();
                autoMail.Email = DistributionConstants.EksenazEmail;
                autoMail.Subject = "Price request";
                autoMail.CCEmail = DistributionConstants.EekakosEmail;
                autoMail.Body = string.Format(admMailFormat, string.Join(",", avrsPN.Select(a=>a.AVRId)));
                var result = processor.SendMail(autoMail
                  //  , testRecipient: "aleksey.gorin@ericsson.com"
                                                );
                if(!string.IsNullOrEmpty(result))
                {
                    importModels.AddRange(avrsPN.Select(s => new NoteDatesModel() { AVRs = s.AVRId, PriceNotifySend = now }));
                }
             }
           
            if(avrsVPN.Count>0)
            {
                var autoMail = new AutoMail();
                autoMail.Email = DistributionConstants.EekakosEmail;
                autoMail.Subject = "VC price notify";
                autoMail.Body = string.Format(vcAdmMailFormat, string.Join(",", avrsVPN.Select(a=>a.AVRId)));
                var result = processor.SendMail(autoMail
                  //  , testRecipient: "aleksey.gorin@ericsson.com"
                                                );
                if (!string.IsNullOrEmpty(result))
                {
                    importModels.AddRange(avrsVPN.Select(s => new NoteDatesModel() { AVRs = s.AVRId, VCPriceNotifySend = now }));
                }
            }

            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });


            return true;

        }

       

        private class NoteDatesModel
        {
            public string AVRs { get; set; }
            public DateTime? PriceNotifySend { get; set; }
            public DateTime? VCPriceNotifySend { get; set; }
        }
    }
}
