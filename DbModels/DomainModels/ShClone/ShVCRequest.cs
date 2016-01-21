using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShVCRequest
    {
        [Key]
        public string Id { get; set; }
        [FKAttribute("ShAVRs_AVRId",typeof(string))]
        public virtual ShAVRs ShAVRs { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? RequestSend { get; set; }
        public bool SendRequest { get; set; }

        public bool HasRequest { get; set; }
        public bool HasOrder { get; set; }

        public DateTime? RequestAccepted { get; set; }
        public DateTime? RequestRejected { get; set; }

        public DateTime? OrderAccepted { get; set; }
        public DateTime? OrderRejected { get; set; }
      
        public string RejectReason { get; set; }

        public string Attachments { get; set; }

        //public string MailPath { get; set; }
       
        
    }

    //public enum RequestType 
    //{
    //    Notify, NotifyWithOrder,Request,RequestWithOrder
    //}
}
