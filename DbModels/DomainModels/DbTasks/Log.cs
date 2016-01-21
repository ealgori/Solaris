using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.DbTasks
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public TaskLog TaskLog { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public byte[] File { get; set; }

        public Log()
        {
            DateTime = DateTime.Now;
        }
    }
}