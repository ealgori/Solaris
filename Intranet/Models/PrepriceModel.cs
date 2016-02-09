using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class PrepriceModel
    {
        public string avrId { get; set; }
        public List<AVRItemModel> items { get; set; }
    }
}