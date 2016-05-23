using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle.HandleResult
{
    public class SecondHandlerResult
    {
        public List<ShItemModel> ShModels { get; set; }
        public List<GRItemModel> GRModels { get; set; }
        public bool Succeed { get; set; }
    }
}
