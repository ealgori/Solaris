using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.SAT
{
    public class SATAct
    {
        public int Id { get; set; }
        public string ActName { get; set; }
        public string CreateName { get; set; }
        public DateTime CreateDate { get; set; }
        public string TO { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public virtual ICollection<SATActItem> SATActItems { get; set; }
        public DateTime? UploadedToSHDate { get; set; }
        public bool UploadedToSH { get; set; }
        public string UploadToSHComment { get; set; }
        public bool WOVAT { get; set; }
        public string NomerDogovora { get; set; }
        public DateTime? DataDogovora { get; set; }
        public string WorkDescription { get; set; }
        public string Region { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string Branch { get; set; }
        public string Network { get; set; }
        public string SubContractor { get; set; }
        public string SubContractorSapNumber { get; set; }
        public string SubContractorAddress { get; set; }
        public bool PrintReadyToUpload { get; set; }
        public DateTime? PrintUploadDate { get; set; }
        public string PrintUploadUser { get; set; }
       // public virtual ICollection<SATActMaterial> SATActMaterials { get; set; }
       // public virtual ICollection<SATActService> SATActSerices { get; set; }
    }
}
