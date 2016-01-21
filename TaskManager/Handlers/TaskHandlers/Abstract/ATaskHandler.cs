using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using TaskManager.TaskParamModels;




namespace TaskManager.Handlers.TaskHandlers
{
    public abstract class ATaskHandler
    {
        public TaskParameters TaskParameters { get; set; }

        public ATaskHandler(TaskParameters taskParams)
        {
            TaskParameters = taskParams;
        }
        
        public abstract  bool Handle();
    }
}