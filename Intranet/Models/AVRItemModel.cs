using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class AVRItemModel
    {
        public int? id { get; set; }

        public string description { get; set; }
      
        public decimal? price { get; set; }
        public decimal? quantity { get; set; }
        //public int? vcPriceListRevisionItemId { get; set; }
        //public decimal? vcQuantity { get; set; }
        //public string vcDescription { get; set; }
        public bool vcCustomPos { get; set; }
        //public decimal? vcPrice { get; set; }
        public bool vcUseCoeff { get; set; }
        public int? priceListRevisionItemId { get; set; }
        public int? avrItemId { get; set; }

        public string noteVC { get; set; }

        public string workReason { get; set; }


        public string shDescription { get; set; }
        public decimal? shQuantity { get; set; }
        public decimal? shPrice { get; set; }


    }
}