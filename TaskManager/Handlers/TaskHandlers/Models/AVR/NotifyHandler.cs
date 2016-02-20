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
            //TODO: попробовать воспользоваться существующими условиями
            // если требует опрайсовки, и еще не отправлено, то отправляем Ксюше
            // после опрайсовки ксюшей при создании пора, проверим, нужно ли его в вк опрйсовывать, и если да, то поставим флаг
            //, что надо отправть Кате



            var avrs = TaskParameters.Context.ShAVRs.Where(AVRRepository.Base)
                .Where(a => a.Items.Any(AVRItemRepository.HasLimitComp)
                        || a.Items.Any(AVRItemRepository.IsVCAddonSalesComp)).ToList();
            var cachedSATPors = TaskParameters.Context.AVRPORs.ToList();

            string admMailFormat = "Привет. Просьба опрайсовать следующие АВР: {0}";
            string vcAdmMailFormat = "Привет. Следующие АВР готовы к опрайсовке: {0}";
            string musAdmMailFormat = "Привет.  По следующим авр пришел нетворк. Поры можешь забрать прямо по ссылкам : <ul>{0}</ul>";

            var now = DateTime.Now;
            var importModels = new List<NoteDatesModel>();
            RedemptionMailProcessor processor = new RedemptionMailProcessor("SOLARIS"); ;
           // у которых нет сат поров
           var avrsPN = avrs.Where(a => !a.PriceNotifySend.HasValue
                                       && a.Subcontractor != DbModels.Constants.EricssonSubcontractor && a.SubcontractorRef != DbModels.Constants.EricssonSubcontractor
                                        && !cachedSATPors.Any(p => p.AVRId == a.AVRId)
           ).ToList();


            // у которых есть сат поры
            var avrsVPN = avrs.Where(a => !a.VCPriceNotifySend.HasValue
                                            && (
                                            cachedSATPors.Any(p => p.AVRId == a.AVRId)
                                            || a.Subcontractor == DbModels.Constants.EricssonSubcontractor && a.SubcontractorRef == DbModels.Constants.EricssonSubcontractor
                                            )
            ).ToList();


            //if (avrsPN.Any() || avrsVPN.Any())
            //{
            //    processor = 
            //}
            //else
            //    return true;


            if (avrsPN.Count > 0)
            {
                var autoMail = new AutoMail();
                autoMail.Email = DistributionConstants.EksenazEmail;
                autoMail.Subject = "Price request";
                autoMail.CCEmail = DistributionConstants.EekakosEmail;
                autoMail.Body = string.Format(admMailFormat, string.Join(",", avrsPN.Select(a => a.AVRId)));
                var result = processor.SendMail(autoMail
                                                //  , testRecipient: "aleksey.gorin@ericsson.com"
                                                );
                if (!string.IsNullOrEmpty(result))
                {
                    importModels.AddRange(avrsPN.Select(s => new NoteDatesModel() { AVRs = s.AVRId, PriceNotifySend = now }));
                }
            }

            if (avrsVPN.Count > 0)
            {
                var autoMail = new AutoMail();
                autoMail.Email = DistributionConstants.EekakosEmail;
                autoMail.Subject = "VC price notify";
                autoMail.Body = string.Format(vcAdmMailFormat, string.Join(",", avrsVPN.Select(a => a.AVRId)));
                var result = processor.SendMail(autoMail
                                                //  , testRecipient: "aleksey.gorin@ericsson.com"
                                                );
                if (!string.IsNullOrEmpty(result))
                {
                    importModels.AddRange(avrsVPN.Select(s => new NoteDatesModel() { AVRs = s.AVRId, VCPriceNotifySend = now }));
                }
            }

            var musNetworkAVRs = TaskParameters.Context.ShAVRs.Where(a =>
            (!string.IsNullOrEmpty(a.MUSNetwork))
            && (!a.MUSNetworkNotifySend.HasValue)
            ).ToList();

            string linkTemplate = "<li><a href='http://eeca.ericsson.se/applications/Solaris/POR/PrintPor/{0}' >{1}</a></li>";
            List<string> links = new List<string>();
            foreach (var avr in musNetworkAVRs)
            {
                var avrPor = TaskParameters.Context.AVRPORs.Where(r => r.AVRId == avr.AVRId).OrderByDescending(a => a.Id).FirstOrDefault();
                if (avrPor != null)
                {
                    links.Add(string.Format(linkTemplate, avrPor.Id, avrPor.AVRId));
                    importModels.Add(new NoteDatesModel() { AVRs = avrPor.AVRId, MUSNetworkNotifySend = now });
                }
            }
            if(links.Count>0)
            {
                var autoMail = new AutoMail();
                autoMail.Email = DistributionConstants.EksenazEmail;
                autoMail.Subject = "Por ready";
                autoMail.Body = string.Format(musAdmMailFormat, string.Join(" ", links));
                var result = processor.SendMail(autoMail
                                                 // , testRecipient: "aleksey.gorin@ericsson.com"
                                                );
            }

           // importModels.AddRange(musNetworkAVRs.Select(s => new NoteDatesModel() { AVRs = s.AVRId,  MUSNetworkNotifySend = now }));



            TaskParameters.ImportHandlerParams.ImportParams.Add(new ImportParams { ImportFileNearlyName = TaskParameters.DbTask.ImportFileName1, Objects = new ArrayList(importModels) });


            return true;

        }

       

        private class NoteDatesModel
        {
            public string AVRs { get; set; }
            public DateTime? PriceNotifySend { get; set; }
            public DateTime? VCPriceNotifySend { get; set; }
            public DateTime? MUSNetworkNotifySend { get; set; }
        }
    }
}
