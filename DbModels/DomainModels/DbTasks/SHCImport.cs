using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.DbTasks
{
    public class SHCImport
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Error { get; set; }
        public string Debug { get; set; }
    }
}