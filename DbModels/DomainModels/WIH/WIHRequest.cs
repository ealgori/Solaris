using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Models;

namespace DbModels.DomainModels.WIH
{
   public class WIHRequest:Entity
    {
        public string Filename { get; set; }
        public string WIHNumber { get; set; }
        public string Type { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? ReceivedWIHNumberDate { get; set; }
        public bool? Approved { get; set; }
        public DateTime? ReceivedWIHResultDate { get; set; }
        public string RejectReason { get; set; }

    }
}
