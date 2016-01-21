using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class DistributionHandler3 : ATaskHandler
    {
        public DistributionHandler3(TaskParameters taskParameters) : base(taskParameters) { }


        public readonly string NoRecipientsText = @"Для следующих subregions отсутсвтуют получателия уведомлений по рассылкам об утверждении авр:<p><b>{0}</b></p>. Пожалуйста воспользуйтесь существующим функционалом для их заполнения. ";
        public readonly string RukOtdelaText = @"Следющие заявки висят у Вас уже больше одного дня:<p><b>{0}</b></p> Пожалуйста утвердите их или попросите Руководителя отдела их утвердить. Данное сообщение будет приходить по всем не утвержденным заявкам на ежедневной основе.";
        public readonly string RukFilalaText = @"Следующие заявки висят у Вас уже больше одного дня:<p><b>{0}</b></p> Пожалуйста утвердите их. Данное сообщение будет приходить по всем не утвержденным заявкам на ежедневной основе.";
        public readonly string EksenazText = @"Ксюша, бери в работу следющие заявки:<p><b>{0}</b></p>";

        public override bool Handle()
        {
            string approved = "Утвержден";
            string declined = "Отклонен";
            bool test = false;

 
 
 
            var avrs = TaskParameters.Context.ShAVRs.Where(a=>a.Year!="2014").ToList();
            var avrSubRegions = avrs.Select(a => a.Subregion).Distinct().ToList();
            var satSubregions = avrSubRegions.GroupJoin(TaskParameters.Context.SATSubregions, s => s, sub => sub.Name, (s, sub) => new {s=s, subregions=sub }).ToList();

            // группировка. каждому сабрегиону соответствуте набор авр
           
            // так же исключаем заявки на удаление ПО
            var unApprovedOwnersGroup = 
                satSubregions.GroupJoin(
                avrs.Where(a => a.RukOtdela!=approved
                    // исключаем заявки отклоненные обоими руководителями
                &&!(a.RukOtdela==declined&&a.RukFiliala==declined)
                //&& a.RukFiliala!=declined
                &&string.IsNullOrEmpty(a.PurchaseOrderNumber)),s=>s.s, a=>a.Subregion,
                (s, a) => new { avrs = a, subregion = s });

           // var unApprovedOwnersGroup = unApprovedOwners.GroupBy(g => new { g.avrCreatedByEmail, g.subregion });
            // бежим по сабрегионам
            foreach (var subregionGroup in unApprovedOwnersGroup)
            {
                // группируем коллекцию авр региона по создателю
                var unapprovedGroup = subregionGroup.avrs.GroupBy(a => a.CreatedByEmail).ToList();

                foreach (var avrGroup in unapprovedGroup)
                {
                    var recipient = string.IsNullOrEmpty(avrGroup.Key) ? DistributionConstants.SolarisEmail : avrGroup.Key;
                    var rukOtdela = !subregionGroup.subregion.subregions.Any() ? "" : subregionGroup.subregion.subregions.FirstOrDefault().RukOtdelaEmail;//avrG
                    var subregion = string.Join(", ", avrGroup.Select(s => s.Subregion).Distinct());
                   // if (subregionGroup.avrs.Count() > 0)
                    if (avrGroup.Count() > 0)
                    {
                        TaskParameters.EmailHandlerParams.EmailParams.Add(
                            CreateEmail(
                            recipient
                            , rukOtdela
                            , string.Format(RukOtdelaText, string.Join(", ", avrGroup.Select(a => a.AVRId)))
                            ,subregion
                            , test)
                        );
                    }
                }
               
            }

            // исключаем заявки на удаление ПО
            var unApprovedRukFilialaGroup = 
                satSubregions.GroupJoin(
                avrs
                    .Where(a => 
                        a.RukOtdela == approved 
                        && string.IsNullOrEmpty(a.RukFiliala) 
                        && string.IsNullOrEmpty(a.PurchaseOrderNumber))
                , s => s.s
                , a => a.Subregion
                , (s, a) => new { avrs = a, subregion = s });
            // бежим по сабрегионам
           
            foreach (var subregionGroup in unApprovedRukFilialaGroup)
            {
                var recipient = !subregionGroup.subregion.subregions.Any() ? DistributionConstants.SolarisEmail : subregionGroup.subregion.subregions.FirstOrDefault().RukFillialaEmail;
                 var subregion = string.Join(", ",subregionGroup.avrs.Select(s=>s.Subregion).Distinct());
                if(subregionGroup.avrs.Count()>0)
                {
                    TaskParameters.EmailHandlerParams.EmailParams.Add(
                        CreateEmail(
                        recipient
                        , ""
                        , string.Format(RukFilalaText, string.Join(", ", subregionGroup.avrs.Select(a => a.AVRId)))
                        ,subregion
                        , test)
                    );
                }
            }
            // исключаем заявки на удаление ПО
            var unApprovedPORPOGroup = 
                satSubregions.GroupJoin(
                    avrs.Where(a => a.RukOtdela == approved
                    && a.RukFiliala == approved
                    && string.IsNullOrEmpty(a.PurchaseOrderNumber)
                    && a.ZayavkaECRAdmPoluchenaVobrabotku == null)
                ,s => s.s, a => a.Subregion, (s, a) => new { avrs = a, subregion = s });

            foreach (var subregionGroup in unApprovedPORPOGroup)
            {
                var recipient = !subregionGroup.subregion.subregions.Any() ? DistributionConstants.SolarisEmail : subregionGroup.subregion.subregions.FirstOrDefault().POROREmail;
                var subregion = string.Join(", ",subregionGroup.avrs.Select(s=>s.Subregion).Distinct());
                if(subregionGroup.avrs.Count()>0)
                {
                    TaskParameters.EmailHandlerParams.EmailParams.Add(
                        CreateEmail(
                        recipient
                        , ""
                        , string.Format(EksenazText, string.Join(", ", subregionGroup.avrs.Select(a => a.AVRId)))
                        , subregion
                        , test)
                    );
                }
            }

 
            

            return true;
        }

        private EmailParams CreateEmail(string recipient, string cc, string body, string subRegion, bool test = false)
        {
            List<string> emails = (recipient ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> ссemails = (cc ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (test)
            {
                emails = new List<string> { DistributionConstants.EalgoriEmail };
            }


            EmailParams param = new EmailParams(emails, "Утверждение авр");
            {
                param.Recipients = emails;
            }
            if (!test)
                param.CCRecipients = ссemails;
            param.AllowWithoutAttachments = true;
            if (test)
            {
                param.HtmlBody += string.Format(@"<p>Recipients:{0}</p>", recipient);
                param.HtmlBody += string.Format(@"<p>CCRecipients:{0}</p>", cc);
            }
            if (!string.IsNullOrEmpty(subRegion))
                param.HtmlBody += string.Format(@"<p>Subregion:{0}</p>", subRegion);
            param.HtmlBody += string.Format(@"{0}", body);
            return param;
        }
    }
}
