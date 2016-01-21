using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.Solaris
{
    public class Activity
    {
        
        public string ActivityName { get; set; }
        [Key]
        public string TOType { get; set; }
        public string Description { get; set; }
    }
}
