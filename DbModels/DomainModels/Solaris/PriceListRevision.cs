using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.Models.ImportFilesModels;
using DbModels.Models.Pors;

namespace DbModels.Models
{
    public class PriceListRevision:Entity
    {
      //  public int id { get; set; }
        public virtual PriceList PriceList { get; set; }
        public DateTime Uploaded { get; set; }
        public virtual ImportFile ImportFile { get; set; }
        public virtual ICollection<PriceListRevisionItem> PriceListRevisionItems { get; set; }
        public virtual ICollection<POR> PORs { get; set; }
        public PriceListRevision()
        {
            Uploaded = DateTime.Now;
            CreationDate = DateTime.Now;
        }
        public DateTime SignDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PaymentTerms { get; set; }
        public string VAT { get; set; }
        public DateTime CreationDate { get; set; }

        public bool Approved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
