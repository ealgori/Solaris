using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models
{
    public class GRItemModel
    {
        public string POItemId  { get; set; }
        public string MaterialCode { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }
}
