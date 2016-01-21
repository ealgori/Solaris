using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.TaskHandlers.Models.Email
{
    public class DistributionHandler2 : ATaskHandler
    {
        public DistributionHandler2(TaskParameters taskParameters) : base(taskParameters) { }
        public override bool Handle()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                return true;

            bool test = false;
            List<string> testRecipients = new List<string> { DistributionConstants.EalgoriEmail };
            DateTime expiaryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-2);
            var paymentRowsAvr = TaskParameters.Context.ShInvoices.Where(t => !string.IsNullOrEmpty(t.AVRid)).Join(TaskParameters.Context.ShAVRs, i => i.AVRid, a => a.AVRId, (i, a) => new { i, a }).Where(s =>
                s.i.PmntDate.HasValue &&
                (s.i.PmntDate.Value <= expiaryDate)
                &&
                !s.i.Clearing.HasValue && !string.IsNullOrEmpty(s.i.PONumber)
                );
            List<string> payments = new List<string>();
            foreach (var item  in paymentRowsAvr.OrderBy(p=>p.i.PmntDate))
            {
                payments.Add(string.Format("AVR: {0}  - PO: {1}, Payment date: {2}, Подрядчик:{3}, Номер счета:{4}, Номер счета-фактуры:{5}"
                    , item.i.AVRid
                    , item.i.PONumber
                    , item.i.PmntDate.Value.ToString("dd.MM.yyy")
                    , item.a.Subcontractor
                    , item.i.InvoiceNumber??"нет"
                    , item.i.FacturaNumber??"нет"
                    ,item.i.InvoiceNumber
                    
                    ));
            }
            var paymentRowsTO = TaskParameters.Context.ShInvoices.Where(t => !string.IsNullOrEmpty(t.TOId.Trim())).Join(TaskParameters.Context.ShTOes, i => i.TOId, a => a.TO, (i, a) => new { i, a }).Where(s =>
                s.i.PmntDate.HasValue &&
                (s.i.PmntDate.Value <= expiaryDate)
                &&
                !s.i.Clearing.HasValue && !string.IsNullOrEmpty(s.i.PONumber)
                );

            foreach (var item in paymentRowsTO.OrderBy(p => p.i.PmntDate))
            {
                payments.Add(string.Format("TO: {0}  - PO: {1}, Payment date: {2}, Подрядчик:{3}, Номер счета:{4}, Номер счета-фактуры:{5}"
                    , item.i.TOId

                    , item.i.PONumber
                    , item.i.PmntDate.Value.ToString("dd.MM.yyy")
                    , item.a.Subcontractor
                    , item.i.InvoiceNumber ?? "нет"
                    , item.i.FacturaNumber ?? "нет"
                  

                    ));
            }

            if (paymentRowsAvr.Count() > 0||paymentRowsTO.Count()>0)
            {

                var emails = new List<string>();
                if (test)
                {
                    emails = new List<string> { DistributionConstants.EalgoriEmail };
                }
                else
                {
                    emails = new List<string> { "kseniya.nazarenko@ericsson.com","valeriy.podoruev@ericsson.com", "aleksey.borshchev@ericsson.com", "evgeniy.devyatkov@ericsson.com", "andrey.zhelezin@ericsson.com", "nadezhda.nikiforova@ericsson.com", "ekaterina.gurina@ericsson.com" };
                }

                EmailParams param = new EmailParams(emails, "Payment overdue summary ");
                {
                    param.Recipients = emails;
                }
                param.CCRecipients = new List<string>() { "aleksey.chekalin@ericsson.com" };
                param.AllowWithoutAttachments = true;
                param.HtmlBody += string.Format(@"Hello
<br>
Payment to ASP is overdue, for the order number:
<br>
{0}
<br>
https://sitehandler-emea2.ericsson.net/sh-emea2/
<br>
This is automatic message, please don't reply to it", payments.Count > 0 ? string.Join("<br> ", payments) : "No expirated payments");
                param.HtmlBody += @"<br>";

                TaskParameters.EmailHandlerParams.EmailParams.Add(param);


            }
            return true;


        }
    }
}
