using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Models.Pors;

namespace DbModels.DomainModels.Solaris.Pors
{
    public class AVRPOR:POR
    {
        public DateTime? UploadedToSH { get; set; }
        public string AVRId { get; set; }
    }
}
