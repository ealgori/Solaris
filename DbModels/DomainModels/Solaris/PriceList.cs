using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models
{
    public class PriceList:Entity
    {
        public PriceList()
        {
            //CreationDate = DateTime.Now;
        }
       /// public int id { get; set; }
        public string PriceListNumber { get; set; }
        public string PriceListAdditionalNumber { get; set; }
        public virtual SubContractor SubContractor { get; set; }
        public virtual Project Project { get; set; }
        //public DateTime SignDate { get; set; }
        //public DateTime? ExpiryDate { get; set; }
        //public string PaymentTerms { get; set; }
        //public string VAT { get; set; }
        //public DateTime CreationDate { get; set; }
        public virtual ICollection<PriceListRevision> PriceListRevisions { get; set; }
        /// <summary>
        /// Прайс с этой отметкой не должен участвовать в создании поров. Единственная его функция, служить опорой, для сравнения с другими прас листами
        /// </summary>
        public bool Comparable { get; set; }

    }
}
