using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManager.Handlers.FileIOHandlers
{
    public class FileHeader
    {
        public string HeadName { get; set; }
        public string PropName { get; set; }
        public int Column { get; set; }
        public Type Type { get; set; }
        public bool Required { get; set;}
    }
}