using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.ImportFilesModels
{
    public class ImportFile:Entity
    {
        public string Name { get; set; }
        public byte[] File { get; set; }
        public bool Success { get; set; }
        public virtual ICollection<ImportFileLog> ImportFileLogs { get; set; }
        public virtual Import Import { get; set; }
    }
}
