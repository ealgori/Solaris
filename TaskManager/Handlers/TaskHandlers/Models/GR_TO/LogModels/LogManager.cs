using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Handlers.TaskHandlers.Models.GR_TO.Models;

namespace TaskManager.Handlers.TaskHandlers.Models.GR_TO.LogModels
{
    public class LogManager
    {
        public List<LogModel> LogModels { get; set; }
        public string Key { get; set; }

        public LogManager()
        {
            LogModels = new List<LogModel>();
        }

        public void Add(LogModel logModel)
        {
            LogModels.Add(logModel);
        }

        public void Add(List<ShItemModel> shModels, List<SAPItemModel> sapModels, string text, LogStatus status)
        {
            var logModel = new LogModel() { Id=Key,  ShModels = shModels, SAPRows = sapModels, Message = text, Status = status };
            Add(logModel);
        }

        public List<LogModelRow> AsTable()
        {
            return LogModels.SelectMany(l => l.ToLogModelRows()).ToList();
        }
    }
}
