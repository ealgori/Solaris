using DbModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbModels.DomainModels.SAT
{
    public class SATPrepricedItem
    {
        public int Id { get; set; }
        public string AVRId { get; set; }
        public int? AVRItemId { get; set; }
        public virtual PriceListRevisionItem Item { get; set; }
        public decimal? vcQuantity { get; set; }
        public DateTime PrepriceDate { get; set; }

        
        /// <summary>
        /// Потребуется если сам сайт будет возвращать письмо
        /// </summary>
        public string VCRequestNumber { get; set; }

        public bool Uploaded { get; set; }

        public bool IsCustomItem { get; set; }

        public string VCDescription { get; set; }

        public decimal? vcPrice { get; set; }

        public bool VCUseCoeff { get; set; }
        public decimal? VCCoeff { get; set; }

        public string  NoteVC { get; set; }
    }
}
