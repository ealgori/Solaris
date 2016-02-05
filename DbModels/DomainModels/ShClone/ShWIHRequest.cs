using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShWIHRequest
    {
        [Key]
        public string WIHrequests { get; set; }
        public DateTime? RequestSentToODdate { get; set; }
        public string WIHnumber { get; set; }
        public DateTime? CompletedByOD { get; set; }
        public DateTime? RejectedByOD { get; set; }
        public string RejectedComment { get; set; }
        public string TOid { get; set; }
        public string AVRId { get; set; }
        public bool CorrectionCompleted { get; set; }
        public string Type { get; set; }
        public string AddAgreementId { get; set; }

    }
}
