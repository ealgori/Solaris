using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.SAT
{
    public class SatMusItem
    {
        public int Id { get; set; }
        public string AVRId { get; set; }
        public string VCRequestNumber { get; set; }

        public bool UseCoeff { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string NoteVC { get; set; }
        public string WorkReason { get; set; }

    }
}
