using DbModels.Models;
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
        public PriceListRevisionItem PriceListRevisionItem { get; set; }
        public bool CustomPos   { get; set; }
        public int? AvrItemId { get; set; }
        /// <summary>
        /// описание кастом айтема
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// кастом цена.  опрайсованная цена на прайревижонайтеме
        /// </summary>
        public decimal Price { get; set; }
       
        public decimal Quantity { get; set; }
        public string NoteVC { get; set; }
        public string WorkReason { get; set; }

        /// <summary>
        /// описание из сх
        /// </summary>
        public string ShDescription { get; set; }
        public decimal ShPrice { get; set; }
        public decimal ShQuantity { get; set; }

    }
}
