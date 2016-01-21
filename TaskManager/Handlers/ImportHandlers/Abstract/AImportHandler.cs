using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using TaskManager.TaskParamModels;

namespace TaskManager.Handlers.ImportHandlers
{
    public abstract class AImportHandler
    {
        public TaskParameters TaskParameters { get; set; }
        public AImportHandler(TaskParameters taskParameters)
        {
            TaskParameters = taskParameters;
        }

        public abstract bool Import();
    }
}