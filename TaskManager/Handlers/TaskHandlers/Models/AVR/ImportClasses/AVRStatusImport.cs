using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Handlers.TaskHandlers.Models.AVR.ImportClasses
{
    public class AVRStatusImport
    {
        public string AVRId { get; set; }
        public bool? NeedPreprice { get; set; }
        public bool? ReadyForPOR { get; set; }
        public bool? ReadyForRequest { get; set; }
    }
}
