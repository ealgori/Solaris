using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShMatToItem
    {
        [Key]
        public string MatTOId { get; set; }
        public string TOId { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? IDItemFromPL { get; set; }
        public int? PLItemRevisionID { get; set; }
        public string SiteId { get; set; }
    }
}
