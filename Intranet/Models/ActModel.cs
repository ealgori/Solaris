using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Intranet.Models
{
    public class ActModel
    {
        public string TO { get; set; }

        public bool Filter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DbModels.SharedModels.ActItemModel> Services { get; set; }
        public List<DbModels.SharedModels.ActItemModel> Materials { get; set; }
    }
}