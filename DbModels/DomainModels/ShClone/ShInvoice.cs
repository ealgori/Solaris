using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels
{
    [Serializable]
    public class ShInvoice
    {  
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public int InvoiceId { get; set; }
        public string TableName { get; set; }
        public string DocumentType { get; set; }
        public string PaymentTerms { get; set; }
        public string SiteID { get; set; }
        public string PONumber { get; set; }
        public string VendorName { get; set; }
        public string InvoiceNumber { get; set; }
        public string FacturaNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? ReceivingDate { get; set; }
        public string LASResponsible { get; set; }
        public decimal? TotalAmount { get; set; }
        public string ApprovedByOD { get; set; }
        public DateTime? PassedToFinance { get; set; }
        public string Comments { get; set; }
        public DateTime? ScannedToOCRWF { get; set; }
        public DateTime? SentToSubcontractor { get; set; }
        public string DeliveryNoteNumber { get; set; }
        public string ResponsiblePerson { get; set; }
        public string TOId { get; set; }
        public bool InSH { get; set; }
        public DateTime? PmntDate { get; set; }
        public DateTime? PstngDate { get; set; }
        public DateTime? Clearing { get; set; }
        public string AVRid { get; set; }
        public int? ActId { get; set; }
     

    }

}
