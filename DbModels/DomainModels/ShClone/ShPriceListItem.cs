using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShPriceListItem
    {
        [Key]
        public string PriceListItemsId { get; set; }
        public string SapCodeNumber { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int RevisionId { get; set; }
        public string Unit { get; set; }
        public string SubContractor { get; set; }
        public string PriceListNumber { get; set; }

    }
}
