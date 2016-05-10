using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models
{
    public class SAPRow
    {
        public string POItem { get; set; }
        public string PO { get; set; }
        public decimal Price { get; set; }
        public decimal GRToBeDone { get; set; }
        public decimal GRQuantity { get; set; }
        public string SAPCode { get; set; }
    }
}
