using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShFilialStruct
    {
        [Key]
        public string Name { get; set; }
        public string  RukFills { get; set; }
        public string  Engineers { get; set; }
    }
}
