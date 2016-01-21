using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbModels.DomainModels.ShClone
{
    public class ShAVRItem
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int AVRItemId { get; set; }

        public string FIXId { get; set; }
        public string FOLId { get; set; }
        public string SiteId { get; set; }
        public string Description { get; set; }

        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        public string ECRType { get; set; }
        public string ECRApprove { get; set; }
        public string Region { get; set; }
        //public string AVRFId { get; set; }
        [FKAttribute("AVRS_AVRId", typeof(string))]
        public virtual ShAVRs AVRS { get; set; }
        [FKAttribute("Limit_LimitCode", typeof(string))]
        public virtual ShLimit Limit { get; set; }

        public bool? InLimit { get; set; }
        public string WorkReason { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Note { get; set; }

        public decimal? VCQuantity { get; set; }
        public decimal? VCPrice { get; set; }
        public int? VCPriceListRevisionItemId { get; set; }

        public string VCDescription { get; set; }

        public bool VCCustomPos { get; set; }
        public bool VCUseCoeff { get; set; }

        public bool VCAddOnSales { get; set; }

        public string NoteVC { get; set; }



    }
}
