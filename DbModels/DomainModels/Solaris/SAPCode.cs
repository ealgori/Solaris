using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models
{
    public class SAPCode:Entity
    {
       // public int id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool ExistedInSAP { get; set; }
        public string Vendor {get;set; }
        public string EmailId { get; set; }
    }
}
