using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShCar
    {
        [Key]
        public string CarId { get; set; }
        public string Macroregion { get; set; }
        public string Responsible { get; set; }
        public string Manager { get; set; }
    }
}
