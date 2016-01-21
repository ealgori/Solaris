using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace DbModels.DomainModels.ShClone
{
   
    public class ShTOItem
    {
        [Key]
        public string TOItem { get; set; }
        public string PORTOItem { get; set; }
        public string TOId { get; set; }
        public string Site { get; set; }
        public string FOL { get; set; }
        public string FIX { get; set; }
        public decimal? PriceFromPL { get; set; }
        public int? IDItemFromPL { get; set; }
        public string DescriptionFromPL { get; set; }
        public int? PLItemRevisionID { get; set; }
        public DateTime? TOPlanDate { get; set; }
        public DateTime? TOFactDate { get; set; }
        public DateTime? TOPlanDateSubcontractor { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public bool WorkConfirmedByEricsson{ get; set; }
        public string WorkConfirmedByEricssonBy { get; set; }
        public DateTime? WorkConfirmedByEricssonDate { get; set; }
        public string FileReportTO1 { get; set; }
        public string FileReportTO2 { get; set; }
        public string FileReportTO3 { get; set; }
        public string FileReportTO4 { get; set; }
        public string ActId { get; set; }
        public string ReasonForPartialClosure { get; set; }
        public decimal? EquipmentQuantity { get; set; }
        public string EquipmentName { get; set; }
        public bool? ExcludeWork { get; set; }
        public DateTime? OtchetPredostavlenVCfact  { get; set; }
        public string LinkToReportinEridoc  { get; set; }
        public string AddAgreementId { get; set; }


    }
}
