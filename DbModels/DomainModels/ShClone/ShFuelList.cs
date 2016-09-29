using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShFuelList
    {
        [Key]
        public string FuelList { get; set; }
        public string Required { get; set; }
        public string Generator { get; set; }
        public string Responsible { get; set; }
        public string Manager { get; set; }
    }
}
