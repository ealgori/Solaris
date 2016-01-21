using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class AVRItemModel
    {
        public int id { get; set; }

        public string shDesc { get; set; }
        public decimal? shPrice { get; set; }
        public decimal? shQuantity { get; set; }
        public int? vcPriceListRevisionItemId { get; set; }
        public decimal? vcQuantity { get; set; }
        public string vcDescription { get; set; }
        public bool vcCustomPos { get; set; }
        public decimal? vcPrice { get; set; }
        public bool vcUseCoeff { get; set; }
        public int itemId { get; set; }

        public string noteVC { get; set; }
    }
}