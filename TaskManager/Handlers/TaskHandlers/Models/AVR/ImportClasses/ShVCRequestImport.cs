using DbModels.DomainModels.ShClone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses
{
    public class ShVCRequestImport
    {
       
        public string Id { get; set; }
        public string ShAVRs { get; set; }
        public DateTime? RequestSend { get; set; }
        public bool SendRequest { get; set; }
        public DateTime? RequestAccept { get; set; }
        public DateTime? RequestReject { get; set; }
        public DateTime? OrderAccept { get; set; }
        public DateTime? OrderReject { get; set; }
        public string RejectReason { get; set; }
        public string Attachment { get; set; }

       
        public bool? HasRequest { get; set; }
        public bool? HasOrder { get; set; }
       // public string RequestType { get; set; }

       // public string MailPath { get; set; }
    }
}
