using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
  public  class ShAvrFull
    {
        [Key]
        public string AVRId { get; set; }
        public string Subregion { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        public string Subcontractor { get; set; }
        public string Project { get; set; }
        public string Region { get; set; }
        public string PurchaseOrderNumber { get; set; }
    }
}
