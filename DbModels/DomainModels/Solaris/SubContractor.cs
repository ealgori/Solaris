using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models
{
    public class SubContractor:Entity
    {
       // public int id { get; set; }
        public string Name { get; set; }
        public string ShName { get; set; }
        public string Address { get; set; }
        public string SAPNumber { get; set; }
        public string SAPName { get; set; }
        public string NameRef { get; set; }
        public virtual Project Project { get; set; }

        public override string ToString()
        {
            return string.Format("Name:{0}; Address:{1}; SAPNumber:{2}; SAPName:{3}; {4}",Name,Address,SAPNumber,SAPName,(Project==null)?"":"Project:"+Project.Name);
        }
    }
}
