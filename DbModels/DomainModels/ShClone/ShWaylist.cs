
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShWaylist
    {
        [Key]
        public string Waylist { get; set; }
        public string Car { get; set; }
        public DateTime? Date { get; set; }
        public string  File { get; set; }
    }
}
