using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class PorAnalyzerModel
    {
        public int subcontractor { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
       // public int? sourceSubc { get; set; }
    }
}