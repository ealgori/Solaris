using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class PriceListUploadModel
    {
        public int ProjectId { get; set; }
        public bool Comparable { get; set; }
    }
    public class PriceListViewModel
    {
        public int PriceListId { get; set; }
        public string PriceListNumber { get; set; }
        public string PriceListAdditionalNumber { get; set; }
        public string VendorNumber { get; set; }
        public string VendorName { get; set; }
        public int? SubcId { get; set; }
        public int? ProjectId { get; set; }
        public DateTime SignDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Project { get; set; }
        public bool ForCompare { get; set; }
    }
}