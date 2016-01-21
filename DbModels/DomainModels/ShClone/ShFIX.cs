using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShFIX
    {
        [Key]
        public string FIX { get; set; }
        public string Address { get; set; }
        public string MacroRegion { get; set; }
    }
}
