using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models
{
    public class SAPItemModel
    {
        public string POItemId { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal GRQty { get; set; }
        //public decimal GRToBeDone { get; set; }
        public decimal Price { get; set; }
        public string MaterialCode { get; set; }

        public override string ToString()
        {
            return $"Id:{POItemId} {GRQty} of {QtyOrdered} for {Price} {MaterialCode}";
        }
    }
}
