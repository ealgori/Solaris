using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.ImportFilesModels
{
    /// <summary>
    /// 
    /// </summary>
    public class Import:Entity
    {
        public virtual ICollection <ImportFile> Files { get; set; }
        public DateTime CreationDate { get; set; }
        public string User { get; set; }

        public Import()
        {
            CreationDate = DateTime.Now;
        }
    }
}
