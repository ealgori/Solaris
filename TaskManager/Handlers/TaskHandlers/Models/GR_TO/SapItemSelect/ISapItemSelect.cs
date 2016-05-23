using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapItemSelect
{
    public interface ISapItemSelect
    {
         bool Select(List<SAPItemModel> sapItems, decimal? grQty, out List<GRItemModel> itemsForGr);
    }
}
