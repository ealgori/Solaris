using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapItemSelect
{
    public class BaseSapItemsSelect:ISapItemSelect
    {
        public bool Select(List<SAPItemModel> sapItems, decimal? grQty, out List<GRItemModel> itemsForGr)
        {

            itemsForGr = new List<GRItemModel>();
            if (grQty <= 0)
                return false;
            //проверяем, что вообще наберется столько GR
            var maxGRAmmount = sapItems.Sum(i=>i.QtyOrdered-i.GRQty);
            if (maxGRAmmount >= grQty)
            {
                decimal? grCount = grQty;
                foreach (var item in sapItems)
                {
                    var possibleGrQty = item.QtyOrdered - item.GRQty;
                    if (possibleGrQty <= 0)
                        continue;
                    if (possibleGrQty>= grCount)
                    {
                        itemsForGr.Add(new GRItemModel() { POItemId= item.POItemId, MaterialCode = item.MaterialCode, Price = item.Price, Qty = grCount.Value });
                        grCount -= grCount;
                    }
                    else
                    {
                        itemsForGr.Add(new GRItemModel() { POItemId = item.POItemId, MaterialCode = item.MaterialCode, Price = item.Price, Qty = possibleGrQty });
                        grCount -= possibleGrQty;
                    }

                    if(grCount == 0)
                    {
                        return true;
                    }

            }
            }

            
            return false;
        }
    }
}
