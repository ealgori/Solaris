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
        //public decimal GRToBeDone { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal GRQty { get; set; }
        public string MaterialCode { get; set; }
        public string PODeletionIndicator { get; set;}
        public string Act { get; set; }
    }
}
