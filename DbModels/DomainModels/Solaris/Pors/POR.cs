using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.Pors
{
    public class POR:Entity
    {
        public DateTime PrintDate { get; set; }
        public virtual SubContractor SubContractor { get; set; }
        public virtual ICollection<PriceListRevision> PriceListRevisions { get; set; }
        public string SubContractorName { get; set; }
        public string SubContractorSAPNumber { get; set; }
        public string SubContractorAddress { get; set; }
        public string PriceListNumbers { get; set; }
        //public string PriceListAdditionalNumber { get; set; }
        public DateTime WorkStart { get; set; }
        public DateTime WorkEnd { get; set; }
        public string MacroRegion { get; set; }
        public string SubRegion { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<PORItem> PorItems { get; set; }
        public string UserName { get; set; }
        public string Network { get; set; }
        public virtual PORNetwork PORNetwork { get; set; }
        public string Activity { get; set; }
        public string POType { get; set; }
        public virtual ICollection<PORStatus> PORStatuses { get; set; }


    }
}
