using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.DomainModels.Solaris.Pors;
using DbModels.Models.Pors;

namespace DbModels.DomainModels.Solaris
{
    public class PurchaseRequest
    {
        public int Id { get; set; }
        public virtual PORActivity Activity { get; set; }
        public virtual PORNetwork Network { get; set; }
        public string PurchReqNo { get; set; }
        public string PRItem { get; set; }
    }
}
