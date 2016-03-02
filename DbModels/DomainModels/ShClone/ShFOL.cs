using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShFOL
    {
        [Key]
        public string FOL { get; set; }
        public string MacroRegion { get; set; }
        public string Branch { get; set; }
        public string StartPoint { get; set; }
        public string DestinationPoint { get; set; }
    }
}
