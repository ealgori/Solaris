using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.SAT
{
    public class SATTO
    {
        public int Id { get; set; }
        [Required]
        public string TO { get; set; }
        [Required]
        public string Activity { get; set; }
        public string Network { get; set; }
        [Required]
        public string SubContractor { get; set; }
        public string SubContractorSapNumber { get; set; }
        public string SubContractorAddress { get; set; }

        [Required]
        public string ToType { get; set; }
        public decimal Total { get; set; }
        public decimal TotalMaterials { get; set; }
        public decimal TotalServices { get; set; }
       
        public string Region { get; set; }
        
        public string Branch { get; set; }

        public bool UploadedToSh { get; set; }
        public DateTime? ShUploadDate { get; set; }
        public string ShComment { get; set; }
        [Required]
        public string CreateUserName { get; set; }
        [Required]
        public DateTime CreateUserDate { get; set; }
        public virtual ICollection<SATTOItem> SATTOItems { get; set; }
        public string ProceListNumbers { get; set; }
        public DateTime? PriceListDate { get; set; }

        public string NomerDogovora { get; set; }
        public DateTime? DataDogovora { get; set; }
        public string WorkDescription { get; set; }
        public string POType { get; set; }
        public bool WOVAT { get; set; }
    }
}
