using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models
{
    public class ShItemModel:IComparable<ShItemModel>
    {
        public string  Id { get; set; }
        //public string  Approved { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Price { get; set; }
        public string  GR { get; set; }
        public string MaterialCode { get; set; }
        public DateTime? TOFactDate { get; set; }
        public string TO { get; set; }
        public string PO { get; set; }

       

        public int CompareTo(ShItemModel obj)
        {
            return this.Id.CompareTo(obj.Id);
        }

      
    }


}
