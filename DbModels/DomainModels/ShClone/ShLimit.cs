using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShLimit
    {
        [Key]
        public string LimitCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
       // public decimal? Limit { get; set; }
        public decimal? SettedLimit { get; set; }
        public decimal? Executed { get; set; }
        public string Units { get; set; }
        public string Description { get; set; }
        public decimal? ShTestValue { get; set; }
        [Exclude]
        public virtual ICollection<ShAVRItem> ShAVRitems { get; set; }

        public decimal? InitValue { get; set; }

    }
}
