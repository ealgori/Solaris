using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.SapReader
{
    public interface ISapReader
    {
        List<SAPRow> Rows { get; set; }
        void  Read();
        List<string> Errors { get; set; }
        bool Succeed { get; set; }
    }
}
