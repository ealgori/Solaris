using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.DomainModels.SAT
{
    public  class SATActItem
    {
        public int Id { get; set; }
        public string ShId { get; set; }
        public virtual SATAct SATAct { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Site { get; set; }
        public string SiteAddress { get; set; }
    }

    public class SATActMaterial:SATActItem
    {

    }
    public class SATActService:SATActItem
    {
        public DateTime FactDate { get; set; }
    }

    
}
