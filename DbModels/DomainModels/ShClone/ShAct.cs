using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShAct
    {
        [Key]
        public string Act { get; set; }
        public DateTime? SentDatePeriodStart { get; set; }
        public DateTime? SentDatePeriodFinish { get; set; }
        public string SentNoteInformation { get; set; }
        public string TOId { get; set; }
        public string ActLink { get; set; }
        public bool GetActLink { get; set; }

        public decimal? StoimostRabot { get; set; }
        public decimal? StoimostMaterialov { get; set; }
        public decimal? ObshayaStoimost { get; set; }
       
        public DateTime? ActApprovedDate { get; set; }

    }
}
