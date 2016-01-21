using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DbModels.DomainModels.ShClone
{
    public class ShTO
    {
        [Key]
        public string TO { get; set; }
        public string TOapproved { get; set; }
        public string TOType  { get; set; }
        public string TOTotalAmmountApproved  { get; set; }
        public decimal? TOTotalAmmount  { get; set; }
        public string Subcontractor { get; set; }
        public string NomerDogovora { get; set; }
        public DateTime? DataDogovora { get; set; }
        public string WorkDescription { get; set; }
        public string SostavRabotTOid { get; set; }
        public string PONumber { get; set; }
        public DateTime? POIssueDate { get; set; }
        public bool RecallPO { get; set; }
        public string RecallPOComment { get; set; }
        public bool PrintSOW { get; set; }
        public bool EquipmentTO { get; set; }
        public string Network { get; set; }
        public string ActivityCode { get; set; }
        public string Year { get; set; }

        public decimal? TOTotalAmountNew { get; set; }
        public int? PlanObektov { get; set; }
        public string ObichniyRegulyarniyTO  { get; set; }
        public bool NotForPOR { get; set; }
    }
}
