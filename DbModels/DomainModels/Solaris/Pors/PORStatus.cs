using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.Pors
{
    public class PORStatus:Entity
    {
        public DateTime StatusDate { get; set; }
        public virtual Status Status { get; set; }
        public virtual POR POR { get; set; }
    }
}
