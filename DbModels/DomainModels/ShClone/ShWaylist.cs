
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
        public string  File1 { get; set; }
        public string Fueling { get; set; }
        public string Required { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string StartRide { get; set; }
        public string EndRide { get; set; }
        public string Comment { get; set; }

    }
}
