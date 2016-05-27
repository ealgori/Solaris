using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.Handle.HandleResult
{
    public class FirstHandleResult
    {
        /// <summary>
        /// принятые позиции
        /// </summary>
        public List<ShItemModel> ShModels { get; set; }
        public List<SAPItemModel> SAPRows { get; set; }
        public List<ShItemModel> ManGRItems { get; set; }

        /// <summary>
        /// Нам все равно нужны принятые позиции, даже если они старые, чтобы проставить на них ManualGR
        /// </summary>
        public bool Succeed { get; set; }
    }
}
