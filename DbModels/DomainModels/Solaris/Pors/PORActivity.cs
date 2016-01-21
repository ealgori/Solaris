using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Models;

namespace DbModels.DomainModels.Solaris.Pors
{
    public class PORActivity:Entity
    {
        public string Activity { get; set; }
        public string POType { get; set; }
        public string TOType { get; set; }
        public string TOWorkDescription { get; set; }

    }
}
