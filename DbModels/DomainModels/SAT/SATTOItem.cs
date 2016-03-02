using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DbModels.Models;

namespace DbModels.DomainModels.SAT
{
    public class SATTOItem
    {
        [Key]
        public int Id { get; set; }
        public string TOItemId { get; set; }
        public string MatTOItemId { get; set; }
        public  virtual PriceListRevisionItem PriceListRevisionItem { get; set; }
        public virtual PriceListRevision PriceListRevision { get; set; }
        public decimal PricePerItem { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Site { get; set; }
        public string  FOL { get; set; }
        public int? SiteIndex { get; set; }
        public string SiteAddress { get; set; }
        public string Description { get; set; } 
        public virtual SATTO SATTO { get; set; }
        public DateTime? PlanDate { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }



    }
}
