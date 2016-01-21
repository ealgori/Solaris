using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShSostavRabotTOItem
    {
        [Key]
        public string SostavRabotTOItemId { get; set; }
        public decimal? Quantity { get; set; }
        public string Description { get; set; }
        public string SostavRabotTOid { get; set; }
        public decimal? Price { get; set; }

    }
}
