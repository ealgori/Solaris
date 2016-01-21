using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class SATPorModel
    {
        public int Id { get; set; }
        public DateTime PrintDate { get; set; }
        public string UserName { get; set; }
        public string SubContractorName { get; set; }
        public DateTime? WorkStart { get; set; }
        public DateTime? WorkEnd { get; set; }
        public string Project { get; set; }
        public string AVR { get; set; }
        public string Status { get; set; }
    }
}