using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace TaskManager.Handlers.ConvertHandlers
{
    public abstract class AConvertSubHandler
    {
        public TaskLogger TaskLogger { get; set; }

        public AConvertSubHandler(TaskLogger taskLogger)
        {
            TaskLogger = taskLogger;
        }
       

        public abstract bool Handle();
        

    }
}