using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models
{
    public class PriceListRevisionItem:Entity
    {
      //  public int id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string Site { get; set; }
        public string Address { get; set; }
        public virtual PriceListRevision PriceListRevision { get; set; }
        public virtual SAPCode SAPCode { get; set; }
    }
}
