using DbModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.Solaris
{
    public class PriceListMap
    {
        public int Id { get; set; }
        //public int ComparablePriceList_Id { get; set; }
        //public int PriceList_Id { get; set; }
        public virtual PriceList PriceList { get; set; }
        public virtual PriceList ComparablePriceList { get; set; }
    }
}
