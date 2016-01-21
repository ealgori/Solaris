using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.Pors
{
    public class PORNetwork:Entity
    {
        public int Network { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string MacroRegion { get; set; }
        public int Network2014 { get; set; }
        public string SiteBranch { get; set; }
        public string RegionRus { get; set; }
    }
}
