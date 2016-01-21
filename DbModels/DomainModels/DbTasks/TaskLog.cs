using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DbModels.DomainModels.DbTasks
{
    /// <summary>
    /// Здесь хранятся результаты выполнения таска.
    /// </summary>
    public class TaskLog
    {
        public int Id { get; set; }
        public virtual DbTask DbTask { get; set; }
        public string UserName { get; set; }
        public DateTime? Time { get; set; }
        public string Status { get; set; }
        //public int? ResultInt1 { get; set; }
        //public int? ResultInt2 { get; set; }
        //public string ResultString1 { get; set; }
        //public string ResultString2 { get; set; }


        public TaskLog()
        {
            Time = DateTime.Now;
        }
    }
}