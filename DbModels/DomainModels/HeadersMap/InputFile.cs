using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.HeadersMap
{
    public class InputFile
    {
        public int Id { get; set; }
        public string Mask { get; set; }
        public virtual ICollection<DbHeader> DbHeaders { get; set; }
    }
}