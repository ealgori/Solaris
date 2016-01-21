using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbModels.Models.ImportFilesModels
{
    public class ImportFileLog:Entity
    {
        public string Level { get; set; }
        public string Message { get; set; }
        public ImportFile ImportFile { get; set; }

    }
}
