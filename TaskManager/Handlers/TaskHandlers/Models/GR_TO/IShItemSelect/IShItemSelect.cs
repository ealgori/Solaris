using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO
{
    public interface IShItemSelect
    {
        bool Select(List<ShItemModel> shItems, decimal? qty, out List<ShItemModel> selected);
      
    }
}
