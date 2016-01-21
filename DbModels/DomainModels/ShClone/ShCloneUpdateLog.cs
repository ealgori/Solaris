using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbModels.DomainModels.DbTasks;

namespace DbModels.DomainModels.ShClone
{
    public class ShCloneUpdateLog
    {
        public int Id { get; set; }
        public virtual DbTask Task { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Success { get; set; }
        public string Comment { get; set; }

        public ShCloneUpdateLog()
        {
            StartTime = DateTime.Now;
        }
    }
}
