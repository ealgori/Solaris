using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.ShClone
{
    public class ShFiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mask { get; set; }
        public string TableName { get; set; }
        public bool Required { get; set; }
        public string TypeName { get; set; }
    }
}